namespace Sweetish

module Http =
    type [<RequireQualifiedAccess>] Verb = GET | POST
    type Request = {
        Verb : Verb
        Url : string
    }

    type Response = {
        StatusCode : int
        Content : string
    }

    type Context = {
        Request : Request
        Response : Response
    }

type WebPart = Http.Context -> Async<Http.Context option>

module Successful =
    open Http

    let OK (content:string) context = async {
        return Some {
            context with Response = { Content = content; StatusCode = 200 }
        }
    }