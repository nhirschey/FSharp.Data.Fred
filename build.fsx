#r "nuget: Fun.Build, 1.0.4"

open Fun.Build

let solutionFile = "FSharp.Data.Fred.sln"
let configuration = "Release"

pipeline "CI" {
    stage "Restore" {
        run "dotnet tool restore"
        run "dotnet paket restore"
    }

    stage "Build" { run $"dotnet build {solutionFile} --configuration {configuration}" }

    stage "Test" {
        run $"dotnet test {solutionFile} --configuration {configuration} --no-build"
    }

    stage "Docs" {
        whenNot { envVar "RUNNER_OS" "Windows" }
        run $"dotnet fsdocs build --eval --clean --properties Configuration={configuration}"
    }

    stage "Pack" {
        run $"dotnet pack {solutionFile} --configuration {configuration} --no-build"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()
