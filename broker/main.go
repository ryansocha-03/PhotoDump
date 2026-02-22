package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"syscall"
)

func main() {
	cfg, err := LoadConfig()
	FailOnError(err, "Issue loading config")

	objectStorageClient, err := InitializeObjectStorage(cfg)
	FailOnError(err, "Issue initializing object storage")

	log.Printf("Connected to object storage at: %v", objectStorageClient.EndpointURL().Host)

	queueConn, err := InitializeQueueConnection(cfg)
	FailOnError(err, "Issue creating connection to broker")
	defer queueConn.Close()

	queueCh, err := InitializeQueueChannel(queueConn, cfg)
	FailOnError(err, "Issue creating broker channel")
	defer queueCh.Close()

	queue, err := DeclareQueue(queueCh, cfg.QueueName)
	FailOnError(err, "Issue declaring queue")

	log.Printf("Connected to broker with queue: %v", queue.Name)

	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	if err = RunConsumer(ctx, queueCh, cfg); err != nil {
		FailOnError(err, "Consumer issue. Details: ")
	}
}
