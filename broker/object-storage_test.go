package main

import (
	"bytes"
	"context"
	"errors"
	"strings"
	"testing"

	"github.com/minio/minio-go/v7"
)

func TestGetOriginalObjectDownloadsBytes(t *testing.T) {
	object := newFakeObject([]byte("image-bytes"), nil, nil)
	client := &fakeObjectClient{object: object}
	storage := &ObjectStorage{
		client: client,
		bucket: "photos",
	}

	data, err := storage.GetOriginalObject(context.Background(), "folder/original.png")
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

func TestGetOriginalObjectReturnsNonRequeueWhenObjectIsMissing(t *testing.T) {
	object := newFakeObject(nil, minio.ErrorResponse{Code: "NoSuchKey"}, nil)
	storage := &ObjectStorage{
		client: &fakeObjectClient{object: object},
		bucket: "photos",
	}

	data, err := storage.GetOriginalObject(context.Background(), "folder/missing.png")
	if err == nil {
		t.Fatalf("expected missing object error, got data %q", string(data))
	}

	if err.Requeue {
		t.Fatal("expected missing object error to avoid requeue")
	}

	if !strings.Contains(err.Message, "not found") {
		t.Fatalf("expected missing object message, got %q", err.Message)
	}

	if !object.closed {
		t.Fatal("expected missing object handle to be closed")
	}
}

func TestGetOriginalObjectReturnsRequeueOnReadFailure(t *testing.T) {
	object := newFakeObject(nil, nil, errors.New("read failed"))
	storage := &ObjectStorage{
		client: &fakeObjectClient{object: object},
		bucket: "photos",
	}

	data, err := storage.GetOriginalObject(context.Background(), "folder/original.png")
	if err == nil {
		t.Fatalf("expected read error, got data %q", string(data))
	}

	if !err.Requeue {
		t.Fatal("expected read failure to requeue")
	}

	if !strings.Contains(err.Message, "unable to read original object") {
		t.Fatalf("expected read failure message, got %q", err.Message)
	}

	if !object.closed {
		t.Fatal("expected object to be closed after read failure")
	}
}

type fakeObjectClient struct {
	object     objectReader
	err        error
	bucketName string
	objectName string
}

func (c *fakeObjectClient) GetObject(_ context.Context, bucketName, objectName string, _ minio.GetObjectOptions) (objectReader, error) {
	c.bucketName = bucketName
	c.objectName = objectName

	if c.err != nil {
		return nil, c.err
	}

	return c.object, nil
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
