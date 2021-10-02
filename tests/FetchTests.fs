module FetchTests

open Fable.Core
open Fable.Core.JsInterop
open Fetch

importSideEffects "isomorphic-fetch"
importSideEffects "abortcontroller-polyfill/dist/polyfill-patch-fetch.js"

let inline equal (expected: 'T) (actual: 'T): unit =
    Testing.Assert.AreEqual(expected, actual)

[<Global>]
let it (msg: string) (f: unit->JS.Promise<'T>): unit = jsNative

[<Global>]
let describe (msg: string) (f: unit->unit): unit = jsNative

describe "Fetch tests" <| fun _ ->
    it "fetch: requests work" <| fun () ->
        let getWebPageLength url =
            fetch url []
            |> Promise.bind (fun res -> res.text())
            |> Promise.map (fun txt -> txt.Length)
        getWebPageLength "https://fable.io"
        |> Promise.map (fun res -> res > 0 |> equal true)

    it "fetch: parallel requests work" <| fun () ->
        let getWebPageLength url =
            promise {
                let! res = fetch url []
                let! txt = res.text()
                return txt.Length
            }
        [ "http://fable-compiler.github.io"
        ; "http://babeljs.io"
        ; "http://fsharp.org" ]
        |> List.map getWebPageLength
        |> Promise.Parallel
        // The sum of lenghts of all web pages is
        // expected to be bigger than 100 characters
        |> Promise.map (fun results -> (Array.sum results) > 100 |> equal true)

    it "fetch: invalid URL returns 404 (Not Found)" <| fun () ->
        promise {
            let! res = fetch "https://fable.io/this-must-be-an-invalid-url-no-really-i-mean-it" []
            return res.Status
        }
        |> Promise.map (fun results -> results |> equal 404)

    it "tryFetch: successful HTTP OPTIONS request" <| fun () ->
        let successMessage = "OPTIONS request accepted (method allowed)"
        let props = [ RequestProperties.Method HttpMethod.OPTIONS]
        tryFetch "https://gandi.net" props
        |> Promise.map (fun a ->
            match a with
            | Ok _ -> successMessage
            | Error e -> e.Message)
        |> Promise.map (fun results -> results |> equal successMessage)

    it "tryFetch: HTTP OPTIONS request on Fable.io returns 405 (Method not allowed)" <| fun () ->
        let props = [ RequestProperties.Method HttpMethod.OPTIONS]
        tryFetch "https://fable.io" props
        |> Promise.map (fun a ->
            match a with
            | Ok a -> Some a.Status
            | Error _ -> None)
        |> Promise.map (fun results -> results |> equal (Some 405))

    it "tryFetch: exception returns an Error" <| fun () ->
        tryFetch "http://this-must-be-an-invalid-url-no-really-i-mean-it.com" []
        |> Promise.map (fun a ->
            match a with
            | Ok _ -> "success"
            | Error _ -> "failure")
        |> Promise.map (fun results -> results |> equal "failure")

    it "tryOptionsRequest: successful HTTP OPTIONS request" <| fun () ->
        let successMessage = "OPTIONS request accepted (method allowed)"
        tryOptionsRequest "https://gandi.net"
        |> Promise.map (fun a ->
            match a with
            | Ok _ -> successMessage
            | Error e -> e.Message)
        |> Promise.map (fun results -> results |> equal successMessage)

    it "tryOptionsRequest: HTTP OPTIONS request on Fable.io returns 405 (Method not allowed)" <| fun () ->
        tryOptionsRequest "https://fable.io"
        |> Promise.map (fun a ->
            match a with
            | Ok a -> Some a.Status
            | Error _ -> None)
        |> Promise.map (fun results -> results |> equal (Some 405))

    it "fetch: request can be aborted" <| fun () ->
        let abortController = newAbortController()
        let props = [ RequestProperties.Signal abortController.signal ]

        let failMessage = "Request was aborted"
        let promise =
          tryFetch "https://fable.io" props
          |> Promise.map (function
              | Error e when e.Message = "Aborted" -> failMessage
              | Ok _ -> "Request was never cancelled"
              | Error _ -> "Something else went wrong")
          |> Promise.map (fun results -> results |> equal failMessage)

        abortController.abort()
        promise
