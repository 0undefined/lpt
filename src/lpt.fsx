#r "lib/FsLexYacc/src/FsLex/bin/Release/netcoreapp3.1/FsLexYacc.Runtime"

open System.Text
open Parser
open FSharp.Text.Lexing

exception SyntaxError of int * int

let concat (s: string []) =
  Array.reduce (fun acc x -> acc + x) s

let parse_string (s: string) : AbSyn.UntypedExp =
  Parser.start Lexer.token
  <| LexBuffer<char>.FromString s

let parse_stdin =
  let rec read_input lines =
    match System.Console.ReadLine() with
      | null -> List.rev lines
      | ""   -> read_input lines
      | s    -> read_input (s::lines)

  read_input [] |> Array.ofList |> concat |> parse_string |> AbSyn.prettyformat |> printfn ": %s"

[<EntryPoint>]
let main (argv: string []) : int =
  try
    match argv with
      | [| |]         -> printfn "Missing input"
      | [|"-c"; _ |]  -> printfn "CNF transformation not supported yet"
      | [| formula |] -> parse_string formula |> AbSyn.prettyformat |> printfn ": %s"
      | formulas      -> concat formulas |> parse_string |> AbSyn.prettyformat |> printfn ": %s"
    0
  with
    | SyntaxError (line,col) ->
        printfn "Syntax error at %d:%d" line col
        System.Environment.Exit 1
        1
    | Lexer.LexicalError (errmsg, (line, col)) ->
        printfn "%s at %d:%d" errmsg line col
        System.Environment.Exit 1
        1
    | :? System.IO.FileNotFoundException as e ->
        printfn "File not found exception: %A" e
        System.Environment.Exit 1
        0
    | e ->
        printfn "Unknown error: %A" e
        System.Environment.Exit 1
        0
