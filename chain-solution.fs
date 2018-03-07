require ./chain-ref.fs
require ./newpuzzle.def \ definition of the puzzle to be solved
require ./allpieces.fs  \ to make all the pieces

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list chain-ref heap-new constant chain-ref-array    \ this object takes reference list from above and makes chain ref array of that data
chain-ref-array fast-puzzle-board heap-new constant the-board \ the main board object for viewing the puzzle ... note it needs the reference created above
\ the-board and chain-ref-array now have all the tools to work on solving the puzzle

: pause-for-key ( -- nkey ) \ halt until a key is pressed then continue nkey is the key pressed when this word returns
  begin key? until
  key 10 ms ;

: key-test-wait ( -- nkey ) \ if keyboard is pressed pause until it is pressed again
  \ the second key pressed is returned as nkey
  key?
  if
    key drop 10 ms
    pause-for-key
  else 0
  then ;

: solved? ( -- nflag ) \ looks for the total pieces on the-board for done or not
  \ nflag is true if puzzle solved
  \ nflag is false if puzzle is not solved
  the-board [bind] fast-puzzle-board max-board-pieces@
  the-board [bind] fast-puzzle-board board-pieces@ = ;

: do-chain-solution ( uref -- nflag ) \ uref is the chain array index to work on for solution ... return nflag true if solution is found false is uref did not solve it
  recursive
  0 35 at-xy ." start " .s key-test-wait drop
  false { uref chain-end? }
  uref chain-ref-array [bind] chain-ref next-chain@ to chain-end? to uref
  \ if true then this is the last chain for uref
  \ if false then there are more chains for uref
  uref the-board [bind] fast-puzzle-board board-piece!
  \ if true the chain for uref was placed on the board
  \ if false the chain for uref was not placed on the board
  the-board [bind] fast-puzzle-board output-board cr
  30 0 at-xy the-board [bind] fast-puzzle-board board-pieces@ . ." < current pieces!  "
  0 36 at-xy chain-end? . ." < chain-end?  " 20 36 at-xy  uref . ." < uref  "
  0 37 at-xy ." placed on board? > " .s  key-test-wait drop
  true = if  \ piece placed on board
    solved? false = if \ no solution yet
      chain-end? false = if \ in the middle of a chain
          uref do-chain-solution
        else \ at end of a chain
          the-board [bind] fast-puzzle-board board-pieces@ 1 = \ if at first piece
          0 the-board [bind] fast-puzzle-board nboard-piece@
          chain-ref-array [bind] chain-ref quantity@ = and if \ and that first piece is the last chain to test
            false \ no solution found after all starting pieces used.
          else \ continue because there is more yet!
            the-board [bind] fast-puzzle-board board-pieces@ 1 -
            the-board [bind] fast-puzzle-board nboard-piece@
            the-board [bind] fast-puzzle-board remove-last-piece
            do-chain-solution
          then
        then
    else \ solution found now
      true
    then
  else \ piece not placed on board
    chain-end? false = if \ in middle of chain
      uref do-chain-solution
    else \ at end of chain ... need to back up
      the-board [bind] fast-puzzle-board board-pieces@ 1 = \ if at first piece
      0 the-board [bind] fast-puzzle-board nboard-piece@
      chain-ref-array [bind] chain-ref quantity@ = and if \ and that first piece is the last chain to test
        false \ no solution found after all starting pieces used.
      else \ continue because there is more yet!
        the-board [bind] fast-puzzle-board board-pieces@ 1 -
        the-board [bind] fast-puzzle-board nboard-piece@
        the-board [bind] fast-puzzle-board remove-last-piece
        do-chain-solution
      then
    then
  then
  0 39 at-xy ." about to return to start > " .s  key-test-wait drop
;

: start-chain-solution ( -- nflag ) \ start the chain solving and return true when solution is in the-board or false when failed
  the-board [bind] fast-puzzle-board clear-board  \ start with clean board
  0 the-board [bind] fast-puzzle-board board-piece! drop \ first piece on board should always work
  0 do-chain-solution \ now for the rest of the pieces
  0 40 at-xy ." after do-chain-solution with stack > " .s
;
