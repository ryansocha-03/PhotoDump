package main

import "github.com/rabbitmq/amqp091-go"

func InitializeQueueConnection(cfg *Config) (conn *amqp091.Connection, err error) {
	conn, err = amqp091.Dial(cfg.QueueConnection)
	return
}

func InitializeQueueChannel(conn *amqp091.Connection) (ch *amqp091.Channel, err error) {
	ch, err = conn.Channel()
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
