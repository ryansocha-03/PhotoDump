package appapi

import (
	"context"
	"fmt"
	"net/http"
	"strings"
	"time"

	"photodump/workers/internal/pipeline"
)

const defaultTimeout = 10 * time.Second

type MediaClient struct {
	baseURL         string
	tokenHeaderName string
	token           string
	httpClient      *http.Client
}

func NewMediaClient(baseURL, tokenHeaderName, token string, httpClient *http.Client) *MediaClient {
	if httpClient == nil {
		httpClient = &http.Client{Timeout: defaultTimeout}
	}

	return &MediaClient{
		baseURL:         strings.TrimRight(baseURL, "/"),
		tokenHeaderName: tokenHeaderName,
		token:           token,
		httpClient:      httpClient,
	}
}

func (c *MediaClient) MarkCompleted(ctx context.Context, mediaID int) *pipeline.ProcessingError {
	url := fmt.Sprintf("%s/internal/media/%d/complete", c.baseURL, mediaID)

	req, err := http.NewRequestWithContext(ctx, http.MethodPost, url, nil)
	if err != nil {
		return &pipeline.ProcessingError{
			Message: fmt.Sprintf("unable to build completion request for media %d: %v", mediaID, err),
			Requeue: true,
		}
	}

	req.Header.Set(c.tokenHeaderName, c.token)

	resp, err := c.httpClient.Do(req)
	if err != nil {
		return &pipeline.ProcessingError{
			Message: fmt.Sprintf("unable to call completion endpoint for media %d: %v", mediaID, err),
			Requeue: true,
		}
	}
	defer resp.Body.Close()

	switch resp.StatusCode {
	case http.StatusOK, http.StatusNoContent:
		return nil
	case http.StatusNotFound, http.StatusConflict, http.StatusUnauthorized, http.StatusForbidden:
		return &pipeline.ProcessingError{
			Message: fmt.Sprintf("completion endpoint rejected media %d with status %d", mediaID, resp.StatusCode),
			Requeue: false,
		}
	default:
		return &pipeline.ProcessingError{
			Message: fmt.Sprintf("completion endpoint returned status %d for media %d", resp.StatusCode, mediaID),
			Requeue: resp.StatusCode >= http.StatusInternalServerError,
		}
	}
}
