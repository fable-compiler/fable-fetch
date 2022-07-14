# Fable.Fetch

Fable bindings for Browsers' [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).

If you need helpers for automatic JSON serialization, check [Thoth.Fetch](https://github.com/thoth-org/Thoth.Fetch#thothfetch-).

- Run tests: `npm test`
- Publish: `npm run publish`

## Usage

```fsharp
type IUser =
  abstract name: string

let fetchGitHubUser accessToken =
  async {
    let! response =
     fetch "https://api.github.com/user" [
          requestHeaders [
              HttpRequestHeaders.Authorization $"token {accessToken}"
          ] ] |> Async.AwaitPromise
    let! item = response.json<IUser>() |> Async.AwaitPromise
  }
```

Response Usage

Normally you don't need to use Response manually but, if you are in a service worker and intercepting fetch requests, you can programatically create responses that satisfy your logic, there are a few javascript runtimes that also offer http frameworks that work with responses directly so here is a few ways to create responses

```fsharp
Response.create("Hello World!", [Status 200; ])
Response.create("Teapot!", [Status 418; Headers ["x-tea", "green"] ])
Response.create("Bad Request!", [Status 400; Headers ["x-custom", "fable"] ])
Response.create("""{ "message": "Bad Request!" }""", [Status 400; Headers ["content-type", "application/json"] ])
Response.create(
  Blob.Create([| csvfile |], unbox {| ``type`` = "text/csv" |}),
  [ Status 200; Headers ["content-type", "text/csv"] ]
)

```

Check the [tests](https://github.com/fable-compiler/fable-fetch/blob/master/tests/FetchTests.fs) for other examples.
