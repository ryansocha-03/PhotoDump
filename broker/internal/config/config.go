package config

import (
	"errors"
	"fmt"
	"strings"

	"github.com/spf13/viper"
)

type Config struct {
	ContentStoreURL    string `mapstructure:"content_store_domain"`
	ContentStoreKey    string `mapstructure:"content_store_key"`
	ContentStoreSecret string `mapstructure:"content_store_secret"`
	ContentStoreBucket string `mapstructure:"content_store_bucket"`
	ContentStoreSecure bool   `mapstructure:"content_store_secure"`
	QueueConnection    string `mapstructure:"queue_connection_string"`
	QueueName          string `mapstructure:"queue_name"`
	MaxWorkers         int    `mapstructure:"max_workers"`
	MaxMessages        int    `mapstructure:"max_messages"`
	AppApiBaseUrl      string `mapstructure:"app_api_base_url"`
	AppApiToken        string `mapstructure:"app_api_token"`
	TokenHeaderName    string `mapstructure:"token_header_name"`
}

func Load() (*Config, error) {
	v := viper.New()
	v.SetEnvPrefix("pd")

	for _, key := range requiredKeys() {
		if err := v.BindEnv(key); err != nil {
			return nil, fmt.Errorf("unable to bind environment variable %s: %w", envName(key), err)
		}
	}

	return loadFromViper(v)
}

func loadFromViper(v *viper.Viper) (*Config, error) {
	missing := missingKeys(v, requiredKeys())
	if len(missing) > 0 {
		return nil, fmt.Errorf("missing required environment variables: %s", strings.Join(missing, ", "))
	}

	cfg := &Config{}
	if err := v.Unmarshal(cfg); err != nil {
		return nil, err
	}

	if cfg.ContentStoreURL == "" || cfg.ContentStoreKey == "" || cfg.ContentStoreSecret == "" || cfg.ContentStoreBucket == "" {
		return nil, errors.New("unable to read content store configuration")
	}

	if cfg.QueueConnection == "" || cfg.QueueName == "" {
		return nil, errors.New("unable to read queue configuration")
	}

	if cfg.MaxWorkers <= 0 {
		return nil, errors.New("max_workers must be greater than zero")
	}

	if cfg.MaxMessages <= 0 {
		return nil, errors.New("max_messages must be greater than zero")
	}

	return cfg, nil
}

func requiredKeys() []string {
	return []string{
		"content_store_domain",
		"content_store_key",
		"content_store_secret",
		"content_store_bucket",
		"content_store_secure",
		"queue_connection_string",
		"queue_name",
		"max_workers",
		"max_messages",
		"app_api_base_url",
		"app_api_token",
		"token_header_name",
	}
}

func missingKeys(v *viper.Viper, keys []string) []string {
	missing := make([]string, 0)

	for _, key := range keys {
		if !v.IsSet(key) {
			missing = append(missing, envName(key))
		}
	}

	return missing
}

func envName(key string) string {
	return "PD_" + strings.ToUpper(key)
}
