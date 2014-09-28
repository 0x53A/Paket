﻿module Paket.FilterVersionSpecs

open Paket
open NUnit.Framework
open FsUnit

let isInRangeNoPreRelease (versionRange:VersionRange) semVer =
    VersionRequirement(versionRange,PreReleaseStatus.No).IsInRange (SemVer.parse semVer)

let isInRange (version:VersionRequirement) semVer =
    version.IsInRange (SemVer.parse semVer)

[<Test>]
let ``can check if in range for Specific``() =
    "2.2" |> isInRangeNoPreRelease (VersionRange.Specific (SemVer.parse "2.2")) |> shouldEqual true
    "2.4" |> isInRangeNoPreRelease (VersionRange.Specific (SemVer.parse "2.2")) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Specific (SemVer.parse "2.4")) |> shouldEqual false
    
[<Test>]
let ``can check if in range for Minimum``() =
    "2.1" |> isInRangeNoPreRelease (VersionRange.Minimum (SemVer.parse "2.2")) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Minimum (SemVer.parse "2.2")) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Minimum (SemVer.parse "2.2")) |> shouldEqual true
    
[<Test>]
let ``can check if in range for GreaterThan``() =
    "2.1" |> isInRangeNoPreRelease (VersionRange.GreaterThan (SemVer.parse "2.2")) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.GreaterThan (SemVer.parse "2.2")) |> shouldEqual false
    "3.0" |> isInRangeNoPreRelease (VersionRange.GreaterThan (SemVer.parse "2.2")) |> shouldEqual true

[<Test>]
let ``can check if in range for Maximum``() =
    "2.0" |> isInRangeNoPreRelease (VersionRange.Maximum (SemVer.parse "2.2")) |> shouldEqual true
    "2.2" |> isInRangeNoPreRelease (VersionRange.Maximum (SemVer.parse "2.2")) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Maximum (SemVer.parse "2.2")) |> shouldEqual false

[<Test>]
let ``can check if in range for LessThan``() =
    "2.0" |> isInRangeNoPreRelease (VersionRange.LessThan (SemVer.parse "2.2")) |> shouldEqual true
    "2.2" |> isInRangeNoPreRelease (VersionRange.LessThan (SemVer.parse "2.2")) |> shouldEqual false
    "3.0" |> isInRangeNoPreRelease (VersionRange.LessThan (SemVer.parse "2.2")) |> shouldEqual false
    
[<Test>]
let ``can check if in range for Range``() =
    "2.1" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false
    "2.5" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false
    "3.2" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false

    "2.1" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual false
    "2.5" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual true
    "3.2" |> isInRangeNoPreRelease (VersionRange.Range (Excluding, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual false

    "2.1" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual true
    "2.5" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false
    "3.2" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Excluding)) |> shouldEqual false

    "2.1" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual false
    "2.2" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual true
    "2.5" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual true
    "3.0" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual true
    "3.2" |> isInRangeNoPreRelease (VersionRange.Range (Including, (SemVer.parse "2.2"), (SemVer.parse "3.0"), Including)) |> shouldEqual false

[<Test>]
let ``can check if in range for 4-parts range``() =    
    "1.0.0.3108" |> isInRange (DependenciesFileParser.parseVersionRequirement "1.0.0.3108") |> shouldEqual true
    "1.0.0.2420" |> isInRange (DependenciesFileParser.parseVersionRequirement "~> 1.0") |> shouldEqual true

[<Test>]
let ``can support trailing 0``() =    
    "1.2.3" |> isInRange (DependenciesFileParser.parseVersionRequirement "1.2.3.0") |> shouldEqual true    

[<Test>]
let ``can support alpha version``() = 
    "1.2.3-alpha001" |> isInRange (DependenciesFileParser.parseVersionRequirement "1.2.3-alpha001") |> shouldEqual true
    "1.2.3-alpha001" |> isInRange (DependenciesFileParser.parseVersionRequirement "1.2.3") |> shouldEqual false
    "1.2.3-alpha003" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1") |> shouldEqual false

    "1.2.3-alpha003" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 prerelease") |> shouldEqual true
    "1.2.3-alpha023" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 alpha") |> shouldEqual true
    "1.2.3-alpha023" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 alpha rc") |> shouldEqual true
    "1.2.3-alpha023" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 beta rc") |> shouldEqual false

[<Test>]
let ``can support rc version``() = 
    "1.2.3-rec003" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 prerelease") |> shouldEqual true
    "1.2.3-rc2" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 alpha") |> shouldEqual false
    "1.2.3-rc2" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 1 beta rc") |> shouldEqual true

    "1.2.3-rc2" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 2 beta rc") |> shouldEqual false

[<Test>]
let ``can support "build" version``() = 
    "0.9.0-build06428" |> isInRange (DependenciesFileParser.parseVersionRequirement ">= 0.9.0-build06428") |> shouldEqual true
    