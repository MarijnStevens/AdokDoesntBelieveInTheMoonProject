#!/bin/sh
set -e

## Linux build
OS="linux-x64"
BUILDDIR="build/$OS"
BINARY="./${BUILDDIR}/Myriad"
_BE_FOR_STRIP=0
_BE_AFTER_STRIP=0


clean() {
    rm -r build/$1 2> /dev/null
}

publish() {
    dotnet publish -c Release -o $BUILDDIR -r $1 --self-contained true \
-p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
-p:PublishTrimmed=true -p:TrimMode=link
}

publish $OS &&\
strip $BINARY 

## MacOS build
# publish osx-x64

## Windows build.
# publish win-x64
