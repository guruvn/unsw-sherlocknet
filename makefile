.NOTPARALLEL:
SHELL := /bin/bash
OUTPUT_DIR := .output
SOLUTION_FILE := src/Sherlock.sln
APP_PROJECT_FILE := src/Sherlock.App/Sherlock.App.csproj

build:
	docker-compose run --rm sdk dotnet build \
		-c Release \
        -o "${OUTPUT_DIR}" \
        -v normal $(APP_PROJECT_FILE)
.PHONIX: build

run:
	docker-compose run --rm runtime
.PHONIX: run