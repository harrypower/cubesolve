require ./chain-ref.fs
require ./newpuzzle.def \ definition of the puzzle to be solved
require ./allpieces.fs  \ to make all the pieces
require ./voxel-ref-list.fs
require ./group-lists.fs
require ./Gforth-Objects/mdca-obj.fs

." First setup all the puzzle basic data!" cr
0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list chain-ref heap-new constant chain-ref-array    \ this object takes reference list from above and makes chain ref array of that data
chain-ref-array fast-puzzle-board heap-new constant the-board \ the main board object for viewing the puzzle ... note it needs the reference created above
chain-ref-array hole-array-piece-list heap-new constant voxel-ref-list

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

2 group-lists heap-new constant p-assembly

: pop-ppassembly ( -- ) \ populate the piece pair assembly
  chain-ref-array [bind] chain-ref quantity@ 0 ?do
    i chain-ref-array [bind] chain-ref chain-quantity@ 0 ?do
      j j chain-ref-array [bind] chain-ref next-chain@ drop
      p-assembly [bind] group-lists group!
    loop
  loop ;
pop-ppassembly

0 value p-assy-array

: make-pparray ( -- )
  p-assembly [bind] group-lists group-dims@ 2dup  2
  multi-cell-array heap-new to p-assy-array
  drop 0 ?do
    p-assembly [bind] group-lists group@> 2drop
    i 0 .s ." before storage" cr p-assy-array [bind] multi-cell-array cell-array!
    i 1 .s ." before storage" cr p-assy-array [bind] multi-cell-array cell-array!
  loop ;

\ make-pparray
\ p-assy-array bind multi-cell-array cell-array-dimensions@ .s
