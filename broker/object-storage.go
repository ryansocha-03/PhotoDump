package main

import (
	"context"
	"errors"
	"fmt"
	"io"

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
}

type ObjectStorage struct {
	client objectClient
	bucket string
}

type minioObjectClient struct {
	client *minio.Client
}

func (c *minioObjectClient) GetObject(ctx context.Context, bucketName, objectName string, opts minio.GetObjectOptions) (objectReader, error) {
	return c.client.GetObject(ctx, bucketName, objectName, opts)
}

func InitializeObjectStorage(cfg *Config) (storage *ObjectStorage, err error) {
	ctx := context.Background()

	client, err := minio.New(cfg.ContentStoreUrl, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.ContentStoreKey, cfg.ContentStoreSecret, ""),
		Secure: cfg.ContentStoreSecure,
	})

	if err != nil {
		err = errors.New("Unable to instantiate object storage client: " + err.Error())
		return
	}

	exists, errExists := client.BucketExists(ctx, cfg.ContentStoreBucket)

	if errExists != nil {
		err = errors.New("Unable to verify if target bucket exists: " + errExists.Error())
		return
	}

	if !exists {
		err = errors.New("Configured bucket does not exist")
	}

	if err != nil {
		return nil, err
	}

	return &ObjectStorage{
		client: &minioObjectClient{client: client},
		bucket: cfg.ContentStoreBucket,
	}, nil
}

func (s *ObjectStorage) GetOriginalObject(ctx context.Context, objectName string) ([]byte, *MessageError) {
	object, err := s.client.GetObject(ctx, s.bucket, objectName, minio.GetObjectOptions{})
	if err != nil {
		return nil, getOriginalObjectError(objectName, err)
	}
	defer object.Close()

	if _, err = object.Stat(); err != nil {
		return nil, getOriginalObjectError(objectName, err)
	}

	data, err := io.ReadAll(object)
	if err != nil {
		return nil, &MessageError{
			Message: fmt.Sprintf("unable to read original object %q: %v", objectName, err),
			Requeue: true,
		}
	}

	return data, nil
}

func getOriginalObjectError(objectName string, err error) *MessageError {
	errorResponse := minio.ToErrorResponse(err)
	if errorResponse.Code == "NoSuchKey" || errorResponse.Code == "NoSuchObject" || errorResponse.Code == "NotFound" {
		return &MessageError{
			Message: fmt.Sprintf("original object %q was not found in object storage", objectName),
			Requeue: false,
		}
	}

	return &MessageError{
		Message: fmt.Sprintf("unable to fetch original object %q: %v", objectName, err),
		Requeue: true,
	}
}
