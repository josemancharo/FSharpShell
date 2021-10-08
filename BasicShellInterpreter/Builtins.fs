module Builtins

open System
open System.IO
open System.Linq


type StringOrUnit = 
| Str of String
| Void of unit

type Command = { KeyWord: string; Function: Func<string, StringOrUnit>}

let userProfileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

let mutable promptShebang = "ϟ"

let setShebang newShebang = (promptShebang <- newShebang) |> Void

let cd (arg:string) = 
    let directory = arg.Trim()
    match directory with
        | "~" -> userProfileDir
        | x when x.StartsWith("~") -> Path.Combine(userProfileDir, x.Remove(0,1))
        | _ -> Path.Combine(Directory.GetCurrentDirectory(), arg)
    |> Directory.SetCurrentDirectory
    |> Void

let dir directoryListing = 
    query {
        for entry in directoryListing do 
        let name = Path.GetRelativePath(Directory.GetCurrentDirectory(), entry)
        select name
    }

let ls path =
    let directory = if String.IsNullOrWhiteSpace(path) then Directory.GetCurrentDirectory() else path
    let result = 
        Directory.EnumerateFileSystemEntries(directory).AsQueryable()
        |> dir 
        |> fun x -> String.Join('\n', x)
    result |> Str

let echo value = value |> Str