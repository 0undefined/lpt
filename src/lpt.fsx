open System.Text
open Parser
open FSharp.Text.Lexing

exception SyntaxError of int * int

let concat (s: string []) =
  Array.reduce (fun acc x -> acc + x) s

let parse_string (s: string) : AbSyn.UntypedExp =
  Parser.start Lexer.token
  <| LexBuffer<char>.FromString s

let parse_stdin () =
  let rec read_input () =
    match System.Console.ReadLine() with
      | null -> []
      | ""   -> read_input ()
      | s    -> s :: read_input ()

  // Should trim whitespace here
  let input = read_input () |> Array.ofList |> concat in
  if  input = ""
  then printfn "Missing input"
  else parse_string input
      |> AbSyn.prettyformat
      |> printfn ": %s"

let HELP_TEXT =
  [ "Usage: lpt [OPTION] [FORMULA]...\n"
  ; "By default interprets and validates logic formulas. Reads from stdin"
  ; " when no\nformula is provided. Each option slightly modifies behaviour.\n"
  ; "   -i, --identify   Identify formula and valid forms\n"
  ; "   -c, --cnf        Transform FORMULA(s) to CNF form\n"
  ; "   -h, --horn       Transform FORMULA(s) to HORN form\n"
  ; "   -n, --nnf        Transform FORMULA(s) to NNF form\n"
  ; "   -d, --dag        Transform FORMULA(s) representation into a DAG\n"
  ; "   --help           Print this help message\n"
  ; "   --version        Print version and exit\n"
  ]

let print_usage () =
  printf "%s" (List.reduce (+) HELP_TEXT)

[<EntryPoint>]
let main (argv: string []) : int =
  let args = Array.toList argv in
  try
    match args with
      | [] | "-"    :: _  -> parse_stdin ()
      | "--help"    :: _  -> print_usage ()
      | "--version" :: _  -> printfn "lpt - Logic-formula Parser and Transformer 1.0 pre-alpha"

      | "-i" :: _ | "--identify" :: _ -> printfn "Formula identification not supported yet"
      | "-c" :: _ | "--cnf"      :: _ -> printfn "CNF transformation not supported yet"
      | "-h" :: _ | "--horn"     :: _ -> printfn "Horn transformation not supported yet"
      | "-n" :: _ | "--nnf"      :: _ -> printfn "NNF transformation not supported yet"
      | "-d" :: _ | "--dag"      :: _ -> printfn "Directed Asyclic Graph transformation not supported yet"
      | f :: fms                      -> List.reduce (+) (f::fms) |> parse_string |> AbSyn.prettyformat |> printfn ": %s"
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
    | :? System.TypeInitializationException as e ->
        printfn "Exception: %A" e
        System.Environment.Exit 1
        0
