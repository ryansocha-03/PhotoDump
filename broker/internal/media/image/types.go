package image

import (
	"errors"
	"fmt"
)

type FitMode string

const (
	FitCover   FitMode = "cover"
	FitContain FitMode = "contain"
)

type Format string

const (
	FormatJPEG Format = "jpeg"
	FormatWebP Format = "webp"
	FormatAVIF Format = "avif"
)

type Color struct {
	R uint8
	G uint8
	B uint8
}

type VariantSpec struct {
	Name          string
	Width         int
	Height        int
	Fit           FitMode
	Format        Format
	Quality       int
	StripMetadata bool
	Background    Color
}

type GeneratedVariant struct {
	Name        string
	Bytes       []byte
	ContentType string
	Extension   string
	Width       int
	Height      int
}

type InvalidImageError struct {
	Err error
}

func (e *InvalidImageError) Error() string {
	return e.Err.Error()
}

func (e *InvalidImageError) Unwrap() error {
	return e.Err
}

func IsInvalidImage(err error) bool {
	var target *InvalidImageError
	return errors.As(err, &target)
}

func ValidateVariantSpecs(specs []VariantSpec) error {
	if len(specs) == 0 {
		return errors.New("at least one variant spec is required")
	}

	for _, spec := range specs {
		if spec.Name == "" {
			return errors.New("variant name is required")
		}

		if spec.Width <= 0 || spec.Height <= 0 {
			return fmt.Errorf("variant %q must have positive width and height", spec.Name)
		}

		if spec.Fit != FitCover && spec.Fit != FitContain {
			return fmt.Errorf("variant %q has unsupported fit mode %q", spec.Name, spec.Fit)
		}

		if spec.Format != FormatJPEG && spec.Format != FormatWebP && spec.Format != FormatAVIF {
			return fmt.Errorf("variant %q has unsupported format %q", spec.Name, spec.Format)
		}

		if spec.Quality <= 0 || spec.Quality > 100 {
			return fmt.Errorf("variant %q must use quality in range 1-100", spec.Name)
		}
	}

	return nil
}
