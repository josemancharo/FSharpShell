open System
open System.IO
open Microsoft.FSharp.Quotations
open System.Linq
open BlackFox.ColoredPrintf
open System.Diagnostics
open System.Text
open Builtins

Console.OutputEncoding <- Encoding.UTF8
Console.InputEncoding <- Encoding.UTF8

let matchCommand (args:string) = 
    let command = args.Trim().Split(' ').Select(fun x -> x.Trim())
    let parameters = String.Join(' ', command.Skip(1))
    printfn "%s %s" (command.FirstOrDefault()) parameters
    let result = 
        match command.FirstOrDefault() with
        | "ls" ->  ls parameters
        | "cd" ->  cd parameters
        | "echo" -> echo parameters
        | "shebang" -> setShebang parameters
        | "exit" -> exit 0
        | x -> Process.Start(x, parameters).WaitForExit() |> Void
    result


let interpret() =
    Console.ReadLine()
    |> matchCommand
    |> function 
        | Str(x) -> colorprintfn "%A" x
        | Void(x) -> do x

let writePrompt() =
    Console.ForegroundColor <- ConsoleColor.Blue
    Directory.GetCurrentDirectory() |> Console.Write    
    Console.ForegroundColor <- ConsoleColor.Green
    Console.Write($" {promptShebang} ")
    Console.ForegroundColor <- ConsoleColor.White

[<EntryPoint>]
let main argv =
    while true do
        writePrompt()
        try interpret()
        with 
        | ex -> colorprintfn "$red[%s]" ex.Message
    0