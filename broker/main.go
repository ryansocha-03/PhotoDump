package main

import (
	"log"
)

func main() {
	cfg, err := LoadConfig()
	FailOnError(err, "Issue loading config")

	objectStorageClient, err := InitializeObjectStorage(cfg)
	FailOnError(err, "Issue initializing object storage")

	log.Printf("We did it! %v", objectStorageClient.EndpointURL().Host)

	queueConn, err := InitializeQueueConnection(cfg)
	FailOnError(err, "Issue creating connection to broker")
	defer queueConn.Close()

	queueCh, err := InitializeQueueChannel(queueConn)
	FailOnError(err, "Issue creating broker channel")
	defer queueCh.Close()

	queue, err := DeclareQueue(queueCh, cfg.QueueName)
	FailOnError(err, "Issue declaring queue")

	log.Println(queue.Name)
}
