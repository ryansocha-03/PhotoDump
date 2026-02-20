package main

import (
	"context"
	"errors"

	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
)

func InitializeObjectStorage(cfg *Config) (client *minio.Client, err error) {
	ctx := context.Background()
	useSSL := false

	client, err = minio.New(cfg.ContentStoreUrl, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.ContentStoreKey, cfg.ContentStoreSecret, ""),
		Secure: useSSL,
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

	return
}
