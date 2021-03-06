require ./chain-ref.fs
require ./newpuzzle.def \ definition of the puzzle to be solved
require ./allpieces.fs  \ to make all the pieces
require ./voxel-ref-list.fs

." First setup all the puzzle basic data!" cr
0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list chain-ref heap-new constant chain-ref-array    \ this object takes reference list from above and makes chain ref array of that data
chain-ref-array fast-puzzle-board heap-new constant the-board \ the main board object for viewing the puzzle ... note it needs the reference created above
chain-ref-array hole-array-piece-list heap-new constant voxel-ref-list
\ the-board and chain-ref-array and voxel-ref-list now have all the tools to work on solving the puzzle

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

: voxel-blocked? ( nvoxelindex -- nflag ) \ test nvoxelindex for a blocked path
\ nflag is true if nvoxelindex does have a blockage
\ nflag is false if no blockage was found
  false { nvoxelindex nflag }
  nvoxelindex voxel-ref-list [bind] hole-array-piece-list reset-A-piece-list
  begin
    nvoxelindex voxel-ref-list [bind] hole-array-piece-list index>xyz  \ ( 0 30 at-xy ) .s ."  < the xyz location of nvoxelindex" cr
    voxel-ref-list [bind] hole-array-piece-list next-ref-piece-in-hole@  \ ( 0 31 at-xy ) .s ."  < the reference and flag for the xyz location " cr
    false = if \ false not at end
      the-board [bind] fast-puzzle-board board-piece? ( dup ) true = if false to nflag true else false then
      \ swap . ."  < true is no blockage bail and false is blockage try again .. at middle" cr
    else \ at end
      the-board [bind] fast-puzzle-board board-piece? ( dup ) true = if false to nflag true else true to nflag true then
      \ swap . ."  < true is no blockage bail and false is blockage try again .. at end " cr
    then
  until
  nflag ;

: blocked-board? ( -- nflag ) \ look at current board and see if there is any blocked parts that will not allow a piece
\ nflag is true if there is a block on the current board
\ nflag is false if no block found
  try
    the-board [bind] fast-puzzle-board max-board-index@ 0 ?do
      i the-board [bind] fast-puzzle-board nvoxel@ true = if \ only test if the board has a hole at the voxel location not if there is a piece there currently
        i voxel-blocked? throw  \ if this board voxel is blocked it will throw with a true else it will do nothing
      then
    loop
    false
  restore
  endtry ;


defer next-chain'
defer remove-too?'
defer remove-marker'
0 value display-loop
20000 value max-display-loop
0 value max-solution

: see-data ( -- ) \ to see the puzzle and testing
  display-loop max-display-loop >= if
    the-board [bind] fast-puzzle-board output-board
    the-board [bind] fast-puzzle-board board-pieces@ 0 ?do
      30 i at-xy i the-board [bind] fast-puzzle-board nboard-piece@ . ."  < " i .
    loop
    30 26 at-xy the-board [bind] fast-puzzle-board board-pieces@ . ." < current total pieces!  "
    30 27 at-xy max-solution . ." < max-solution at this moment"
    0 34 at-xy remove-too?' . ." < remove-too? "
    0 35 at-xy remove-marker' . ." < remove-marker"
    0 36 at-xy next-chain' . ." < next chain "
    0 37 at-xy .s  ." < stack"  ( pause-for-key ) key-test-wait drop
    0 to display-loop
  else
    display-loop 1 + to display-loop
  then ;

: update-max ( ncurrentsize -- ) \ update max-solution to show the farthest it has gotten
  max-solution max to max-solution ;

0 value next-chain
' next-chain is next-chain'
: next-chain-piece! ( -- nchain-end? nboard-placed? ) \ get next chain for last piece and place on board
  \ nboard-placed? is true when chain for last piece was placed on board successfully
  \ nboard-placed? is false when chain for last piece was not placed on board successfully
  \ nchain-end? is true when chain list for last board piece is at end
  \ nchain-end? is false when chain list for last board piece is not at end
  \ note this will always work provided there is at least one piece on the board
  the-board [bind] fast-puzzle-board board-pieces@ 1 - \ get last piece index
  the-board [bind] fast-puzzle-board nboard-piece@ \ get last piece
  chain-ref-array [bind] chain-ref next-chain@ swap \ get next chain for last piece
  dup to next-chain
  the-board [bind] fast-puzzle-board board-piece! \ put next piece on board
  true = if
    blocked-board? true = if \ if blocked then remove this last piece
      the-board [bind] fast-puzzle-board remove-last-piece
      false
    else
      true
    then
  else
    false
  then
  \ if true the chain for uref was placed on the board
  \ if false the chain for uref was not placed on the board
  see-data ;

false value remove-too?
' remove-too? is remove-too?'
0 value remove-marker
' remove-marker is remove-marker'
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
        remove-too? true = if
          \ not needed maybe ... because i am going to a marked spot not a counter value
        else
          nchain-end? if
            true to remove-too?
            the-board [bind] fast-puzzle-board board-pieces@ dup update-max 1 - to remove-marker
          then
        then
        \ here i need to reset the chain list for this piece just placed to start at its begging
        the-board [bind] fast-puzzle-board board-pieces@ dup update-max 1 - \ get last piece index
        the-board [bind] fast-puzzle-board nboard-piece@ \ get last piece
        chain-ref-array [bind] chain-ref nchain-reset \ reset the chain list for the last piece
        false
      then
    else
      nchain-end? if
        the-board [bind] fast-puzzle-board remove-last-piece
        remove-too? if
            false to remove-too?
            begin
              the-board [bind] fast-puzzle-board remove-last-piece
              the-board [bind] fast-puzzle-board board-pieces@ remove-marker <
            until
          then
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
    max-display-loop to display-loop see-data
    solved?
    \ should reset the chain link list for next piece if not solved yet to ensure the start of the list
  ;

\ ***********************************************************************************************************************************************************
\ \\\
: test-block ( -- )
  the-board [bind] fast-puzzle-board clear-board
  0 the-board [bind] fast-puzzle-board board-piece! drop
  1 the-board [bind] fast-puzzle-board board-piece! drop
  2 the-board [bind] fast-puzzle-board board-piece! drop
  3 the-board [bind] fast-puzzle-board board-piece! drop
  4 the-board [bind] fast-puzzle-board board-piece! drop
  73 the-board [bind] fast-puzzle-board board-piece! drop
  12 the-board [bind] fast-puzzle-board board-piece! drop
  11 the-board [bind] fast-puzzle-board board-piece! drop
  10 the-board [bind] fast-puzzle-board board-piece! drop
  325 the-board [bind] fast-puzzle-board board-piece! drop
  19 the-board [bind] fast-puzzle-board board-piece! drop
  34 the-board [bind] fast-puzzle-board board-piece! drop
  38 the-board [bind] fast-puzzle-board board-piece! drop
  see-data ;

\ make test word that places a piece on board one at a time at each voxel location using voxel-ref-list to confirm voxel addressing of voxel-ref-list verses fast-puzzle-board voxel addressing
