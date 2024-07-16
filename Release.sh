#!/bin/bash
dotnet publish FileController -r win-x64 -p:PublishSingleFile=True --self-contained false -o "Release"