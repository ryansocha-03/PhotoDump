package main

import (
	"context"
	"fmt"
	"log"
	"sync"

	"github.com/rabbitmq/amqp091-go"
)

func InitializeQueueConnection(cfg *Config) (conn *amqp091.Connection, err error) {
	conn, err = amqp091.Dial(cfg.QueueConnection)
	return
}

func InitializeQueueChannel(conn *amqp091.Connection, cfg *Config) (ch *amqp091.Channel, err error) {
	ch, err = conn.Channel()
	ch.Qos(cfg.MaxMessages, 0, true)
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

func RunConsumer(ctx context.Context, ch *amqp091.Channel, cfg *Config) error {
	msgs, err := ch.Consume(cfg.QueueName, "", false, false, false, false, nil)
	if err != nil {
		return err
	}

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
				return fmt.Errorf("Message channel was closed.") // TODO: "gracefully" handle message channel closure
			}

			log.Println("Processing message")

			waitGroup.Add(1)

			go func(mes *amqp091.Delivery) {
				sem <- struct{}{}
				defer func() {
					<-sem
					waitGroup.Done()
				}()

				msgErr := ProcessMessage(mes)
				if msgErr != nil {
					log.Printf("Error processing message: %v\n", err.Error())
					msg.Nack(false, msgErr.Requeue)
					return
				}

				msg.Ack(false)
			}(&msg)
		}
	}
}
