﻿/// Contains NuGet support.
module Paket.NuGetV3

open Newtonsoft.Json
open System.Collections.Generic

/// [omit]
type JSONResource = 
    { Type : string;
      ID: string }

/// [omit]
type JSONVersionData = 
    { Data : string [] }

/// [omit]
type JSONRootData = 
    { Resources : JSONResource [] }

/// [omit]
let getSearchAutocompleteService (data : string) =  
    JsonConvert.DeserializeObject<JSONRootData>(data.Replace("@id","ID").Replace("@type","Type")).Resources
    |> Array.tryFind (fun x -> x.Type <> null && x.Type.ToLower() = "searchautocompleteservice")
    |> Option.map (fun x -> x.ID)

/// [omit]
let private searchDict = new System.Collections.Concurrent.ConcurrentDictionary<_,_>()

/// Calculates the NuGet v3 URL from a NuGet v2 URL.
let calculateNuGet3Path(nugetUrl:string) = 
    match nugetUrl.TrimEnd([|'/'|]) with
    | "http://nuget.org/api/v2" -> Some "http://api.nuget.org/v3/index.json"
    | "https://nuget.org/api/v2" -> Some "https://api.nuget.org/v3/index.json"
    | url when url.EndsWith("api/v2") && url.Contains("myget.org") -> Some (url.Replace("api/v2","api/v3/index.json"))
    | _ -> None

/// [omit]
let getSearchAPI(auth,nugetUrl) = 
    match searchDict.TryGetValue nugetUrl with
    | true,v -> v
    | _ ->
        let result = 
            match calculateNuGet3Path nugetUrl with
            | None -> None
            | Some v3Path ->
                let serviceData =
                    safeGetFromUrl(auth,v3Path) 
                    |> Async.RunSynchronously

                match serviceData with
                | None -> None
                | Some data -> getSearchAutocompleteService data

        searchDict.[nugetUrl] <- result
        result

/// [omit]
let extractVersions(response:string) =
    JsonConvert.DeserializeObject<JSONVersionData>(response).Data

/// Uses the NuGet v3 autocomplete service to retrieve all package versions for the given package.
let FindVersionsForPackage(auth, nugetURL, package, maxResults) =
    async {
        match getSearchAPI(auth,nugetURL) with        
        | Some url ->
            let! response = safeGetFromUrl(auth,sprintf "%s?id=%s&take=%d" url package 100000) // Nuget is showing old versions first
            match response with
            | Some text -> 
                let r = extractVersions text
                if r.Length > maxResults then
                    return r |> Seq.take maxResults |> Seq.toArray
                else
                    return r
            | None -> return [||]
        | None -> return [||]
    }

/// [omit]
let extractPackages(response:string) =
    JsonConvert.DeserializeObject<JSONVersionData>(response).Data

let private getPackages(auth, nugetURL, packageNamePrefix, maxResults) = async {
    match getSearchAPI(auth,nugetURL) with
    | Some url -> 
        let query = sprintf "%s?q=%s&take=%d" url packageNamePrefix maxResults
        let! response = safeGetFromUrl(auth,query)
        match response with
        | Some text -> return extractPackages text
        | None -> return [||]
    | None -> return [||]
}

/// Uses the NuGet v3 autocomplete service to retrieve all packages with the given prefix.
let FindPackages(auth, nugetURL, packageNamePrefix, maxResults) =
    async {
        let! packages = getPackages(auth, nugetURL, packageNamePrefix, maxResults)                        

        return SemVer.SortVersions packages
    }
