package main

import (
	"fmt"
	"log"
)

func FailOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}

type MessageError struct {
	Requeue bool
	Message string
}

func (e *MessageError) Error() string {
	return fmt.Sprintf("Message Error. Requeue: %v: %v", e.Requeue, e.Message)
}
