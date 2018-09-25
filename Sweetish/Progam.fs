open System
open Sweetish

module Console =
    open Sweetish.Http

    let execute inputContext webpart =
        async {
            let! outputContext = webpart inputContext
            match outputContext with
            | Some context ->
                printfn "--------------"
                printfn "Code : %d" context.Response.StatusCode
                printfn "Output : %s" context.Response.Content
                printfn "--------------"
            | None ->
                printfn "No Output"
        } |> Async.RunSynchronously

    let parseRequest (input : System.String) =
        let parts = input.Split([|';'|])
        let rawType = parts.[0]
        let route = parts.[1]
        match rawType with
        | "GET" -> { Verb = Verb.GET; Route = route }
        | "POST" -> { Verb = Verb.POST; Route = route }
        | _ -> failwith "invalid request"

    let printInstructions() =
        printfn "Valid routes are of the form \"VERB;route\", where VERB can be either GET or POST."
        printfn "For instance:"
        printfn "  GET;/"
        printfn "  POST;/user/123"
        printfn ""

    let executeInLoop webpart =
        let emptyRequest = { Route = ""; Verb = Verb.GET }
        let emptyResponse = { Content = ""; StatusCode = 200 }
        let emptyContext = { Request = emptyRequest; Response = emptyResponse }

        let rec loop () =
            printf "Enter Input Route (or exit to exit):"
            let input = System.Console.ReadLine()
            try
                if input = "exit" then
                    ()
                else
                    let context = { emptyContext with Request = parseRequest input }
                    execute context webpart
                    loop ()
            with
            | ex ->
                printfn ""
                printfn "Error : %s" ex.Message
                printfn ""
                printInstructions()
                loop ()

        printfn "Welcome to Sweetish console!"
        printInstructions()
        loop ()

open Expecto

[<EntryPoint>]
let main argv =
    match argv with
        | [| "tests" |] -> Tests.runTestsInAssembly { defaultConfig with ``parallel`` = false } Array.empty
        | _ ->
            Console.executeInLoop (Successful.OK "hello world")
            0