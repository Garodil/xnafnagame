# Build prerequisites
- Microsoft Directx SDK (june 2010)
- .net 8.0
- MGCB edit

# Installing prerequisites
## Directx SDK
Needed for building shaders for MojoShader. Can be found at following locations:

- https://www.microsoft.com/en-us/download/details.aspx?id=6812
- https://download.microsoft.com/download/a/e/7/ae743f1f-632b-4809-87a9-aa1bb3458e31/DXSDK_Jun10.exe

### Windows
Install at default path

### Linux
Install at default path with wine.
Shader build script expects your wipe prefixto be located at ~/.wine

## MGCB edit
A content pipeline for building XNB archives for FNA. Should be installed automatically.
Install local version with `dotnet tool restore`

Install global version with
```
dotnet tool install --global dotnet-mgcb-editor --version 3.8.4
dotnet tool install --global dotnet-mgcb-editor-windows --version 3.8.4
dotnet tool install --global dotnet-mgcb-editor-linux --version 3.8.4
dotnet tool install --global dotnet-mgcb --version 3.8.4
```

Either way can be run with `dotnet mgcb-editor` or with "Launch MGCB editor" at Run & Debug

# xnafnagame
