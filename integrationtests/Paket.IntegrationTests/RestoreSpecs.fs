﻿module Paket.IntegrationTests.RestoreSpec

open System
open System.IO
open Fake
open NUnit.Framework
open FsUnit

[<Test>]
let ``#2496 Paket fails on projects that target multiple frameworks``() = 
    let project = "EmptyTarget"
    let scenario = "i002496"
    prepareSdk scenario

    let wd = (scenarioTempPath scenario) @@ project
    directDotnet true (sprintf "restore %s.csproj" project) wd
        |> ignore
        
    
[<Test>]
let ``#2642 dotnet restore writes paket references file to correct obj dir``() = 
    let project = "ObjDir"
    let scenario = "i002642-obj-dir"
    let tmpPaketFolder = (scenarioTempPath scenario) @@ "../.paket"
    let targetsFile = FullName(__SOURCE_DIRECTORY__ + "../../../src/Paket/embedded/Paket.Restore.targets")
    let paketExe = FullName(__SOURCE_DIRECTORY__ + "../../../bin/paket.exe")

    setEnvironVar "PaketExePath" paketExe    
    prepare scenario
    if (not (Directory.Exists tmpPaketFolder)) then
        Directory.CreateDirectory tmpPaketFolder |> ignore

    FileHelper.CopyFile tmpPaketFolder targetsFile
    FileHelper.CopyFile tmpPaketFolder paketExe
    
    let wd = (scenarioTempPath scenario) @@ project
    let restore =
        let dotnetExePath =
            match Environment.GetEnvironmentVariable "DOTNET_EXE_PATH" with
            | null | "" -> "dotnet"
            | s -> s
        ProcessHelper.ExecProcessAndReturnMessages (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- wd
            info.Arguments <- (sprintf "restore %s.csproj" project)
        )
    let result = restore <| TimeSpan.FromMinutes 3.
    let exitCode = result.ExitCode

    result.Errors.ForEach (fun m-> tracefn "%s" m )
    result.Messages.ForEach (fun m-> tracefn "%s" m )
    
    let hasErrorOut = result.Errors.Count > 0;
   
    match (result.OK, hasErrorOut) with 
    | (true, false) ->
        printfn "all good!"
        // do the actual asserts:
        let originalPath = wd @@ "obj"
        if Directory.Exists originalPath then
            let files = Directory.GetFiles (originalPath, "*", SearchOption.AllDirectories)
            if files.Length > 0 then
                failwithf "Expected no files in obj, but got %A" files
        let modifiedPath = wd @@ "MyCustomFancyObjDir"
        Directory.Exists modifiedPath |> shouldEqual true
        let expectedFiles =
            [ modifiedPath @@ "ObjDir.csproj.NuGet.Config"
              modifiedPath @@ "ObjDir.csproj.references" ]
            |> set
        let actualFiles = Directory.GetFiles (modifiedPath, "*", SearchOption.AllDirectories) |> set
        let missingFiles = expectedFiles - actualFiles
        Assert.AreEqual(Set.empty, missingFiles)
    | _ -> failwithf "dotnet restore exitCode %d > 0? or hasErrorOut %b" exitCode hasErrorOut