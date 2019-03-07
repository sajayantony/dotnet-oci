all : build uninstall install

build :
	cd dotnet-start && \
	dotnet pack

uninstall :
	-dotnet tool uninstall --global dotnet-start

install : 
	cd dotnet-start && \
	dotnet tool install --global --add-source ./nupkg/ dotnet-start

sample :
	cd samples/helloworld && \
	dotnet publish

push :
	cd samples/helloworld && \
	oras push $$REGISTRY/hello-world:dotnet  \
	./bin/Debug/netcoreapp2.2/publish/helloworld.dll \
	./bin/Debug/netcoreapp2.2/publish/helloworld.runtimeconfig.json  \
 	manifest.json \
	-u $$DOCKER_USERNAME \
	-p  $$DOCKER_PASSWORD