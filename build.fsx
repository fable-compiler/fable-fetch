#load "node_modules/fable-publish-utils/PublishUtils.fs"
open PublishUtils

run "npm test"
match args with
| IgnoreCase "publish"::_ ->
    pushFableNuget "src/Fable.Fetch.fsproj" [] doNothing
| _ -> ()
