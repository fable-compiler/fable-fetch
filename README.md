# Fable.Fetch

Fable bindings for Browsers' [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).

If you need helpers for automatic JSON serialization, check [Thoth.Fetch](https://github.com/thoth-org/Thoth.Fetch#thothfetch-).

- Run tests: `npm test`
- Publish: `npm run build Publish`

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
