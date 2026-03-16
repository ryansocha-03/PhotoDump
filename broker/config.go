package main

import (
	"errors"
	"fmt"
	"strings"

	"github.com/spf13/viper"
)

type Config struct {
	ContentStoreUrl    string `mapstructure:"content_store_domain"`
	ContentStoreKey    string `mapstructure:"content_store_key"`
	ContentStoreSecret string `mapstructure:"content_store_secret"`
	ContentStoreBucket string `mapstructure:"content_store_bucket"`
	ContentStoreSecure bool   `mapstructure:"content_store_secure"`
	QueueConnection    string `mapstructure:"queue_connection_string"`
	QueueName          string `mapstructure:"queue_name"`
	MaxWorkers         int    `mapstructure:"max_workers"`
	MaxMessages        int    `mapstructure:"max_messages"`
}

func LoadConfig() (c *Config, err error) {
	v := viper.New()
	v.SetEnvPrefix("pd")

	for _, key := range requiredConfigKeys() {
		if err = v.BindEnv(key); err != nil {
			return nil, fmt.Errorf("unable to bind environment variable %s: %w", envName(key), err)
		}
	}

	return loadConfig(v)
}

func loadConfig(v *viper.Viper) (*Config, error) {
	missing := missingEnvKeys(v, requiredConfigKeys())
	if len(missing) > 0 {
		return nil, fmt.Errorf("missing required environment variables: %s", strings.Join(missing, ", "))
	}

	c := &Config{}
	if err := v.Unmarshal(c); err != nil {
		return nil, err
	}

	if c.ContentStoreUrl == "" || c.ContentStoreKey == "" || c.ContentStoreSecret == "" || c.ContentStoreBucket == "" {
		return nil, errors.New("unable to read content store configuration")
	}

	if c.QueueConnection == "" || c.QueueName == "" {
		return nil, errors.New("unable to read queue configuration")
	}

	if c.MaxWorkers <= 0 {
		return nil, errors.New("max_workers must be greater than zero")
	}

	if c.MaxMessages <= 0 {
		return nil, errors.New("max_messages must be greater than zero")
	}

	return c, nil
}

func requiredConfigKeys() []string {
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
	}
}

func missingEnvKeys(v *viper.Viper, keys []string) []string {
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
