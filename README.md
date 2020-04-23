# Logic-formula Parser and Transformer (lpt)

Read, parse, verify, and transform (propositional) logic formulas of the style
described in Huth and Ryans book "Logic in computer science".


## Status

The current status with `lpt` is a very limited syntax which works and `lpt` is
only able to parse conjunctions, disjunctions and negations. To see a more
complete feature list go to [roadmap](#roadmap)


## Compiling

Requirements

* F#
* Dotnet
* Make

1. `git clone --recursive <git-url>`
2. `cd lpt`
   _One might need to edit lib/FsLexYacc/global.json to match the installed version of dotnet_
3. `make`


## Usage

By default lpt print wether the input is valid or not and which form it is in.
If no argument is passed, it tries to read from `stdin`:

```
$ lpt "~(~p\/q)/\~(p -> q)"
```


### Roadmap

There is a lot of features planned which should not be that hard to implement,
but requires time none the less.

* [ ] Full propositional logic syntax support
  + [x] Negation `~`
  + [x] Conjunctions `/\`
  + [x] Disjunctions `\/`
  + [x] Parenthesis
  + [ ] Implication `->`
* [ ] Transformation
  + [ ] Normal forms
    - [ ] CNF
    - [ ] Horn
* [ ] Identify and display which form the input formula is
* [ ] _perhaps_ Predicate logic syntax support
  + [x] UTF8
  + [ ] Forall `∀`
  + [ ] Exists `∃`
  + [ ] Function symbols
  + [ ] Predicates
  + [ ] Terms
  + [ ] Variables
* [ ] _perhaps_ SAT solving
* [ ] _perhaps_ Sequent Proving of the form φ⊢ψ, where φ is a set of
      predicates and ψ is the conclusion which we want to prove.
