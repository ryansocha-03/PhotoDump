package image

import (
	"bytes"
	"image"
	"image/color"
	"image/jpeg"
	"image/png"
	"strings"
	"testing"
)

func TestDefaultVariantSpecsAreValid(t *testing.T) {
	if err := ValidateVariantSpecs(DefaultVariantSpecs()); err != nil {
		t.Fatalf("expected default variant specs to be valid, got %v", err)
	}
}

func TestGenerateVariantsCreatesConfiguredOutput(t *testing.T) {
	processor, err := NewProcessor()
	if err != nil {
		t.Fatalf("expected processor initialization to succeed, got %v", err)
	}

	source := image.NewRGBA(image.Rect(0, 0, 1200, 800))
	fill(source, image.Rect(0, 0, 1200, 800), color.RGBA{R: 240, G: 240, B: 240, A: 255})

	variants, err := processor.GenerateVariants(t.Context(), mustEncodePNG(t, source), DefaultVariantSpecs())
	if err != nil {
		t.Fatalf("expected variant generation to succeed, got %v", err)
	}

	if len(variants) != 1 {
		t.Fatalf("expected one generated variant, got %d", len(variants))
	}

	variant := variants[0]
	if variant.Name != "gallery" {
		t.Fatalf("expected gallery variant name, got %q", variant.Name)
	}

	if variant.ContentType != "image/jpeg" {
		t.Fatalf("expected image/jpeg content type, got %q", variant.ContentType)
	}

	if variant.Extension != "jpg" {
		t.Fatalf("expected jpg extension, got %q", variant.Extension)
	}

	thumbImage := mustDecodeJPEG(t, variant.Bytes)
	if thumbImage.Bounds().Dx() != DefaultVariantSpecs()[0].Width {
		t.Fatalf("expected width %d, got %d", DefaultVariantSpecs()[0].Width, thumbImage.Bounds().Dx())
	}

	if thumbImage.Bounds().Dy() != DefaultVariantSpecs()[0].Height {
		t.Fatalf("expected height %d, got %d", DefaultVariantSpecs()[0].Height, thumbImage.Bounds().Dy())
	}
}

func TestGenerateVariantsCenterCropsLandscapeImages(t *testing.T) {
	processor, err := NewProcessor()
	if err != nil {
		t.Fatalf("expected processor initialization to succeed, got %v", err)
	}

	source := image.NewRGBA(image.Rect(0, 0, 400, 200))
	fill(source, image.Rect(0, 0, 100, 200), color.RGBA{R: 255, A: 255})
	fill(source, image.Rect(100, 0, 300, 200), color.RGBA{G: 255, A: 255})
	fill(source, image.Rect(300, 0, 400, 200), color.RGBA{B: 255, A: 255})

	variants, err := processor.GenerateVariants(t.Context(), mustEncodePNG(t, source), DefaultVariantSpecs())
	if err != nil {
		t.Fatalf("expected variant generation to succeed, got %v", err)
	}

	thumbImage := mustDecodeJPEG(t, variants[0].Bytes)
	centerColor := color.RGBAModel.Convert(thumbImage.At(DefaultVariantSpecs()[0].Width/2, DefaultVariantSpecs()[0].Height/2)).(color.RGBA)
	if centerColor.G < 170 {
		t.Fatalf("expected center crop to preserve the middle region, got %#v", centerColor)
	}

	leftColor := color.RGBAModel.Convert(thumbImage.At(0, DefaultVariantSpecs()[0].Height/2)).(color.RGBA)
	if leftColor.R > 120 {
		t.Fatalf("expected left edge of thumbnail to exclude the far-left red band, got %#v", leftColor)
	}

	rightColor := color.RGBAModel.Convert(thumbImage.At(DefaultVariantSpecs()[0].Width-1, DefaultVariantSpecs()[0].Height/2)).(color.RGBA)
	if rightColor.B > 120 {
		t.Fatalf("expected right edge of thumbnail to exclude the far-right blue band, got %#v", rightColor)
	}
}

func TestGenerateVariantsFlattensAlphaForJPEG(t *testing.T) {
	processor, err := NewProcessor()
	if err != nil {
		t.Fatalf("expected processor initialization to succeed, got %v", err)
	}

	source := image.NewNRGBA(image.Rect(0, 0, 200, 200))
	for y := 60; y < 140; y++ {
		for x := 60; x < 140; x++ {
			source.Set(x, y, color.NRGBA{R: 255, G: 0, B: 0, A: 255})
		}
	}

	variants, err := processor.GenerateVariants(t.Context(), mustEncodePNG(t, source), DefaultVariantSpecs())
	if err != nil {
		t.Fatalf("expected variant generation to succeed, got %v", err)
	}

	thumbImage := mustDecodeJPEG(t, variants[0].Bytes)
	corner := color.RGBAModel.Convert(thumbImage.At(0, 0)).(color.RGBA)
	if corner.R < 200 || corner.G < 200 || corner.B < 200 {
		t.Fatalf("expected transparent background to flatten to white, got %#v", corner)
	}
}

func TestGenerateVariantsRejectsInvalidImages(t *testing.T) {
	processor, err := NewProcessor()
	if err != nil {
		t.Fatalf("expected processor initialization to succeed, got %v", err)
	}

	variants, err := processor.GenerateVariants(t.Context(), []byte("not-an-image"), DefaultVariantSpecs())
	if err == nil {
		t.Fatalf("expected decode error, got variants %+v", variants)
	}

	if !IsInvalidImage(err) {
		t.Fatalf("expected invalid image error, got %v", err)
	}

	if !strings.Contains(err.Error(), "error") && !strings.Contains(strings.ToLower(err.Error()), "image") {
		t.Fatalf("expected decode error message, got %q", err.Error())
	}
}

func fill(img *image.RGBA, rect image.Rectangle, c color.RGBA) {
	for y := rect.Min.Y; y < rect.Max.Y; y++ {
		for x := rect.Min.X; x < rect.Max.X; x++ {
			img.SetRGBA(x, y, c)
		}
	}
}

func mustEncodePNG(t *testing.T, img image.Image) []byte {
	t.Helper()

	var output bytes.Buffer
	if err := png.Encode(&output, img); err != nil {
		t.Fatalf("failed to encode PNG: %v", err)
	}

	return output.Bytes()
}

func mustDecodeJPEG(t *testing.T, data []byte) image.Image {
	t.Helper()

	img, err := jpeg.Decode(bytes.NewReader(data))
	if err != nil {
		t.Fatalf("failed to decode JPEG thumbnail: %v", err)
	}

	return img
}
