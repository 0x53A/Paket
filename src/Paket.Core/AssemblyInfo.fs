﻿namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Paket.Core")>]
[<assembly: AssemblyProductAttribute("Paket")>]
[<assembly: AssemblyCompanyAttribute("Paket team")>]
[<assembly: AssemblyDescriptionAttribute("A package dependency manager for .NET with support for NuGet packages and GitHub repositories.")>]
[<assembly: AssemblyVersionAttribute("1.33.4")>]
[<assembly: AssemblyFileVersionAttribute("1.33.4")>]
[<assembly: AssemblyInformationalVersionAttribute("1.33.4")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.33.4"
