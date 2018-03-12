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

: see-data ( -- ) \ to see the puzzle and testing
  the-board [bind] fast-puzzle-board output-board
  the-board [bind] fast-puzzle-board board-pieces@ 0 ?do
    30 i at-xy i the-board [bind] fast-puzzle-board nboard-piece@ . ."  < " i .
  loop
  30 26 at-xy the-board [bind] fast-puzzle-board board-pieces@ . ." < current total pieces!  "
  \ the-board [bind] fast-puzzle-board board-pieces@ 1 - \ get last piece index
  \ dup the-board [bind] fast-puzzle-board nboard-piece@ \ get last piece
  \ swap 1 - the-board [bind] fast-puzzle-board nboard-piece@ swap
  \ 0 38 at-xy . ." < last piece placed !"
  \ 0 39 at-xy . ." < second last piece placed!"
  \ 0 37 at-xy ."                                              "
  0 37 at-xy .s  ." < stack" pause-for-key drop \ key-test-wait drop
;

: next-chain-piece! ( -- nchain-end? nboard-placed? ) \ get next chain for last piece and place on board
  \ nboard-placed? is true when chain for last piece was placed on board successfully
  \ nboard-placed? is false when chain for last piece was not placed on board successfully
  \ nchain-end? is true when chain list for last board piece is at end
  \ nchain-end? is false when chain list for last board piece is not at end
  \ note this will always work provided there is at least one piece on the board
  the-board [bind] fast-puzzle-board board-pieces@ 1 - \ get last piece index
  the-board [bind] fast-puzzle-board nboard-piece@ \ get last piece
  chain-ref-array [bind] chain-ref next-chain@ swap \ get next chain for last piece
  the-board [bind] fast-puzzle-board board-piece! \ put next piece on board
  \ if true the chain for uref was placed on the board
  \ if false the chain for uref was not placed on the board
  see-data ;

false value remove-two?
: do-solution ( -- )
  0 0 { nchain-end? nboard-placed? }
  begin
    \ next chain for last piece placed
    \ put next chain piece on board
    next-chain-piece! to nboard-placed? to nchain-end?
    \ piece placed
      \ solved? exit
      \ if not solved continue
    \ piece not placed
      \ remove last piece from board
      \ if board is empty after this last piece removal exit
      \ if board is not empty continue
    nboard-placed?
    if
      solved?
      if
        true
      else
        nchain-end? if true to remove-two? then
        \ here i need to reset the chain list for this piece just placed to start at its begging
        the-board [bind] fast-puzzle-board board-pieces@ 1 - \ get last piece index
        the-board [bind] fast-puzzle-board nboard-piece@ \ get last piece
        chain-ref-array [bind] chain-ref nchain-reset \ reset the chain list for the last piece
        false
      then
    else
      nchain-end? if
        the-board [bind] fast-puzzle-board remove-last-piece
        remove-two? if the-board [bind] fast-puzzle-board remove-last-piece false to remove-two? then 
        the-board [bind] fast-puzzle-board board-pieces@ 1 = if true else false then
      else
        false
      then
    then
  until ;

: start-main-chain ( -- nflag ) \ loops through all first pieces and trys to solve puzzle from there
  \ this will only do solution for the first piece ... i will change code later to add all the pieces once i get it all working
    the-board [bind] fast-puzzle-board clear-board
    0 the-board [bind] fast-puzzle-board board-piece! drop \ first piece on board always works
    do-solution
    solved?
    \ should reset the chain link list for next piece if not solved yet to ensure the start of the list
  ;
