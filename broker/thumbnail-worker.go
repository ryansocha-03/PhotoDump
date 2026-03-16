package main

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"sync"

	"github.com/davidbyttow/govips/v2/vips"
	"github.com/rabbitmq/amqp091-go"
)

type ThumbnailGenerationMessage struct {
	ObjectName string `json:"ObjectName"`
	MediaId    int    `json:"MediaId"`
}

const (
	galleryThumbnailSize    = 320
	galleryThumbnailQuality = 72
)

var (
	vipsStartupOnce sync.Once
	vipsStartupErr  error
)

func ProcessMessage(msg *amqp091.Delivery, objstr *ObjectStorage) (err *MessageError) {
	var msgData ThumbnailGenerationMessage
	umErr := json.Unmarshal(msg.Body, &msgData)
	if umErr != nil {
		err = &MessageError{Message: umErr.Error(), Requeue: false}
		return
	}

	originalObject, fetchErr := objstr.GetOriginalObject(context.Background(), msgData.ObjectName)
	if fetchErr != nil {
		return fetchErr
	}

	thumbnailBytes, thumbErr := GenerateGalleryThumbnail(originalObject)
	if thumbErr != nil {
		return thumbErr
	}

	log.Printf(
		"Downloaded original image for media %v from %v (%d bytes) and generated gallery thumbnail (%d bytes)",
		msgData.MediaId,
		msgData.ObjectName,
		len(originalObject),
		len(thumbnailBytes),
	)

	return
}

func InitializeImageProcessing() error {
	vipsStartupOnce.Do(func() {
		vips.LoggingSettings(nil, vips.LogLevelWarning)
		vipsStartupErr = vips.Startup(&vips.Config{ConcurrencyLevel: 1})
	})

	return vipsStartupErr
}

func GenerateGalleryThumbnail(originalObject []byte) ([]byte, *MessageError) {
	if err := InitializeImageProcessing(); err != nil {
		return nil, &MessageError{
			Message: fmt.Sprintf("unable to initialize image processing: %v", err),
			Requeue: true,
		}
	}

	thumbnail, err := vips.NewThumbnailFromBuffer(
		originalObject,
		galleryThumbnailSize,
		galleryThumbnailSize,
		vips.InterestingCentre,
	)
	if err != nil {
		return nil, &MessageError{
			Message: fmt.Sprintf("unable to decode original image: %v", err),
			Requeue: false,
		}
	}
	defer thumbnail.Close()

	if thumbnail.HasAlpha() {
		if err = thumbnail.Flatten(&vips.Color{R: 255, G: 255, B: 255}); err != nil {
			return nil, &MessageError{
				Message: fmt.Sprintf("unable to flatten gallery thumbnail: %v", err),
				Requeue: true,
			}
		}
	}

	exportParams := vips.NewJpegExportParams()
	exportParams.StripMetadata = true
	exportParams.Quality = galleryThumbnailQuality
	exportParams.OptimizeCoding = true
	exportParams.Interlace = true
	exportParams.TrellisQuant = true
	exportParams.OptimizeScans = true
	exportParams.OvershootDeringing = true
	exportParams.SubsampleMode = vips.VipsForeignSubsampleAuto

	output, _, err := thumbnail.ExportJpeg(exportParams)
	if err != nil {
		return nil, &MessageError{
			Message: fmt.Sprintf("unable to encode gallery thumbnail: %v", err),
			Requeue: true,
		}
	}

	return output, nil
}
