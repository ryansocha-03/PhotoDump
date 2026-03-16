package main

import (
	"context"
	"strings"
	"sync"
	"testing"
	"time"

	"github.com/rabbitmq/amqp091-go"
)

func TestConsumeMessagesAcksProcessedMessages(t *testing.T) {
	msgs := make(chan amqp091.Delivery, 1)
	recorder := &ackRecorder{}

	msgs <- amqp091.Delivery{
		Acknowledger: recorder,
		DeliveryTag:  1,
		Body:         []byte(`{"ObjectName":"img.jpg","MediaId":1}`),
	}
	close(msgs)

	err := consumeMessages(context.Background(), msgs, &Config{MaxWorkers: 1}, nil, func(_ *amqp091.Delivery, _ *ObjectStorage) *MessageError {
		return nil
	})
	if err == nil || !strings.Contains(err.Error(), "message channel was closed") {
		t.Fatalf("expected closed channel error, got %v", err)
	}

	if got := recorder.ackCount(); got != 1 {
		t.Fatalf("expected 1 ack, got %d", got)
	}

	if got := recorder.nackCount(); got != 0 {
		t.Fatalf("expected 0 nacks, got %d", got)
	}
}

func TestConsumeMessagesNacksFailedMessages(t *testing.T) {
	msgs := make(chan amqp091.Delivery, 1)
	recorder := &ackRecorder{}

	msgs <- amqp091.Delivery{
		Acknowledger: recorder,
		DeliveryTag:  2,
		Body:         []byte(`{"ObjectName":"img.jpg","MediaId":2}`),
	}
	close(msgs)

	err := consumeMessages(context.Background(), msgs, &Config{MaxWorkers: 1}, nil, func(_ *amqp091.Delivery, _ *ObjectStorage) *MessageError {
		return &MessageError{Message: "boom", Requeue: true}
	})
	if err == nil || !strings.Contains(err.Error(), "message channel was closed") {
		t.Fatalf("expected closed channel error, got %v", err)
	}

	if got := recorder.ackCount(); got != 0 {
		t.Fatalf("expected 0 acks, got %d", got)
	}

	if got := recorder.nackCount(); got != 1 {
		t.Fatalf("expected 1 nack, got %d", got)
	}

	if !recorder.lastNack().requeue {
		t.Fatal("expected message to be requeued on nack")
	}
}

func TestConsumeMessagesWaitsForInflightWorkersWhenChannelCloses(t *testing.T) {
	msgs := make(chan amqp091.Delivery, 1)
	recorder := &ackRecorder{}
	started := make(chan struct{})
	release := make(chan struct{})
	done := make(chan error, 1)

	msgs <- amqp091.Delivery{
		Acknowledger: recorder,
		DeliveryTag:  3,
		Body:         []byte(`{"ObjectName":"img.jpg","MediaId":3}`),
	}
	close(msgs)

	go func() {
		done <- consumeMessages(context.Background(), msgs, &Config{MaxWorkers: 1}, nil, func(_ *amqp091.Delivery, _ *ObjectStorage) *MessageError {
			close(started)
			<-release
			return nil
		})
	}()

	<-started

	select {
	case err := <-done:
		t.Fatalf("consumer returned before inflight worker completed: %v", err)
	case <-time.After(100 * time.Millisecond):
	}

	close(release)

	err := <-done
	if err == nil || !strings.Contains(err.Error(), "message channel was closed") {
		t.Fatalf("expected closed channel error, got %v", err)
	}

	if got := recorder.ackCount(); got != 1 {
		t.Fatalf("expected 1 ack after worker completion, got %d", got)
	}
}

type ackRecorder struct {
	mu        sync.Mutex
	ackCalls  []ackCall
	nackCalls []nackCall
}

type ackCall struct {
	tag      uint64
	multiple bool
}

type nackCall struct {
	tag      uint64
	multiple bool
	requeue  bool
}

func (r *ackRecorder) Ack(tag uint64, multiple bool) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	r.ackCalls = append(r.ackCalls, ackCall{tag: tag, multiple: multiple})
	return nil
}

func (r *ackRecorder) Nack(tag uint64, multiple bool, requeue bool) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	r.nackCalls = append(r.nackCalls, nackCall{tag: tag, multiple: multiple, requeue: requeue})
	return nil
}

func (r *ackRecorder) Reject(tag uint64, requeue bool) error {
	return nil
}

func (r *ackRecorder) ackCount() int {
	r.mu.Lock()
	defer r.mu.Unlock()

	return len(r.ackCalls)
}

func (r *ackRecorder) nackCount() int {
	r.mu.Lock()
	defer r.mu.Unlock()

	return len(r.nackCalls)
}

func (r *ackRecorder) lastNack() nackCall {
	r.mu.Lock()
	defer r.mu.Unlock()

	return r.nackCalls[len(r.nackCalls)-1]
}
