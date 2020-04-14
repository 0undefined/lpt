type TOKEN =
  | LITERAL of string
  | CON  | DIS
  | NEG
  | LPAR | RPAR

let explode (s : string) = [for c in s -> c]
let isA t c              = List.exists (fun c' -> c = c') t

let operator   = ["/\\"; "\\/"; "~"]
let letter     = explode "abcdefghijklmnopqrstuvwxyz"
let asdf     = explode "\\/()~"
let whitespace = explode " \n"


let rec lex = function
  | (c :: _ ) as cs when c |> isA letter     -> lex_word "" cs
  | (c :: _ ) as cs when c |> isA asdf     -> lex_op   "" cs
  | (c :: cs)       when c |> isA whitespace -> lex cs
  | (c :: _ )                                -> failwith ("unknown : " + string c)
  | (_ : char list) -> ([] : TOKEN list)


and lex_word s = function
  | (c :: cs) when (c |> isA letter) -> lex_word (s + string c) cs
  |       cs                         -> LITERAL s :: lex cs


and lex_op s = function
  | (c :: cs) when string c |> isA operator -> get_op (string c) :: lex cs
  | (c :: cs) when c        |> isA asdf   -> lex_op (s + string c) cs
  |       cs                                -> get_op s :: lex cs

  (*
  | (c :: cs) when c = '~' && s = "" -> get_symbol c :: lex cs
  | (c :: cs) when c = '(' && s = "" -> get_symbol c :: lex cs
  | (c :: cs) when c = ')' && s = "" -> get_symbol c :: lex cs
   *)


and get_symbol = function
  | '~'        -> NEG
  | '('        -> LPAR
  | ')'        -> RPAR
  | _          -> failwith ("Unrecognized symbol: ") // + string sym)


and get_op = function
  | "~"   -> NEG  //:: get_op cs
  | "("   -> LPAR //:: get_op cs
  | ")"   -> RPAR //:: get_op cs
  | "/\\" -> CON
  | "\\/" -> DIS
  | op    -> failwith ("Unrecognized operator: " + string op)
  // | c::cs -> get_symbol c
  // | [] -> ([] : TOKEN list)


let rec checker = function
  | (a::aa, b::bb) when a = b   -> checker (aa, bb)
  | (a::_ , b::_ ) when a <> b  -> (false, (a,  b))
  | (   [],    [])              -> (true,  (LITERAL "ok", LITERAL "ok"))
  | (a::_ ,    [])              -> (false, (a, LITERAL "length mismatch"))
  | (   [], b::_ )              -> (false, (b, LITERAL "length mismatch"))
  | _                           -> (false, (LITERAL "error", LITERAL "error"))

let prog = "~ (p /\ q) \/ (~p/\q)"
let expected_output = [NEG; LPAR; LITERAL "p"; CON; LITERAL "q"; RPAR; DIS; LPAR; NEG; LITERAL "p"; CON; LITERAL "q"; RPAR]
let output = (explode prog |> lex)

printfn "input: %A"   prog
printfn "output: %A"  output
printfn "result: %A" (checker (output, expected_output))

