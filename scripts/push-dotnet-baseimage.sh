#!/bin/sh

set -e

if [ -z "$REGISTRY" ] 
    then
        echo ERROR: Environment variable REGISTRY not defined
        exit 1
fi

if [ -z "$DOCKER_USERNAME" ]
  then 
        echo ERROR: DOCKER_USERNAME not defined. 
        exit 1
fi

if [ -z "$DOCKER_PASSWORD" ]
  then 
        echo ERROR: DOCKER_PASSWORD not defined. 
        exit 1
fi


BASE_IMAGE="mcr.microsoft.com/dotnet/core/runtime:3.0.0-preview4-bionic"
REPO="dotnet"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd $DIR
cd ../base-image
OCI_BASE=$REGISTRY/dotnet-oci-preview-3:base
docker build -t $OCI_BASE -f preview-3.Dockerfile  .

# Prepare base image 
 docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD $REGISTRY
docker push $OCI_BASE 