package pipeline

import (
	"context"
	"errors"
	"strings"
	"testing"

	mediaimage "photodump/workers/internal/media/image"
	"photodump/workers/internal/storage"
)

func TestProcessMessageReturnsNonRetryableWhenObjectMissing(t *testing.T) {
	pipe, err := NewImageVariantPipeline(
		&fakeStore{getErr: &storage.ObjectNotFoundError{ObjectName: "missing.jpg"}},
		&fakeGenerator{},
		mediaimage.DefaultVariantSpecs(),
	)
	if err != nil {
		t.Fatalf("expected pipeline creation to succeed, got %v", err)
	}

	msgErr := pipe.ProcessMessage(context.Background(), []byte(`{"ObjectName":"missing.jpg","MediaId":10}`))
	if msgErr == nil {
		t.Fatal("expected processing error for missing object")
	}

	if msgErr.Requeue {
		t.Fatal("expected missing object to avoid requeue")
	}
}

func TestProcessMessageReturnsNonRetryableForInvalidImage(t *testing.T) {
	pipe, err := NewImageVariantPipeline(
		&fakeStore{getData: []byte("original")},
		&fakeGenerator{err: &mediaimage.InvalidImageError{Err: errors.New("invalid image")}},
		mediaimage.DefaultVariantSpecs(),
	)
	if err != nil {
		t.Fatalf("expected pipeline creation to succeed, got %v", err)
	}

	msgErr := pipe.ProcessMessage(context.Background(), []byte(`{"ObjectName":"photo.jpg","MediaId":10}`))
	if msgErr == nil {
		t.Fatal("expected processing error for invalid image")
	}

	if msgErr.Requeue {
		t.Fatal("expected invalid image to avoid requeue")
	}
}

func TestProcessMessageUploadsGeneratedVariants(t *testing.T) {
	store := &fakeStore{getData: []byte("original")}
	generator := &fakeGenerator{
		variants: []mediaimage.GeneratedVariant{
			{
				Name:        "gallery",
				Bytes:       []byte("thumb"),
				ContentType: "image/jpeg",
				Extension:   "jpg",
				Width:       320,
				Height:      320,
			},
		},
	}

	pipe, err := NewImageVariantPipeline(store, generator, mediaimage.DefaultVariantSpecs())
	if err != nil {
		t.Fatalf("expected pipeline creation to succeed, got %v", err)
	}

	msgErr := pipe.ProcessMessage(context.Background(), []byte(`{"ObjectName":"abc/public/photo.png","MediaId":10}`))
	if msgErr != nil {
		t.Fatalf("expected processing to succeed, got %v", msgErr)
	}

	if got := len(store.putCalls); got != 1 {
		t.Fatalf("expected 1 uploaded variant, got %d", got)
	}

	if store.putCalls[0].objectName != "abc/public/photo_gallery.jpg" {
		t.Fatalf("unexpected variant object name %q", store.putCalls[0].objectName)
	}

	if store.putCalls[0].contentType != "image/jpeg" {
		t.Fatalf("unexpected variant content type %q", store.putCalls[0].contentType)
	}

	if string(generator.receivedOriginal) != "original" {
		t.Fatalf("expected generator to receive original object bytes, got %q", string(generator.receivedOriginal))
	}

	if len(generator.receivedSpecs) != 1 || generator.receivedSpecs[0].Name != "gallery" {
		t.Fatalf("expected generator to receive default gallery spec, got %+v", generator.receivedSpecs)
	}
}

func TestVariantObjectNamePreservesThreePartObjectPath(t *testing.T) {
	variantObjectName := VariantObjectName(
		"4ccdf5c0-5648-4ebd-a2b7-63b75abcddd6/public/8bef58d7944a467aa64d6be902b4e177.png",
		mediaimage.GeneratedVariant{
			Name:      "gallery",
			Extension: "jpg",
		},
	)

	if variantObjectName != "4ccdf5c0-5648-4ebd-a2b7-63b75abcddd6/public/8bef58d7944a467aa64d6be902b4e177_gallery.jpg" {
		t.Fatalf("unexpected variant object name %q", variantObjectName)
	}
}

func TestProcessMessageReturnsRetryableWhenUploadFails(t *testing.T) {
	pipe, err := NewImageVariantPipeline(
		&fakeStore{
			getData: []byte("original"),
			putErr:  errors.New("upload failed"),
		},
		&fakeGenerator{
			variants: []mediaimage.GeneratedVariant{
				{
					Name:        "gallery",
					Bytes:       []byte("thumb"),
					ContentType: "image/jpeg",
					Extension:   "jpg",
				},
			},
		},
		mediaimage.DefaultVariantSpecs(),
	)
	if err != nil {
		t.Fatalf("expected pipeline creation to succeed, got %v", err)
	}

	msgErr := pipe.ProcessMessage(context.Background(), []byte(`{"ObjectName":"photo.jpg","MediaId":10}`))
	if msgErr == nil {
		t.Fatal("expected upload failure")
	}

	if !msgErr.Requeue {
		t.Fatal("expected upload failure to requeue")
	}

	if !strings.Contains(msgErr.Message, "unable to upload variant") {
		t.Fatalf("expected upload failure message, got %q", msgErr.Message)
	}
}

type fakeStore struct {
	getData  []byte
	getErr   error
	putErr   error
	putCalls []putCall
}

type putCall struct {
	objectName  string
	data        []byte
	contentType string
}

func (s *fakeStore) GetObject(_ context.Context, _ string) ([]byte, error) {
	if s.getErr != nil {
		return nil, s.getErr
	}

	return s.getData, nil
}

func (s *fakeStore) PutObject(_ context.Context, objectName string, data []byte, contentType string) error {
	s.putCalls = append(s.putCalls, putCall{
		objectName:  objectName,
		data:        data,
		contentType: contentType,
	})

	if s.putErr != nil {
		return s.putErr
	}

	return nil
}

type fakeGenerator struct {
	variants         []mediaimage.GeneratedVariant
	err              error
	receivedOriginal []byte
	receivedSpecs    []mediaimage.VariantSpec
}

func (g *fakeGenerator) GenerateVariants(_ context.Context, original []byte, specs []mediaimage.VariantSpec) ([]mediaimage.GeneratedVariant, error) {
	g.receivedOriginal = append([]byte(nil), original...)
	g.receivedSpecs = append([]mediaimage.VariantSpec(nil), specs...)

	if g.err != nil {
		return nil, g.err
	}

	return g.variants, nil
}
