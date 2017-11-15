module Tests

(*
//If you want to execute part of this file in the REPL, you must first load the following files:
#load "../Blank.fs"
#load "../Sweetish.fs"

#r "../../packages/NUnit/lib/nunit.framework.dll"
#load "../../paket-files/forki/FsUnit/FsUnit.fs"
*)

open System
open Sweetish
open Sweetish.Http
open Sweetish.Successful
open Sweetish.Combinators

type Test = NUnit.Framework.TestAttribute
open FsUnit

let emptyResponse = {
    StatusCode = 0
    Content = null
}

let createContext (verb, route) = {
    Request = { Verb = verb; Route = route }
    Response = emptyResponse
}

let dontHandle context = async { return None }

let [<Test>] ``OK returns a content and a status 200`` () =
    let webPart = OK "hello world"
    let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
    outputContext.IsSome |> shouldEqual true
    outputContext.Value.Response.StatusCode |> shouldEqual 200
    outputContext.Value.Response.Content |> shouldEqual "hello world"

let [<Test>] ``Compose chains the webparts`` () =
    let webPart = OK "hello world" >=> OK "bye!"
    let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
    outputContext.IsSome |> shouldEqual true
    outputContext.Value.Response.StatusCode |> shouldEqual 200
    outputContext.Value.Response.Content |> shouldEqual "bye!"

let [<Test>] ``Choose uses the first webpart if it handles the request`` () =
    let webPart = choose [ OK "hello world"; OK "bye!" ]
    let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
    outputContext.IsSome |> shouldEqual true
    outputContext.Value.Response.StatusCode |> shouldEqual 200
    outputContext.Value.Response.Content |> shouldEqual "hello world"

let [<Test>] ``Choose skips the first webpart if it doesn't handle the request`` () =
    let webPart = choose [ dontHandle ]
    let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
    outputContext |> shouldEqual None

let [<Test>] ``Choose uses the first webpart which handles the request`` () =
    let webPart = choose [ dontHandle; OK "bye!"; OK "hello world"; dontHandle ]
    let outputContext = createContext (Verb.GET, "/") |> webPart |> Async.RunSynchronously
    outputContext.IsSome |> shouldEqual true
    outputContext.Value.Response.StatusCode |> shouldEqual 200
    outputContext.Value.Response.Content |> shouldEqual "bye!"
