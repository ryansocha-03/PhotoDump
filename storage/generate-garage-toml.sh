export $(cat ../.env | xargs)
envsubst < garage.toml.tpl > garage.toml