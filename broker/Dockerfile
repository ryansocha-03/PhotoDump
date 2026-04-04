FROM golang:1.25-bookworm AS build
WORKDIR /src

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    pkg-config \
    libvips-dev \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY broker/go.mod broker/go.sum ./broker/
WORKDIR /src/broker
RUN go mod download

COPY broker/ ./

RUN CGO_ENABLED=1 GOOS=linux go build -o /out/broker ./cmd/broker

FROM debian:bookworm-slim AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    libvips42 \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /out/broker /app/broker

ENTRYPOINT ["/app/broker"]
