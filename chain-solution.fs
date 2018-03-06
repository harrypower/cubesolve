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
  ." start of do-chain-solution" .s cr pause-for-key drop
  false { uref chain-end? }
  uref chain-ref-array [bind] chain-ref next-chain@ to chain-end? to uref
  \ if true then this is the last chain for uref
  \ if false then there are more chains for uref
  uref the-board [bind] fast-puzzle-board board-piece!
  \ if true the chain for uref was placed on the board
  \ if false the chain for uref was not placed on the board
  the-board [bind] fast-puzzle-board output-board cr
  chain-end? . ." < chain-end?" cr
  uref . ." < uref" cr
  .s ." after piece placed on board!" cr pause-for-key drop
  true = if  \ piece placed on board
    solved? false = if \ no solution
      chain-end? false = if \ in the middle of a chain
          uref do-chain-solution
        else \ at end of a chain
          \ this might be the incorrect thing to do as it will cause do-chain-solution to return false and that causes start-chain-solution to loop
          \ but the end of the chain may not be the chain that is the first piece place..!!!
          false
        then
    else \ solution found
      true
    then
  else \ piece not placed on board
    chain-end? false = if \ in middle of chain
      uref do-chain-solution
    else \ at end of chain ... need to back up 
      \ solved?
      the-board [bind] fast-puzzle-board board-pieces@ 1 -
      the-board [bind] fast-puzzle-board nboard-piece@
      the-board [bind] fast-puzzle-board remove-last-piece
      do-chain-solution
    then
  then
  .s ." after all conditions" cr pause-for-key drop
;

: start-chain-solution ( -- nflag )  \ start the chain solving and return true when solution in in the-board or return false when failed
  try
    chain-ref-array [bind] chain-ref quantity@ 0 ?do
      the-board [bind] fast-puzzle-board clear-board  \ start with clean board
      i the-board [bind] fast-puzzle-board board-piece! drop \ first piece on board should always work
      i do-chain-solution throw \ now for the rest of the pieces
      .s ." after do-chain-solution" cr
    loop
    false \ after going through all the reference pieces and not finding a chain solution bail with false
  restore
  endtry
;
