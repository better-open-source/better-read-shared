#!/usr/bin/env bash

docker build . -t better-read-shared-build-image -f Dockerfile --build-arg CI_BUILDID=$1 --build-arg CI_PRERELEASE=$2
docker create --name better-read-shared-build-container better-read-shared-build-image
docker cp better-read-shared-build-container:./app/out ./out
docker rm -fv better-read-shared-build-container
