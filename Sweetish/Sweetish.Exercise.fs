namespace Sweetish

open Http
open Successful

module Combinators =
    let compose (first:WebPart) (second:WebPart) : WebPart = __

    let (>=>) = compose

    let choose (parts:WebPart list) : WebPart = __

module Filters =
    // This should filter and only handle GET requests
    let GET : WebPart = fun _ -> __
    // This should filter and only handle POST requests
    let POST : WebPart = fun _ -> __

    // this should filter and only handle routes which match a given path
    let Path (path:string) : WebPart = fun _ -> __