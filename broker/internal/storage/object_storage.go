package storage

import (
	"bytes"
	"context"
	"errors"
	"fmt"
	"io"

	"photodump/workers/internal/config"

	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
)

type objectReader interface {
	io.Reader
	io.Closer
	Stat() (minio.ObjectInfo, error)
}

type objectClient interface {
	GetObject(ctx context.Context, bucketName, objectName string, opts minio.GetObjectOptions) (objectReader, error)
	PutObject(ctx context.Context, bucketName, objectName string, reader io.Reader, objectSize int64, opts minio.PutObjectOptions) (minio.UploadInfo, error)
}

type Store struct {
	client objectClient
	bucket string
}

type minioObjectClient struct {
	client *minio.Client
}

type ObjectNotFoundError struct {
	ObjectName string
}

func (e *ObjectNotFoundError) Error() string {
	return fmt.Sprintf("object %q was not found", e.ObjectName)
}

func IsObjectNotFound(err error) bool {
	var target *ObjectNotFoundError
	return errors.As(err, &target)
}

func (c *minioObjectClient) GetObject(ctx context.Context, bucketName, objectName string, opts minio.GetObjectOptions) (objectReader, error) {
	return c.client.GetObject(ctx, bucketName, objectName, opts)
}

func (c *minioObjectClient) PutObject(ctx context.Context, bucketName, objectName string, reader io.Reader, objectSize int64, opts minio.PutObjectOptions) (minio.UploadInfo, error) {
	return c.client.PutObject(ctx, bucketName, objectName, reader, objectSize, opts)
}

func New(cfg *config.Config) (*Store, error) {
	ctx := context.Background()

	client, err := minio.New(cfg.ContentStoreURL, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.ContentStoreKey, cfg.ContentStoreSecret, ""),
		Secure: cfg.ContentStoreSecure,
	})
	if err != nil {
		return nil, fmt.Errorf("unable to instantiate object storage client: %w", err)
	}

	exists, err := client.BucketExists(ctx, cfg.ContentStoreBucket)
	if err != nil {
		return nil, fmt.Errorf("unable to verify if target bucket exists: %w", err)
	}

	if !exists {
		return nil, errors.New("configured bucket does not exist")
	}

	return &Store{
		client: &minioObjectClient{client: client},
		bucket: cfg.ContentStoreBucket,
	}, nil
}

func (s *Store) GetObject(ctx context.Context, objectName string) ([]byte, error) {
	object, err := s.client.GetObject(ctx, s.bucket, objectName, minio.GetObjectOptions{})
	if err != nil {
		return nil, mapGetObjectError(objectName, err)
	}
	defer object.Close()

	if _, err = object.Stat(); err != nil {
		return nil, mapGetObjectError(objectName, err)
	}

	data, err := io.ReadAll(object)
	if err != nil {
		return nil, fmt.Errorf("unable to read object %q: %w", objectName, err)
	}

	return data, nil
}

func (s *Store) PutObject(ctx context.Context, objectName string, data []byte, contentType string) error {
	_, err := s.client.PutObject(
		ctx,
		s.bucket,
		objectName,
		bytes.NewReader(data),
		int64(len(data)),
		minio.PutObjectOptions{ContentType: contentType},
	)
	if err != nil {
		return fmt.Errorf("unable to upload object %q: %w", objectName, err)
	}

	return nil
}

func mapGetObjectError(objectName string, err error) error {
	errorResponse := minio.ToErrorResponse(err)
	if errorResponse.Code == "NoSuchKey" || errorResponse.Code == "NoSuchObject" || errorResponse.Code == "NotFound" {
		return &ObjectNotFoundError{ObjectName: objectName}
	}

	return err
}
