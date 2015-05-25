﻿namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Paket")>]
[<assembly: AssemblyProductAttribute("Paket")>]
[<assembly: AssemblyCompanyAttribute("Paket team")>]
[<assembly: AssemblyDescriptionAttribute("A package dependency manager for .NET with support for NuGet packages and GitHub repositories.")>]
[<assembly: AssemblyVersionAttribute("1.5.6")>]
[<assembly: AssemblyFileVersionAttribute("1.5.6")>]
[<assembly: AssemblyInformationalVersionAttribute("1.5.6")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.5.6"
