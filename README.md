# Dotnet app from OCI registry

The goal here is to enable dotnet app distribution using an OCI registry.
Dotnet apps can be run directly using the dotnet CLI as follow.

```
dotnet start myregistry.azurecr.io/dotnet-oci-preview-3:app
```

[![asciicast](https://asciinema.org/a/ok2FAeq8vCizvreF9L1KvlQsX.svg)](https://asciinema.org/a/ok2FAeq8vCizvreF9L1KvlQsX?speed=2&autoplay=1)

## Building the project

Build and install the tool.

```bash
## this builds the tool and installs it into the global tools 
make all
```

## Pushing the sample application
This uses oras - https://github.com/deislabs/oras/

1. Install dotnet preview 3
2. Ensure you have oras binary on your path

```bash
# Get the access keys from the registry
export REGISTRY_NAME=<NAME>
az configure --defaults acr=$REGISTRY_NAME
az acr login -n $REGISTRY_NAME
export DOCKER_USERNAME=$(az acr credential show --query username -o tsv)
export DOCKER_PASSWORD=$(az acr credential show --query passwords[0].value  -o tsv)
export REGISTRY=$(az acr show --query loginServer -o tsv)
cd samples/hw-preview-3/
dotnet publish
cd $(git rev-parse --show-toplevel)
./scripts/push-image.sh -p ./samples/hw-preview-3/bin/Debug/netcoreapp3.0/publish/
```

## App cache 

The applications are downloaded to a cache and we can make this smarter by indexing by a sha256 digest if needed. 

```
$ tree ~/.dotnet-oras-app/
/Users/sajay/.dotnet-oras-app/
└── sajaywus2.azurecr.io.dotnet-oci-preview-3.app
    ├── hw-preview-3
    ├── hw-preview-3.deps.json
    ├── hw-preview-3.dll
    ├── hw-preview-3.pdb
    └── hw-preview-3.runtimeconfig.json
```