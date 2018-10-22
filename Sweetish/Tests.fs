module Tests

(*
//If you want to execute part of this file in the REPL, you must first load the following files:
#load "Blank.fs"
#load "Sweetish.fs"
#load "Sweetish.Exercise.fs"

let packagesFolder = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("userprofile"), @".nuget\packages")
let expecto = packagesFolder + @"\expecto\8.3.0\lib\netstandard2.0\Expecto.dll"
#r @"C:\Users\Pierre\.nuget\packages\expecto\8.3.0\lib\netstandard2.0\Expecto.dll"
*)

open Sweetish.Http
open Sweetish.Successful
open Sweetish.Combinators

open Expecto

let shouldEqual expected actual =
  Expect.equal actual expected "Values should be equal"

let emptyResponse = {
    StatusCode = 0
    Content = null
}

let createContext (verb, url) = {
    Request = { Verb = verb; Url = url }
    Response = emptyResponse
}

let dontHandle context = async { return None }

[<Tests>]
let tests =
    testList "All tests" [
        test "OK returns a content and a status 200" {
            let webPart = OK "hello world"
            let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
            outputContext.IsSome |> shouldEqual true
            outputContext.Value.Response.StatusCode |> shouldEqual 200
            outputContext.Value.Response.Content |> shouldEqual "hello world"
        }

        test "Compose chains the webparts" {
            let webPart = OK "hello world" >=> OK "bye!"
            let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
            outputContext.IsSome |> shouldEqual true
            outputContext.Value.Response.StatusCode |> shouldEqual 200
            outputContext.Value.Response.Content |> shouldEqual "bye!"
        }

        test "Choose uses the first webpart if it handles the request" {
            let webPart = choose [ OK "hello world"; OK "bye!" ]
            let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
            outputContext.IsSome |> shouldEqual true
            outputContext.Value.Response.StatusCode |> shouldEqual 200
            outputContext.Value.Response.Content |> shouldEqual "hello world"
        }

        test "Choose skips the first webpart if it doesn't handle the request" {
            let webPart = choose [ dontHandle ]
            let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
            outputContext |> shouldEqual None
        }

        test "Choose uses the first webpart which handles the request" {
            let webPart = choose [ dontHandle; OK "bye!"; OK "hello world"; dontHandle ]
            let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
            outputContext.IsSome |> shouldEqual true
            outputContext.Value.Response.StatusCode |> shouldEqual 200
            outputContext.Value.Response.Content |> shouldEqual "bye!"
        }
    ]
