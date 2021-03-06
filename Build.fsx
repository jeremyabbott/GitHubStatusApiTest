#r "packages/FAKE/tools/FakeLib.dll" // include Fake lib
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System.IO
open System.Text
open Fake
open Fake.Git
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

open Newtonsoft.Json

type State = 
    | Pending
    | Failure
    | Success
    | Error

type Status = {
    state : string
    target_url : string
    description : string
    context : string
}


let stateString =
    function 
    | Pending -> "pending"
    | Failure -> "failure"
    | Success -> "success"
    | Error -> "error"

let gitRootUrl = "https://api.github.com"
let gitStatusEndPoint = "repos/jeremyabbott/GitHubStatusApiTest/statuses"

Target "Test" (fun _ -> 
    let hash = Fake.Git.Information.getCurrentSHA1 "./"

    trace <| hash)

Target "PostStatus" (fun _ -> 
    let token = getBuildParamOrDefault "token" "test"
    let url = sprintf "%s/%s/%s" gitRootUrl gitStatusEndPoint (getCurrentSHA1 "./")

    let state = Error |> stateString
    let status = {
        state = state
        target_url = ""
        description = "test error"
        context = "Fake error"
    }

    let body = JsonConvert.SerializeObject(status) |> TextRequest
    printfn "%A" body
    printfn "%s" url
    let request =
        Http.Request 
            (url = url, headers = [ContentType HttpContentTypes.Json; UserAgent "FSharp.Data - Fake"], query=["access_token", token], body=body)

    
    ())

RunTargetOrDefault "Test"