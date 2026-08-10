#!/usr/bin/env bash

dotnet restore

if [ ! -d "build" ]; then
    mkdir build
fi

dotnet publish src -c Release -o build

(
  cd build
  # zip the XP_Pen.Wireless.Init.Fix.dll
  jar -cfM XP_Pen.Wireless.Init.Fix.zip ./*.dll

  sha256sum XP_Pen.Wireless.Init.Fix.zip >> hashes.txt
)