package main

import (
	"strings"
	"testing"

	"github.com/spf13/viper"
)

func TestLoadConfigFailsWhenRequiredEnvMissing(t *testing.T) {
	cfg, err := loadConfig(newTestConfigViper(map[string]any{
		"max_workers": nil,
	}))
	if err == nil {
		t.Fatalf("expected missing env error, got config: %+v", cfg)
	}

	if !strings.Contains(err.Error(), "PD_MAX_WORKERS") {
		t.Fatalf("expected missing env error to mention PD_MAX_WORKERS, got %q", err.Error())
	}
}

func TestLoadConfigFailsWhenConcurrencySettingsAreNotPositive(t *testing.T) {
	testCases := []struct {
		name      string
		overrides map[string]any
		wantErr   string
	}{
		{
			name: "zero max workers",
			overrides: map[string]any{
				"max_workers": 0,
			},
			wantErr: "max_workers must be greater than zero",
		},
		{
			name: "zero max messages",
			overrides: map[string]any{
				"max_messages": 0,
			},
			wantErr: "max_messages must be greater than zero",
		},
	}

	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			cfg, err := loadConfig(newTestConfigViper(tc.overrides))
			if err == nil {
				t.Fatalf("expected concurrency validation error, got config: %+v", cfg)
			}

			if err.Error() != tc.wantErr {
				t.Fatalf("expected %q, got %q", tc.wantErr, err.Error())
			}
		})
	}
}

func TestLoadConfigReadsExplicitSettings(t *testing.T) {
	cfg, err := loadConfig(newTestConfigViper(map[string]any{
		"content_store_secure": true,
		"max_workers":          4,
		"max_messages":         8,
	}))
	if err != nil {
		t.Fatalf("expected config to load successfully, got %v", err)
	}

	if !cfg.ContentStoreSecure {
		t.Fatal("expected content store secure to be true")
	}

	if cfg.MaxWorkers != 4 {
		t.Fatalf("expected max workers to be 4, got %d", cfg.MaxWorkers)
	}

	if cfg.MaxMessages != 8 {
		t.Fatalf("expected max messages to be 8, got %d", cfg.MaxMessages)
	}
}

func newTestConfigViper(overrides map[string]any) *viper.Viper {
	v := viper.New()

	values := map[string]any{
		"content_store_domain":    "localhost:9000",
		"content_store_key":       "access-key",
		"content_store_secret":    "secret-key",
		"content_store_bucket":    "photos",
		"content_store_secure":    false,
		"queue_connection_string": "amqp://guest:guest@localhost:5672/",
		"queue_name":              "thumbnail-jobs",
		"max_workers":             2,
		"max_messages":            2,
	}

	for key, value := range overrides {
		if value == nil {
			delete(values, key)
			continue
		}

		values[key] = value
	}

	for key, value := range values {
		v.Set(key, value)
	}

	return v
}
