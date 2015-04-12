﻿module Paket.BindingRedirects

open Paket
open Paket.BindingRedirects
open NUnit.Framework
open System.Xml.Linq
open FsUnit

let defaultRedirect =
    {   AssemblyName = "Assembly"
        Version = "1.0.0"
        PublicKeyToken = "PUBLIC_KEY"
        Culture = None }

let sampleDoc() = """<?xml version="1.0" encoding="utf-8"?>
<configuration>
</configuration>""" |> XDocument.Parse

let private bindingNs = "urn:schemas-microsoft-com:asm.v1"
let private containsDescendents count ns elementName (doc:XDocument) =
    Assert.AreEqual(count, doc.Descendants(XName.Get(elementName, ns)) |> Seq.length)
let private containsSingleDescendent = containsDescendents 1 ""
let private containsSingleDescendentWithNs = containsDescendents 1 bindingNs
let private createBindingRedirectXml culture assembly version publicKey = sprintf "<dependentAssembly xmlns=\"urn:schemas-microsoft-com:asm.v1\">\r\n  <assemblyIdentity name=\"%s\" publicKeyToken=\"%s\" culture=\"%s\" />\r\n  <bindingRedirect oldVersion=\"0.0.0.0-%s\" newVersion=\"%s\" />\r\n</dependentAssembly>" assembly publicKey culture version version
let private xNameForNs name = XName.Get(name, bindingNs)

let sampleDocWithNoIndentation() = sprintf """<?xml version="1.0" encoding="utf-8"?>
<configuration>
<runtime><assemblyBinding xmlns="%s">%s</assemblyBinding></runtime></configuration>""" bindingNs (createBindingRedirectXml "cul" "asm" "v" "pKey") |> XDocument.Parse

[<Test>]
let ``add missing elements to configuration file``() = 
    let doc = sampleDoc()

    // Act
    setRedirect doc defaultRedirect |> ignore

    // Assert
    doc |> containsSingleDescendent "runtime"
    doc |> containsSingleDescendentWithNs "assemblyBinding"

[<Test>]
let ``add new binding redirect to configuration file``() = 
    let doc = sampleDoc()

    // Act
    setRedirect doc defaultRedirect |> ignore

    // Assert
    doc |> containsSingleDescendentWithNs "dependentAssembly"


[<Test>]
let ``correctly creates a binding redirect``() = 
    let doc = sampleDoc()
    setRedirect doc { defaultRedirect with Culture = Some "en-gb"; PublicKeyToken = "123456" } |> ignore

    // Act
    let dependency = doc.Descendants(xNameForNs "dependentAssembly") |> Seq.head

    // Assert
    dependency.ToString() |> shouldEqual (createBindingRedirectXml "en-gb" "Assembly" "1.0.0" "123456")

[<Test>]
let ``correctly creates a binding redirect with default culture``() = 
    let doc = sampleDoc()
    setRedirect doc defaultRedirect |> ignore

    // Act
    let dependency = doc.Descendants(xNameForNs "dependentAssembly") |> Seq.head

    // Assert
    dependency.ToString() |> shouldEqual (createBindingRedirectXml "neutral" "Assembly" "1.0.0" "PUBLIC_KEY")

[<Test>]
let ``does not overwrite existing binding redirects for a different assembly``() = 
    let doc = sampleDoc()
    setRedirect doc defaultRedirect |> ignore

    // Act
    setRedirect doc { defaultRedirect with AssemblyName = "OtherAssembly" } |> ignore

    // Assert
    doc |> containsDescendents 2 bindingNs "dependentAssembly"

[<Test>]
let ``does not add a new binding redirect if one already exists for the assembly``() = 
    let doc = sampleDoc()
    setRedirect doc defaultRedirect |> ignore

    // Act
    setRedirect doc { defaultRedirect with Version = "2.0.0" } |> ignore

    // Assert
    doc |> containsSingleDescendentWithNs "dependentAssembly"

[<Test>]
let ``correctly updates an existing binding redirect``() = 
    let doc = sampleDoc()
    setRedirect doc defaultRedirect |> ignore

    // Act
    setRedirect doc { defaultRedirect with Version = "2.0.0" } |> ignore

    // Assert
    let dependency = doc.Descendants(xNameForNs "dependentAssembly") |> Seq.head
    dependency.ToString() |> shouldEqual (createBindingRedirectXml "neutral" "Assembly" "2.0.0" "PUBLIC_KEY")
    
[<Test>]
let ``redirects got properly indented for readability``() = 
    let doc = sampleDoc()
    setRedirect doc defaultRedirect |> ignore

    // Act
    indentAssemblyBindings doc

    // Assert
    let dependency = doc.Descendants(xNameForNs "dependentAssembly") |> Seq.head
    dependency.ToString() |> shouldEqual "<dependentAssembly xmlns=\"urn:schemas-microsoft-com:asm.v1\">\r\n    <assemblyIdentity name=\"Assembly\" publicKeyToken=\"PUBLIC_KEY\" culture=\"neutral\" />\r\n    <bindingRedirect oldVersion=\"0.0.0.0-1.0.0\" newVersion=\"1.0.0\" />\r\n  </dependentAssembly>"

