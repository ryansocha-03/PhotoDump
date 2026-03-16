package main

import (
	"bytes"
	"image"
	"image/color"
	"image/jpeg"
	"image/png"
	"strings"
	"testing"
)

func TestGenerateGalleryThumbnailCreatesSquareThumbnail(t *testing.T) {
	source := image.NewRGBA(image.Rect(0, 0, 1200, 800))
	fill(source, image.Rect(0, 0, 1200, 800), color.RGBA{R: 240, G: 240, B: 240, A: 255})

	originalBytes := mustEncodePNG(t, source)
	thumbnailBytes, err := GenerateGalleryThumbnail(originalBytes)
	if err != nil {
		t.Fatalf("expected thumbnail generation to succeed, got %v", err)
	}

	if !bytes.HasPrefix(thumbnailBytes, []byte{0xff, 0xd8, 0xff}) {
		t.Fatalf("expected JPEG thumbnail output, got leading bytes %v", thumbnailBytes[:3])
	}

	thumbImage := mustDecodeJPEG(t, thumbnailBytes)
	if thumbImage.Bounds().Dx() != galleryThumbnailSize {
		t.Fatalf("expected width %d, got %d", galleryThumbnailSize, thumbImage.Bounds().Dx())
	}

	if thumbImage.Bounds().Dy() != galleryThumbnailSize {
		t.Fatalf("expected height %d, got %d", galleryThumbnailSize, thumbImage.Bounds().Dy())
	}
}

func TestGenerateGalleryThumbnailCenterCropsLandscapeImages(t *testing.T) {
	source := image.NewRGBA(image.Rect(0, 0, 400, 200))
	fill(source, image.Rect(0, 0, 100, 200), color.RGBA{R: 255, A: 255})
	fill(source, image.Rect(100, 0, 300, 200), color.RGBA{G: 255, A: 255})
	fill(source, image.Rect(300, 0, 400, 200), color.RGBA{B: 255, A: 255})

	originalBytes := mustEncodePNG(t, source)
	thumbnailBytes, err := GenerateGalleryThumbnail(originalBytes)
	if err != nil {
		t.Fatalf("expected thumbnail generation to succeed, got %v", err)
	}

	thumbImage := mustDecodeJPEG(t, thumbnailBytes)
	centerColor := color.RGBAModel.Convert(thumbImage.At(galleryThumbnailSize/2, galleryThumbnailSize/2)).(color.RGBA)
	if centerColor.G < 170 {
		t.Fatalf("expected center crop to preserve the middle region, got %#v", centerColor)
	}

	leftColor := color.RGBAModel.Convert(thumbImage.At(0, galleryThumbnailSize/2)).(color.RGBA)
	if leftColor.R > 120 {
		t.Fatalf("expected left edge of thumbnail to exclude the far-left red band, got %#v", leftColor)
	}

	rightColor := color.RGBAModel.Convert(thumbImage.At(galleryThumbnailSize-1, galleryThumbnailSize/2)).(color.RGBA)
	if rightColor.B > 120 {
		t.Fatalf("expected right edge of thumbnail to exclude the far-right blue band, got %#v", rightColor)
	}
}

func TestGenerateGalleryThumbnailReturnsNonRequeueForInvalidImages(t *testing.T) {
	thumbnailBytes, err := GenerateGalleryThumbnail([]byte("not-an-image"))
	if err == nil {
		t.Fatalf("expected decode error, got thumbnail of %d bytes", len(thumbnailBytes))
	}

	if err.Requeue {
		t.Fatal("expected invalid image bytes to avoid requeue")
	}

	if !strings.Contains(err.Message, "unable to decode original image") {
		t.Fatalf("expected decode error message, got %q", err.Message)
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
