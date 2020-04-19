# Logic Parser and Transformer

Read, parse, verify and transform logic formulas.


## Status

WIP, no features work,


## Usage

By default lpt print wether the input is valid or not and which form it is in.
If no argument is passed, it tries to read from `stdin`:

```
$ lpt "~(~p\/q)/\~(p -> q)"
```


## Compiling

Requirements

* F#
* Mono, if you're on gnu/linux
* Make, because of course you're on gnu/linux
