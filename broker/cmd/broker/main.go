package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"syscall"

	"photodump/workers/internal/appapi"
	"photodump/workers/internal/config"
	mediaimage "photodump/workers/internal/media/image"
	"photodump/workers/internal/pipeline"
	"photodump/workers/internal/queue"
	"photodump/workers/internal/storage"
)

func main() {
	cfg, err := config.Load()
	failOnError(err, "Issue loading config")

	imageProcessor, err := mediaimage.NewProcessor()
	failOnError(err, "Issue initializing image processing")

	objectStorage, err := storage.New(cfg)
	failOnError(err, "Issue initializing object storage")

	log.Printf("Connected to object storage at: %v", cfg.ContentStoreURL)

	mediaClient := appapi.NewMediaClient(cfg.AppApiBaseUrl, cfg.TokenHeaderName, cfg.AppApiToken, nil)

	imagePipeline, err := pipeline.NewImageVariantPipeline(objectStorage, imageProcessor, mediaimage.DefaultVariantSpecs(), mediaClient)
	failOnError(err, "Issue creating image processing pipeline")

	queueConn, err := queue.InitializeConnection(cfg)
	failOnError(err, "Issue creating connection to broker")
	defer queueConn.Close()

	queueCh, err := queue.InitializeChannel(queueConn, cfg)
	failOnError(err, "Issue creating broker channel")
	defer queueCh.Close()

	declaredQueue, err := queue.DeclareQueue(queueCh, cfg.QueueName)
	failOnError(err, "Issue declaring queue")

	log.Printf("Connected to broker with queue: %v", declaredQueue.Name)

	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	if err = queue.RunConsumer(ctx, queueCh, cfg, imagePipeline); err != nil {
		failOnError(err, "Consumer issue")
	}
}

func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %v", msg, err)
	}
}
