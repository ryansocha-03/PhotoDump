package main

import (
	"context"
	"log"

	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
)

func main() {
	ctx := context.Background()

	cfg, cfgErr := LoadConfig()

	if cfgErr != nil {
		log.Fatal(cfgErr.Error())
	}

	useSSL := false

	minioClient, objErr := minio.New(cfg.ContentStoreUrl, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.ContentStoreKey, cfg.ContentStoreSecret, ""),
		Secure: useSSL,
	})

	if objErr != nil {
		log.Fatalf("Unable to instantiate object storage client: %v", objErr)
	}

	exists, errExists := minioClient.BucketExists(ctx, cfg.ContentStoreBucket)

	if errExists != nil {
		log.Println(errExists.Error())
	}

	if exists {
		log.Println("We have a bucket")
	} else {
		log.Println("No bucket")
	}
}
