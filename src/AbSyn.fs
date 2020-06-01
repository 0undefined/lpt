module AbSyn

(*** Helper Functions ***)
let toCString (s : string) : string =
    let escape c =
        match c with
            | '\\' -> "\\\\"
            | '"'  -> "\\\""
            | '\n' -> "\\n"
            | '\t' -> "\\t"
            | _    -> System.String.Concat [c]
    String.collect escape s

// Doesn't actually support all escapes.  Too badefacilliteter.
let fromCString (s : string) : string =
    let rec unescape l: char list =
        match l with
            | []                -> []
            | '\\' :: 'n' :: l' -> '\n' :: unescape l'
            | '\\' :: 't' :: l' -> '\t' :: unescape l'
            | '\\' :: c   :: l' -> c    :: unescape l'
            | c           :: l' -> c    :: unescape l'
    Seq.toList s |> unescape |> System.String.Concat

(* position: (line, column) *)
type Position = int * int

type Exp<'T> =
  | Constant  of bool              * Position
  | Literal   of char              * Position
  | Not       of Exp<'T>           * Position
  | And       of Exp<'T> * Exp<'T> * Position
  | Or        of Exp<'T> * Exp<'T> * Position
  | Impl      of Exp<'T> * Exp<'T> * Position

(****************************************************)
(********** Pretty-Printing Functionality ***********)
(****************************************************)
//
//let rec indent = function
//  | 0 -> ""
//  | n -> "  " + indent (n-1)
//
//LET PPPARAM = FUNCTION
//  | PARAM(ID, TP) -> PPTYPE TP + " " + ID
//
//LET REC PPPARAMS = FUNCTION
//  | [] -> ""
//  | [BD] -> PPPARAM BD
//  | BD::L -> PPPARAM BD + ", " + PPPARAMS L
//
//LET REC PPVAL D = FUNCTION
//  | INTVAL N           -> SPRINTF "%I" N
//  | BOOLVAL B          -> SPRINTF "%B" B
//  | CHARVAL C          -> "'" + TOCSTRING (STRING C) + "'"
//  | ARRAYVAL (VALS, T) -> "{ " + (STRING.CONCAT ", " (LIST.MAP (PPVAL D) VALS)) + " }"
//
//LET NEWLINE EXP = MATCH EXP WITH
//                     | LET _ -> ""
//                     | _     -> "\N"
//
//LET REC PPEXP D = FUNCTION
//  | CONSTANT(V, _)              -> PPVAL D V
//  | STRINGLIT(S,_)              -> "\"" + TOCSTRING S + "\""
//  | ARRAYLIT(ES, T, _)          -> "{ " + (STRING.CONCAT ", " (LIST.MAP (PPEXP D) ES)) + " }"
//  | VAR (ID, _)                 -> ID
//  | PLUS (E1, E2, _)            -> "(" + PPEXP D E1 + " + "  + PPEXP D E2 + ")"
//  | MINUS (E1, E2, _)           -> "(" + PPEXP D E1 + " - "  + PPEXP D E2 + ")"
//  | TIMES (E1, E2, _)           -> "(" + PPEXP D E1 + " * "  + PPEXP D E2 + ")"
//  | DIVIDE (E1, E2, _)          -> "(" + PPEXP D E1 + " / "  + PPEXP D E2 + ")"
//  | AND (E1, E2, _)             -> "(" + PPEXP D E1 + " && " + PPEXP D E2 + ")"
//  | OR (E1, E2, _)              -> "(" + PPEXP D E1 + " || " + PPEXP D E2 + ")"
//  | NOT (E, _)                  -> "NOT("+PPEXP D E + ")"
//  | NEGATE (E, _)               -> "~(" + PPEXP D E + ")"
//  | EQUAL (E1, E2, _)           -> "(" + PPEXP D E1 + " == " + PPEXP D E2 + ")"
//  | LESS (E1, E2, _)            -> "(" + PPEXP D E1 + " < " + PPEXP D E2 + ")"
//  | IF (E1, E2, E3, _)          -> ("IF (" + PPEXP D E1 + ")\N" +
//                                    INDENT (D+2) + "THEN " + PPEXP (D+2) E2 + "\N" +
//                                    INDENT (D+2) + "ELSE " + PPEXP (D+2) E3 + "\N" +
//                                    INDENT D)
//  | APPLY (ID, ARGS, _)         -> (ID + "(" +
//                                    (STRING.CONCAT ", " (LIST.MAP (PPEXP D) ARGS)) + ")")
//  | LET (DEC(ID, E1, _), E2, _) -> ("\N" + INDENT (D+1) + "LET " + ID + " = " +
//                                    PPEXP (D+2) E1 + " IN" + NEWLINE E2 +
//                                    INDENT (D+1) + PPEXP D E2)
//  | INDEX (ID, E, T, _)         -> ID + "[" + PPEXP D E + "]"
//  | IOTA (E, _)                 -> "IOTA(" + PPEXP D E + ")"
//  | REPLICATE (E, EL, T, POS)   -> "REPLICATE(" + PPEXP D E + ", " + PPEXP D EL + ")"
//  | MAP (F, E, _, _, _)         -> "MAP(" + PPFUNARG D F + ", " + PPEXP D E + ")"
//  | FILTER (F, ARR, _, _)     -> ("FILTER(" + PPFUNARG D F + ", " + PPEXP D ARR + ")")
//  | REDUCE (F, EL, LST, _, _)   ->
//      "REDUCE(" + PPFUNARG D F + ", " + PPEXP D EL + ", " + PPEXP D LST + ")"
//  | SCAN (F, ACC, ARR, _, POS)  -> ("SCAN(" + PPFUNARG D F +
//                                    ", " + PPEXP D ACC +
//                                    ", " + PPEXP D ARR + ")")
//  | READ (T, _)                 -> "READ(" + PPTYPE T + ")"
//  | WRITE (E, T, _)             -> "WRITE(" + PPEXP D E + ")"

let expPos = function
  | Constant (_, p)    -> p
  | And      (_, _, p) -> p
  | Or       (_, _, p) -> p
  | Impl     (_, _, p) -> p
  | Not      (_, p)    -> p
  | _                  -> failwith "Parsing error, unable to get position"


type UntypedExp = Exp<unit>
type TypedExp   = Exp<bool>

let rec prettyformat (expr: Exp<'T>) : string =
  match expr with
  | Not (And (a,b,_),_)  -> "~(" + prettyformat (And (a,b,(0,0))) + ")"
  | Not (Or (a,b,_),_)   -> "~(" + prettyformat (Or (a,b,(0,0))) + ")"
  | Not (Impl (a,b,_),_) -> "~(" + prettyformat (Impl (a,b,(0,0))) + ")"
  | Not (n,_)       -> "~" + prettyformat n
  | Literal (l,_)   -> string l
  | Constant (c,_)  -> if c then "T" else "F"
  | And (a, b, _)   -> prettyformat a + " /\\ " + prettyformat b
  | Or  (a, b, _)   -> prettyformat a + " \\/ " + prettyformat b
  | Impl  (a, b, _) -> prettyformat a + " -> " + prettyformat b
