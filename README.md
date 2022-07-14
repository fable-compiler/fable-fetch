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

```fsharp
Response.create("Hello World!", [Status 200; ])
Response.create("Teapot!", [Status 418; Headers ["x-tea", "green"] ])
Response.create("Bad Request!", [Status 400; Headers ["x-custom", "fable"] ])
```

Check the [tests](https://github.com/fable-compiler/fable-fetch/blob/master/tests/FetchTests.fs) for other examples.
