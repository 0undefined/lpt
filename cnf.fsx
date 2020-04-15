module LogicParse

type TOKEN =
  | LITERAL   of string
  | PREDICATE of string
  | NEG | FORALL | EXISTS
  | CON | DIS
  | IMPL
  | LPAR | RPAR
  | MISMATCH of int // Only used for debugging

let explode (s : string) = [for c in s -> c]
let isA t c              = List.exists (fun c' -> c = c') t

let operator             = ["/\\"; "\\/"; "~"; "∀"; "∃"]
let letter               = explode "abcdefghijklmnopqrstuvwxyz"
let LETTER               = explode "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
let asdf                 = explode "\\/()~->"
let single_letter_symbol = explode "()~"
let whitespace           = explode " \n"


let rec lex = function
  | (c :: _ ) as cs when c |> isA letter     -> lex_word "" cs
  | (c :: _ ) as cs when c |> isA asdf       -> lex_op   "" cs
  | (c :: cs)       when c |> isA whitespace -> lex cs
  | (c :: _ )                                -> failwith ("Unhandled symbol: " + string c)
  | (_ : char list) -> ([] : TOKEN list)


and lex_word s = function
  | (c :: cs) when (c |> isA letter) -> lex_word (s + string c) cs
  |       cs                         -> LITERAL s :: lex cs


and lex_op s = function
  | (c :: cs) when s + string c |> isA operator             -> get_op (s + string c) :: lex cs
  | (c :: cs) when c            |> isA single_letter_symbol -> get_op (string c)     :: lex cs
  | (c :: cs) when c            |> isA asdf                 -> lex_op (s + string c) cs
  |       cs                                                -> get_op s              :: lex cs


and get_op = function
  | "~"   -> NEG
  | "∀"   -> FORALL
  | "∃"   -> EXISTS
  | "("   -> LPAR
  | ")"   -> RPAR
  | "/\\" -> CON
  | "\\/" -> DIS
  | "->"  -> IMPL
  | op    -> failwith ("Unrecognized operator: " + string op)
