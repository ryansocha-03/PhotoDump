package main

import (
	"context"
	"fmt"
	"log"
	"sync"

	"github.com/minio/minio-go/v7"
	"github.com/rabbitmq/amqp091-go"
)

type messageProcessor func(*amqp091.Delivery, *minio.Client) *MessageError

func InitializeQueueConnection(cfg *Config) (conn *amqp091.Connection, err error) {
	conn, err = amqp091.Dial(cfg.QueueConnection)
	return
}

func InitializeQueueChannel(conn *amqp091.Connection, cfg *Config) (ch *amqp091.Channel, err error) {
	ch, err = conn.Channel()
	if err != nil {
		return nil, err
	}

	if err = ch.Qos(cfg.MaxMessages, 0, true); err != nil {
		_ = ch.Close()
		return nil, err
	}

	return
}

func DeclareQueue(ch *amqp091.Channel, qName string) (q *amqp091.Queue, err error) {
	queueObj, err := ch.QueueDeclare(
		qName,
		true,
		false,
		false,
		false,
		nil,
	)

	return &queueObj, err
}

func RunConsumer(ctx context.Context, ch *amqp091.Channel, cfg *Config, objstr *minio.Client) error {
	msgs, err := ch.Consume(cfg.QueueName, "", false, false, false, false, nil)
	if err != nil {
		return err
	}

	return consumeMessages(ctx, msgs, cfg, objstr, ProcessMessage)
}

func consumeMessages(ctx context.Context, msgs <-chan amqp091.Delivery, cfg *Config, objstr *minio.Client, processor messageProcessor) error {
	var waitGroup sync.WaitGroup

	log.Printf("Max workers: %v", cfg.MaxWorkers)

	sem := make(chan struct{}, cfg.MaxWorkers)

	log.Println("Waiting for messages. Press Ctrl + C to exit...")

	for {
		select {
		case <-ctx.Done():
			log.Println("Shutdown signal received. Waiting for in-flight jobs to finish...")
			waitGroup.Wait()
			return nil
		case msg, ok := <-msgs:
			if !ok {
				log.Println("Message channel closed. Waiting for in-flight jobs to finish...")
				waitGroup.Wait()
				return fmt.Errorf("message channel was closed")
			}

			log.Println("Processing message")

			waitGroup.Add(1)

			go func(mes amqp091.Delivery) {
				sem <- struct{}{}
				defer func() {
					<-sem
					waitGroup.Done()
				}()

				msgErr := processor(&mes, objstr)
				if msgErr != nil {
					log.Printf("Error processing message: %v\n", msgErr.Error())
					if nackErr := mes.Nack(false, msgErr.Requeue); nackErr != nil {
						log.Printf("Issue nacking message: %v\n", nackErr)
					}
					return
				}

				if ackErr := mes.Ack(false); ackErr != nil {
					log.Printf("Issue acking message: %v\n", ackErr)
				}
			}(msg)
		}
	}
}
