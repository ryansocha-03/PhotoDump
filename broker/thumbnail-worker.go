package main

import (
	"encoding/json"
	"log"
	"time"

	"github.com/rabbitmq/amqp091-go"
)

type ThumbnailGenerationMessage struct {
	ObjectName string `json:"ObjectName"`
	MediaId    int    `json:"MediaId"`
}

func ProcessMessage(msg *amqp091.Delivery) (err error) {
	var msgData ThumbnailGenerationMessage
	err = json.Unmarshal(msg.Body, &msgData)
	if err != nil {
		return
	}
	log.Println("About to go to sleep...")

	time.Sleep(time.Second * 30)

	log.Println("Arise")

	log.Printf("Object Name: %v\nMedia ID: %v", msgData.ObjectName, msgData.MediaId)

	return
}
