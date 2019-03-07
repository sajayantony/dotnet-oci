# Dotnet app from OCI registry

The goal here is to enable dotnet app distribution using an OCI registry. 
Dotnet apps can be run directly using the dotnet CLI as follow.

```
dotnet start myregistry.azurecr.io/dotnet:app
```

[![asciicast](https://asciinema.org/a/ok2FAeq8vCizvreF9L1KvlQsX.svg)](https://asciinema.org/a/ok2FAeq8vCizvreF9L1KvlQsX?speed=2&autoplay=1)

## Building the project

Build and install the tool.

```bash
## this build the tool and install it into the global tools 
make all
```

## Pushing the sample application
This uses oras - https://github.com/deislabs/oras/

```bash
# Get the access keys from the registry
export DOCKER_USERNAME=something
export DOCKER_PASSWORD=password
export REGISTRY=sajayeus.azurecr.io
cd samples/helloworld

#ORAS needs  PR to push a directory.
# But for now just push the needed files.

oras push $REGISTRY/hello-world:dotnet  \
    ./bin/Debug/netcoreapp2.2/publish/helloworld.dll \
    ./bin/Debug/netcoreapp2.2/publish/helloworld.runtimeconfig.json  
    manifest.json \
    -u $DOCKER_USERNAME \
    -p  $DOCKER_PASSWORD
```

## App cache 

The applications are downloaded to a cache and we can mak this smarter by indexing by a sha256 digest if needed. 

```
❯ tree ~/.dotnet-oras-app/
/Users/sajay/.dotnet-oras-app/
└── sajayeus.azurecr.io.hello-world.dotnet
    ├── bin
    │   └── Debug
    │       ├── helloworld.1.0.0.nupkg
    │       └── netcoreapp2.2
    │           └── publish
    │               ├── helloworld.dll
    │               └── helloworld.runtimeconfig.json
    ├── helloworld.dll
    └── manifest.json
```

So the entry point in the manifest is defined as the full path to the dll. 

```json
{
	"entrypoint": "./bin/Debug/netcoreapp2.2/publish/helloworld.dll"
}
```