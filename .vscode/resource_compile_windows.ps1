#!/bin/bash
# buildEffects
# Compiles all .fx files found in the project's Content directory.
# Intended for usage with VS Code Build Tasks tooling.
# You may need to change the path to fxc.exe depending on your installation.

Write-Output "Compiling shaders...\n"

Set-Location $PSScriptRoot
Set-Location ../frontend/Resource

$files = Get-ChildItem -Path "*" -Recurse -Include *mgcb 

foreach ($file in $files)
{
    $fileName = $file.BaseName
    $filePath = $file.FullName
    & dotnet tool run mgcb /r "${fileName}.mgcb"

    if($LASTEXITCODE -ne 0)
    {
        exit 1
    }

    Write-Output "Built ${fileName}.mgcb"
}