package pipeline

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"path"
	"strings"

	"photodump/workers/internal/jobs"
	mediaimage "photodump/workers/internal/media/image"
	"photodump/workers/internal/storage"
)

type Processor interface {
	ProcessMessage(ctx context.Context, body []byte) *ProcessingError
}

type ObjectStore interface {
	GetObject(ctx context.Context, objectName string) ([]byte, error)
	PutObject(ctx context.Context, objectName string, data []byte, contentType string) error
}

type ImageVariantGenerator interface {
	GenerateVariants(ctx context.Context, original []byte, specs []mediaimage.VariantSpec) ([]mediaimage.GeneratedVariant, error)
}

type ImageVariantPipeline struct {
	store     ObjectStore
	generator ImageVariantGenerator
	variants  []mediaimage.VariantSpec
}

func NewImageVariantPipeline(store ObjectStore, generator ImageVariantGenerator, variants []mediaimage.VariantSpec) (*ImageVariantPipeline, error) {
	if err := mediaimage.ValidateVariantSpecs(variants); err != nil {
		return nil, err
	}

	copiedVariants := append([]mediaimage.VariantSpec(nil), variants...)

	return &ImageVariantPipeline{
		store:     store,
		generator: generator,
		variants:  copiedVariants,
	}, nil
}

func (p *ImageVariantPipeline) ProcessMessage(ctx context.Context, body []byte) *ProcessingError {
	var job jobs.GenerateImageVariantsJob
	if err := json.Unmarshal(body, &job); err != nil {
		return &ProcessingError{
			Message: fmt.Sprintf("unable to parse image variants job: %v", err),
			Requeue: false,
		}
	}

	originalObject, err := p.store.GetObject(ctx, job.ObjectName)
	if err != nil {
		if storage.IsObjectNotFound(err) {
			return &ProcessingError{
				Message: fmt.Sprintf("original object %q was not found in object storage", job.ObjectName),
				Requeue: false,
			}
		}

		return &ProcessingError{
			Message: fmt.Sprintf("unable to fetch original object %q: %v", job.ObjectName, err),
			Requeue: true,
		}
	}

	generatedVariants, err := p.generator.GenerateVariants(ctx, originalObject, p.variants)
	if err != nil {
		if mediaimage.IsInvalidImage(err) {
			return &ProcessingError{
				Message: fmt.Sprintf("unable to decode original image: %v", err),
				Requeue: false,
			}
		}

		return &ProcessingError{
			Message: fmt.Sprintf("unable to generate image variants for %q: %v", job.ObjectName, err),
			Requeue: true,
		}
	}

	for _, variant := range generatedVariants {
		variantObjectName := VariantObjectName(job.ObjectName, variant)
		if err = p.store.PutObject(ctx, variantObjectName, variant.Bytes, variant.ContentType); err != nil {
			return &ProcessingError{
				Message: fmt.Sprintf("unable to upload variant %q to %q: %v", variant.Name, variantObjectName, err),
				Requeue: true,
			}
		}

		log.Printf("Generated image variant %q for media %d at %q (%d bytes)", variant.Name, job.MediaID, variantObjectName, len(variant.Bytes))
	}

	return nil
}

func VariantObjectName(originalObjectName string, variant mediaimage.GeneratedVariant) string {
	dir := path.Dir(originalObjectName)
	baseName := path.Base(originalObjectName)
	extension := path.Ext(baseName)
	rootName := strings.TrimSuffix(baseName, extension)

	return path.Join(dir, fmt.Sprintf("%s_%s.%s", rootName, variant.Name, variant.Extension))
}
