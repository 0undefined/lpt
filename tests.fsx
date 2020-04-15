#load "cnf.fsx"
open LogicParse


let test (prog: string) (expected_out: TOKEN list) =
  let rec checker n = function
    | (a :: aa, b :: bb) when a =  b -> checker (n+1) (aa, bb)
    | (a :: _ , b :: _ ) when a <> b -> (false, MISMATCH n)
    | (   [],    [])             -> (true,  MISMATCH n)
    | _                          -> (false, MISMATCH n)

  let res  = explode prog |> lex in
  let lens = (res.Length, expected_out.Length) in
  if fst lens <> snd lens then
    printfn "Lengths doesn't match! %d <> %d" (fst lens) (snd lens)
  else
    let (r, e) =
      match checker 0 (res, expected_out) with
      | (r, MISMATCH e) -> (r,e)
      | _ -> failwith "unexpected error"
    in
    if not r then
      printfn "output: %A" expected_out;
      printfn "result: %A" res;
      printfn "Mismatch at index %d: %s <> %s"
        e
        (expected_out.[e].ToString())
        (res.[e].ToString())


let test1     = "~ (p /\ q) \/ (~p/\q) -> r \/ (q /\ ~ s)"
let test1_out = [NEG;
  LPAR;      LITERAL "p"; CON; LITERAL "q"; RPAR; DIS;
  LPAR; NEG; LITERAL "p"; CON; LITERAL "q"; RPAR; IMPL; LITERAL "r"; DIS;
  LPAR;      LITERAL "q"; CON; NEG; LITERAL "s"; RPAR]

// Test with and without spacing
test test1 test1_out
test (String.filter (fun x -> x <> ' ') test1) test1_out

let test2     = "∀x(P x -> ∃y L x y)"
let test2_out = [
  FORALL; LITERAL "x"; LPAR; PREDICATE "P"; LITERAL "x"; IMPL;
  EXISTS; LITERAL "y"; PREDICATE "L"; LITERAL"y"; LITERAL"x"]

test test2 test2_out
test (String.filter (fun x -> x <> ' ') test2) test2_out
