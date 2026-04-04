package appapi

import (
	"context"
	"errors"
	"io"
	"net/http"
	"strings"
	"testing"
)

func TestMarkCompletedTreatsOkAndNoContentAsSuccess(t *testing.T) {
	testCases := []int{
		http.StatusOK,
		http.StatusNoContent,
	}

	for _, statusCode := range testCases {
		t.Run(http.StatusText(statusCode), func(t *testing.T) {
			var requestPath string
			var tokenHeader string

			httpClient := &http.Client{
				Transport: roundTripFunc(func(req *http.Request) (*http.Response, error) {
					requestPath = req.URL.Path
					tokenHeader = req.Header.Get("X-Worker-Token")
					return &http.Response{
						StatusCode: statusCode,
						Body:       io.NopCloser(strings.NewReader("")),
						Header:     make(http.Header),
					}, nil
				}),
			}

			client := NewMediaClient("http://app-api:8080", "X-Worker-Token", "test-token", httpClient)

			if msgErr := client.MarkCompleted(context.Background(), 123); msgErr != nil {
				t.Fatalf("expected success, got %v", msgErr)
			}

			if requestPath != "/internal/media/123/complete" {
				t.Fatalf("expected request path to be %q, got %q", "/internal/media/123/complete", requestPath)
			}

			if tokenHeader != "test-token" {
				t.Fatalf("expected token header to be %q, got %q", "test-token", tokenHeader)
			}
		})
	}
}

func TestMarkCompletedTreatsNonRetryableStatusesAsTerminal(t *testing.T) {
	testCases := []int{
		http.StatusNotFound,
		http.StatusConflict,
		http.StatusUnauthorized,
		http.StatusForbidden,
	}

	for _, statusCode := range testCases {
		t.Run(http.StatusText(statusCode), func(t *testing.T) {
			httpClient := &http.Client{
				Transport: roundTripFunc(func(req *http.Request) (*http.Response, error) {
					return &http.Response{
						StatusCode: statusCode,
						Body:       io.NopCloser(strings.NewReader("")),
						Header:     make(http.Header),
					}, nil
				}),
			}

			client := NewMediaClient("http://app-api:8080", "X-Worker-Token", "test-token", httpClient)
			msgErr := client.MarkCompleted(context.Background(), 123)
			if msgErr == nil {
				t.Fatal("expected processing error")
			}

			if msgErr.Requeue {
				t.Fatalf("expected status %d to be non-retryable", statusCode)
			}
		})
	}
}

func TestMarkCompletedTreatsServerErrorAsRetryable(t *testing.T) {
	httpClient := &http.Client{
		Transport: roundTripFunc(func(req *http.Request) (*http.Response, error) {
			return &http.Response{
				StatusCode: http.StatusInternalServerError,
				Body:       io.NopCloser(strings.NewReader("")),
				Header:     make(http.Header),
			}, nil
		}),
	}

	client := NewMediaClient("http://app-api:8080", "X-Worker-Token", "test-token", httpClient)
	msgErr := client.MarkCompleted(context.Background(), 123)
	if msgErr == nil {
		t.Fatal("expected processing error")
	}

	if !msgErr.Requeue {
		t.Fatal("expected server error to be retryable")
	}
}

func TestMarkCompletedTreatsTransportFailureAsRetryable(t *testing.T) {
	httpClient := &http.Client{
		Transport: roundTripFunc(func(req *http.Request) (*http.Response, error) {
			return nil, errors.New("dial tcp: failed")
		}),
	}

	client := NewMediaClient("http://app-api:8080", "X-Worker-Token", "test-token", httpClient)
	msgErr := client.MarkCompleted(context.Background(), 123)
	if msgErr == nil {
		t.Fatal("expected processing error")
	}

	if !msgErr.Requeue {
		t.Fatal("expected transport error to be retryable")
	}
}

func TestMarkCompletedTreatsUnexpectedClientErrorAsNonRetryable(t *testing.T) {
	httpClient := &http.Client{
		Transport: roundTripFunc(func(req *http.Request) (*http.Response, error) {
			return &http.Response{
				StatusCode: http.StatusBadRequest,
				Body:       io.NopCloser(strings.NewReader("")),
				Header:     make(http.Header),
			}, nil
		}),
	}

	client := NewMediaClient("http://app-api:8080", "X-Worker-Token", "test-token", httpClient)
	msgErr := client.MarkCompleted(context.Background(), 123)
	if msgErr == nil {
		t.Fatal("expected processing error")
	}

	if msgErr.Requeue {
		t.Fatal("expected bad request to be non-retryable")
	}
}

func TestMarkCompletedIncludesBuildErrorInMessage(t *testing.T) {
	client := NewMediaClient("://bad-url", "X-Worker-Token", "test-token", nil)
	msgErr := client.MarkCompleted(context.Background(), 123)
	if msgErr == nil {
		t.Fatal("expected processing error")
	}

	if !msgErr.Requeue {
		t.Fatal("expected request build error to be retryable")
	}

	if !strings.Contains(msgErr.Message, "unable to build completion request") {
		t.Fatalf("unexpected error message %q", msgErr.Message)
	}
}

type roundTripFunc func(*http.Request) (*http.Response, error)

func (fn roundTripFunc) RoundTrip(req *http.Request) (*http.Response, error) {
	return fn(req)
}
