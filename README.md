Download on [nuget](https://www.nuget.org/packages/FSharp.Data.Fred/)

[Changelog](https://github.com/nhirschey/FSharp.Data.Fred/blob/main/src/FSharp.Data.Fred/CHANGELOG.md)

## Development

Building and testing:
```
dotnet tool restore
dotnet paket restore
dotnet build
dotnet test
```

The docs literate scripts depend on release build dlls:
```
dotnet build --configuration Release
dotnet fsdocs watch --properties configuration=release --clean --eval
```

dotnet fsdocs watch --eval
```