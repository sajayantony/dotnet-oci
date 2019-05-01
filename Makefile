all : build uninstall install

build :
	cd dotnet-start && \
	dotnet pack
	cd src/Dotnet.Publish.Oci && \
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
	cd samples/helloworld/bin/Debug/netcoreapp2.2/publish && \
	oras push $$REGISTRY/hello-world:dotnet  . \
 	manifest.json 
