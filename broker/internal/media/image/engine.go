package image

import (
	"sync"

	"github.com/davidbyttow/govips/v2/vips"
)

var (
	startupOnce sync.Once
	startupErr  error
)

func Initialize() error {
	startupOnce.Do(func() {
		vips.LoggingSettings(nil, vips.LogLevelWarning)
		startupErr = vips.Startup(&vips.Config{ConcurrencyLevel: 1})
	})

	return startupErr
}
