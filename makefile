.NOTPARALLEL:
SHELL := /bin/bash
OUTPUT_DIR := .output
SOLUTION_FILE := src/Sherlock.sln
APP_PROJECT_FILE := src/Sherlock.App/Sherlock.App.csproj
TEST_PROJECT_FILE := src/Sherlock.App.Tests/Sherlock.App.Tests.csproj

build:
	@docker-compose run --rm sdk dotnet build \
		-c Release \
        -o "${OUTPUT_DIR}/dotnet" \
        -v normal $(APP_PROJECT_FILE)
.PHONIX: build

test:
	@docker-compose run --rm sdk dotnet build \
    		-c Release \
            -o "${OUTPUT_DIR}/tests" \
            -v normal $(TEST_PROJECT_FILE)
	@docker-compose run --rm sdk dotnet test -v:normal "${OUTPUT_DIR}/tests/Sherlock.App.Tests.dll"
.PHONIX: test

run:
	@docker-compose run --rm runtime
.PHONIX: run