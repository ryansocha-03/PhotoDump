package storage

import (
	"bytes"
	"context"
	"errors"
	"io"
	"strings"
	"testing"

	"github.com/minio/minio-go/v7"
)

func TestGetObjectDownloadsBytes(t *testing.T) {
	object := newFakeObject([]byte("image-bytes"), nil, nil)
	client := &fakeObjectClient{object: object}
	store := &Store{
		client: client,
		bucket: "photos",
	}

	data, err := store.GetObject(context.Background(), "folder/original.png")
	if err != nil {
		t.Fatalf("expected object download to succeed, got %v", err)
	}

	if string(data) != "image-bytes" {
		t.Fatalf("expected downloaded bytes to match source object, got %q", string(data))
	}

	if !object.closed {
		t.Fatal("expected object to be closed after download")
	}

	if client.bucketName != "photos" {
		t.Fatalf("expected bucket name to be photos, got %q", client.bucketName)
	}

	if client.objectName != "folder/original.png" {
		t.Fatalf("expected object name to be passed through, got %q", client.objectName)
	}
}

func TestGetObjectReturnsObjectNotFoundWhenMissing(t *testing.T) {
	object := newFakeObject(nil, minio.ErrorResponse{Code: "NoSuchKey"}, nil)
	store := &Store{
		client: &fakeObjectClient{object: object},
		bucket: "photos",
	}

	data, err := store.GetObject(context.Background(), "folder/missing.png")
	if err == nil {
		t.Fatalf("expected missing object error, got data %q", string(data))
	}

	if !IsObjectNotFound(err) {
		t.Fatalf("expected object not found error, got %v", err)
	}

	if !object.closed {
		t.Fatal("expected missing object handle to be closed")
	}
}

func TestGetObjectReturnsReadFailure(t *testing.T) {
	object := newFakeObject(nil, nil, errors.New("read failed"))
	store := &Store{
		client: &fakeObjectClient{object: object},
		bucket: "photos",
	}

	data, err := store.GetObject(context.Background(), "folder/original.png")
	if err == nil {
		t.Fatalf("expected read error, got data %q", string(data))
	}

	if !strings.Contains(err.Error(), "unable to read object") {
		t.Fatalf("expected read failure message, got %q", err.Error())
	}

	if !object.closed {
		t.Fatal("expected object to be closed after read failure")
	}
}

func TestPutObjectUploadsBytesWithContentType(t *testing.T) {
	client := &fakeObjectClient{}
	store := &Store{
		client: client,
		bucket: "photos",
	}

	err := store.PutObject(context.Background(), "folder/variants/gallery.jpg", []byte("thumb"), "image/jpeg")
	if err != nil {
		t.Fatalf("expected object upload to succeed, got %v", err)
	}

	if client.putBucketName != "photos" {
		t.Fatalf("expected upload bucket name to be photos, got %q", client.putBucketName)
	}

	if client.putObjectName != "folder/variants/gallery.jpg" {
		t.Fatalf("expected upload object name to be passed through, got %q", client.putObjectName)
	}

	if client.putContentType != "image/jpeg" {
		t.Fatalf("expected upload content type image/jpeg, got %q", client.putContentType)
	}

	if string(client.putData) != "thumb" {
		t.Fatalf("expected uploaded data to match, got %q", string(client.putData))
	}
}

type fakeObjectClient struct {
	object         objectReader
	err            error
	bucketName     string
	objectName     string
	putBucketName  string
	putObjectName  string
	putContentType string
	putData        []byte
}

func (c *fakeObjectClient) GetObject(_ context.Context, bucketName, objectName string, _ minio.GetObjectOptions) (objectReader, error) {
	c.bucketName = bucketName
	c.objectName = objectName

	if c.err != nil {
		return nil, c.err
	}

	return c.object, nil
}

func (c *fakeObjectClient) PutObject(_ context.Context, bucketName, objectName string, reader io.Reader, _ int64, opts minio.PutObjectOptions) (minio.UploadInfo, error) {
	c.putBucketName = bucketName
	c.putObjectName = objectName
	c.putContentType = opts.ContentType

	data, err := io.ReadAll(reader)
	if err != nil {
		return minio.UploadInfo{}, err
	}
	c.putData = data

	return minio.UploadInfo{}, nil
}

type fakeObject struct {
	reader  *bytes.Reader
	statErr error
	readErr error
	closed  bool
}

func newFakeObject(data []byte, statErr error, readErr error) *fakeObject {
	return &fakeObject{
		reader:  bytes.NewReader(data),
		statErr: statErr,
		readErr: readErr,
	}
}

func (o *fakeObject) Read(p []byte) (int, error) {
	if o.readErr != nil {
		return 0, o.readErr
	}

	return o.reader.Read(p)
}

func (o *fakeObject) Close() error {
	o.closed = true
	return nil
}

func (o *fakeObject) Stat() (minio.ObjectInfo, error) {
	return minio.ObjectInfo{}, o.statErr
}
