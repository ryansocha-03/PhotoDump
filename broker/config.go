package main

import (
	"errors"

	"github.com/spf13/viper"
)

type Config struct {
	ContentStoreUrl    string `mapstructure:"content_store_domain"`
	ContentStoreKey    string `mapstructure:"content_store_key"`
	ContentStoreSecret string `mapstructure:"content_store_secret"`
	ContentStoreBucket string `mapstructure:"content_store_bucket"`
}

func LoadConfig() (c Config, err error) {
	viper.SetEnvPrefix("pd")

	viper.BindEnv("content_store_domain")
	viper.BindEnv("content_store_key")
	viper.BindEnv("content_store_secret")
	viper.BindEnv("content_store_bucket")

	err = viper.Unmarshal(&c)
	if err != nil {
		return
	}

	if c.ContentStoreUrl == "" || c.ContentStoreKey == "" || c.ContentStoreSecret == "" || c.ContentStoreBucket == "" {
		err = errors.New("Unable to read content store configuration.")
	}

	return
}
