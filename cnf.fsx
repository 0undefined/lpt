module Cnf

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
let single_letter_symbol = explode "()~"
let whitespace = explode " \n"


let rec lex = function
  | (c :: _ ) as cs when c |> isA letter     -> lex_word "" cs
//| (c :: cs)       when c |> isA single_letter_symbol -> get_symbol c :: lex cs
  | (c :: _ ) as cs when c |> isA asdf     -> lex_op   "" cs
  | (c :: cs)       when c |> isA whitespace -> lex cs
  | (c :: _ )                                -> failwith ("unknown : " + string c)
  | (_ : char list) -> ([] : TOKEN list)


and lex_word s = function
  | (c :: cs) when (c |> isA letter) -> lex_word (s + string c) cs
  |       cs                         -> LITERAL s :: lex cs


and lex_op s = function
  | (c :: cs) when s + string c     |> isA operator             -> get_op (s + string c) :: lex cs
  | (c :: cs) when c            |> isA single_letter_symbol -> get_op (string c) :: lex cs
  | (c :: cs) when c            |> isA asdf                 -> lex_op (s + string c) cs
  |       cs                                                -> get_op s :: lex cs


and get_symbol = function
  | '~'        -> NEG
  | '('        -> LPAR
  | ')'        -> RPAR
  | _          -> failwith ("Unrecognized symbol: ")


and get_op = function
  | "~"   -> NEG
  | "("   -> LPAR
  | ")"   -> RPAR
  | "/\\" -> CON
  | "\\/" -> DIS
  | op    -> failwith ("Unrecognized operator: " + string op)
