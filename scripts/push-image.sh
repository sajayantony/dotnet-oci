#!/bin/bash

set -ex

if [ -z "$REGISTRY" ] 
    then
        echo ERROR: Environment variable REGISTRY not defined
        exit 1
fi

if [ -z "$DOCKER_USERNAME"]
  then 
        echo ERROR: DOCKER_USERNAME not defined. 
        exit 1
fi

if [ -z "$DOCKER_PASSWORD"]
  then 
        echo ERROR: DOCKER_PASSWORD not defined. 
        exit 1
fi

while getopts "p:" OPTION; do
  case $OPTION in
    p )
      APP_DIR=$OPTARG
  esac
done
shift "$(($OPTIND -1))"

repo="dotnet-oci-preview-3"
DOCKER_IMAGE_TAG="hello-world-app"
CURL_USER_ARGS="--user $DOCKER_USERNAME:$DOCKER_PASSWORD"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

cd $APP_DIR
TEMP_DIR=".temp"

if [ -d "$TEMP_DIR" ]
 then 
     rm -rf $TEMP_DIR
fi

# Prepare app: no docker required

#tar -cf app.tar --owner=0 --group=0 .
# OSX doesn't have sha256sum so alias it i
# stat is -f instead of -c for OSX

tar -cf app.tar . 
mkdir $TEMP_DIR
mv app.tar  $TEMP_DIR
cd $TEMP_DIR

app_diff_id="sha256:$(shasum -a 256  app.tar | cut -d " " -f1)"
gzip app.tar
oras push $REGISTRY/$repo:app app.tar.gz
app_size=$(stat -f%p app.tar.gz)
app_digest="sha256:$(shasum -a 256  app.tar.gz | cut -d " " -f1)"

##
# Demo Main Part: no docker required
##

# Get base manifest and config
curl $CURL_USER_ARGS  -sH "Accept: application/vnd.docker.distribution.manifest.v2+json" \
        "https://$REGISTRY/v2/$repo/manifests/base" > base_manifest.json
config_digest=$(cat base_manifest.json | jq -r .config.digest)
curl $CURL_USER_ARGS -s -L "https://$REGISTRY/v2/$repo/blobs/$config_digest" > base_config.json

# Modify config file for the new layer
cat base_config.json | jq -c ".rootfs.diff_ids += [\"$app_diff_id\"]" > app_config.json
app_config_size=$(stat -f "%p" app_config.json)
app_config_digest="sha256:$(shasum -a 256 app_config.json | cut -d " " -f1)"

# Modify manifest file for the new layer
cat base_manifest.json |
    jq ".config.size = $app_config_size" |
    jq ".config.digest = \"$app_config_digest\"" |
    jq ".layers += [{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": $app_size,\"digest\":\"$app_digest\"}]" \
    > app_manifest.json

# Upload config file
upload_url=$(curl $CURL_USER_ARGS -sIXPOST "https://$REGISTRY/v2/$repo/blobs/uploads/" | grep "Location: " | cut -d " " -f2 | tr -d "\r")
curl $CURL_USER_ARGS  --upload-file app_config.json "https://$REGISTRY$upload_url&digest=$app_config_digest"

# Push image (push manifest)
curl $CURL_USER_ARGS -XPUT -d @app_manifest.json \
        -H "Content-Type: application/vnd.docker.distribution.manifest.v2+json" \
        "https://$REGISTRY/v2/$repo/manifests/$DOCKER_IMAGE_TAG"

#clean up 
rm -rf $TEMP_DIR

##
# End Of Demo Main Part
##

# Pull and run the merged image
docker pull $REGISTRY/$repo:$DOCKER_IMAGE_TAG 
docker run --rm -it $REGISTRY/$repo:$DOCKER_IMAGE_TAG 