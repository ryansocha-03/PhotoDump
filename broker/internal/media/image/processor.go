package image

import (
	"context"
	"fmt"

	"github.com/davidbyttow/govips/v2/vips"
)

type Processor struct{}

func NewProcessor() (*Processor, error) {
	if err := Initialize(); err != nil {
		return nil, err
	}

	return &Processor{}, nil
}

func (p *Processor) GenerateVariants(ctx context.Context, original []byte, specs []VariantSpec) ([]GeneratedVariant, error) {
	if err := Initialize(); err != nil {
		return nil, err
	}

	if err := ValidateVariantSpecs(specs); err != nil {
		return nil, err
	}

	variants := make([]GeneratedVariant, 0, len(specs))

	for _, spec := range specs {
		select {
		case <-ctx.Done():
			return nil, ctx.Err()
		default:
		}

		variant, err := generateVariant(original, spec)
		if err != nil {
			return nil, err
		}

		variants = append(variants, variant)
	}

	return variants, nil
}

func generateVariant(original []byte, spec VariantSpec) (GeneratedVariant, error) {
	imageRef, err := newThumbnail(original, spec)
	if err != nil {
		return GeneratedVariant{}, &InvalidImageError{Err: err}
	}
	defer imageRef.Close()

	if spec.Format == FormatJPEG && imageRef.HasAlpha() {
		if err = imageRef.Flatten(&vips.Color{
			R: spec.Background.R,
			G: spec.Background.G,
			B: spec.Background.B,
		}); err != nil {
			return GeneratedVariant{}, fmt.Errorf("unable to flatten variant %q: %w", spec.Name, err)
		}
	}

	output, metadata, err := exportVariant(imageRef, spec)
	if err != nil {
		return GeneratedVariant{}, err
	}

	width := spec.Width
	height := spec.Height
	if metadata != nil {
		width = metadata.Width
		height = metadata.Height
	}

	return GeneratedVariant{
		Name:        spec.Name,
		Bytes:       output,
		ContentType: contentType(spec.Format),
		Extension:   extension(spec.Format),
		Width:       width,
		Height:      height,
	}, nil
}

func newThumbnail(original []byte, spec VariantSpec) (*vips.ImageRef, error) {
	switch spec.Fit {
	case FitCover:
		return vips.NewThumbnailFromBuffer(original, spec.Width, spec.Height, vips.InterestingCentre)
	case FitContain:
		return vips.NewThumbnailWithSizeFromBuffer(original, spec.Width, spec.Height, vips.InterestingNone, vips.SizeDown)
	default:
		return nil, fmt.Errorf("unsupported fit mode %q", spec.Fit)
	}
}

func exportVariant(imageRef *vips.ImageRef, spec VariantSpec) ([]byte, *vips.ImageMetadata, error) {
	switch spec.Format {
	case FormatJPEG:
		params := vips.NewJpegExportParams()
		params.StripMetadata = spec.StripMetadata
		params.Quality = spec.Quality
		params.OptimizeCoding = true
		params.Interlace = true
		params.TrellisQuant = true
		params.OptimizeScans = true
		params.OvershootDeringing = true
		params.SubsampleMode = vips.VipsForeignSubsampleAuto
		output, metadata, err := imageRef.ExportJpeg(params)
		if err != nil {
			return nil, nil, fmt.Errorf("unable to encode variant %q as jpeg: %w", spec.Name, err)
		}
		return output, metadata, nil
	case FormatWebP:
		params := vips.NewWebpExportParams()
		params.StripMetadata = spec.StripMetadata
		params.Quality = spec.Quality
		params.ReductionEffort = 6
		params.MinSize = true
		output, metadata, err := imageRef.ExportWebp(params)
		if err != nil {
			return nil, nil, fmt.Errorf("unable to encode variant %q as webp: %w", spec.Name, err)
		}
		return output, metadata, nil
	case FormatAVIF:
		params := vips.NewAvifExportParams()
		params.StripMetadata = spec.StripMetadata
		params.Quality = spec.Quality
		params.Effort = 5
		output, metadata, err := imageRef.ExportAvif(params)
		if err != nil {
			return nil, nil, fmt.Errorf("unable to encode variant %q as avif: %w", spec.Name, err)
		}
		return output, metadata, nil
	default:
		return nil, nil, fmt.Errorf("unsupported output format %q", spec.Format)
	}
}

func contentType(format Format) string {
	switch format {
	case FormatJPEG:
		return "image/jpeg"
	case FormatWebP:
		return "image/webp"
	case FormatAVIF:
		return "image/avif"
	default:
		return "application/octet-stream"
	}
}

func extension(format Format) string {
	switch format {
	case FormatJPEG:
		return "jpg"
	case FormatWebP:
		return "webp"
	case FormatAVIF:
		return "avif"
	default:
		return "bin"
	}
}
