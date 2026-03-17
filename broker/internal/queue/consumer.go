package queue

import (
	"context"
	"fmt"
	"log"
	"sync"

	"photodump/workers/internal/config"
	"photodump/workers/internal/pipeline"

	"github.com/rabbitmq/amqp091-go"
)

func InitializeConnection(cfg *config.Config) (conn *amqp091.Connection, err error) {
	conn, err = amqp091.Dial(cfg.QueueConnection)
	return
}

func InitializeChannel(conn *amqp091.Connection, cfg *config.Config) (ch *amqp091.Channel, err error) {
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

func DeclareQueue(ch *amqp091.Channel, queueName string) (*amqp091.Queue, error) {
	queueObj, err := ch.QueueDeclare(
		queueName,
		true,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		return nil, err
	}

	return &queueObj, nil
}

func RunConsumer(ctx context.Context, ch *amqp091.Channel, cfg *config.Config, processor pipeline.Processor) error {
	msgs, err := ch.Consume(cfg.QueueName, "", false, false, false, false, nil)
	if err != nil {
		return err
	}

	return consumeMessages(ctx, msgs, cfg, processor)
}

func consumeMessages(ctx context.Context, msgs <-chan amqp091.Delivery, cfg *config.Config, processor pipeline.Processor) error {
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

			go func(delivery amqp091.Delivery) {
				sem <- struct{}{}
				defer func() {
					<-sem
					waitGroup.Done()
				}()

				msgErr := processor.ProcessMessage(context.WithoutCancel(ctx), delivery.Body)
				if msgErr != nil {
					log.Printf("Error processing message: %v\n", msgErr.Error())
					if nackErr := delivery.Nack(false, msgErr.Requeue); nackErr != nil {
						log.Printf("Issue nacking message: %v\n", nackErr)
					}
					return
				}

				if ackErr := delivery.Ack(false); ackErr != nil {
					log.Printf("Issue acking message: %v\n", ackErr)
				}
			}(msg)
		}
	}
}
