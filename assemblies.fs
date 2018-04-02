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

8 group-lists heap-new constant 8-assembly

: pop-8assembly
  chain-ref-array [bind] chain-ref quantity@ 0 0 0 0 0 { qnt one two three four five }
  qnt 0 ?do \ one
    i to one
    qnt 0 ?do \ two
      i to two
      one two chain-ref-array [bind] chain-ref fast-intersect? false = if
        qnt 0 ?do \ three
          i to three
          one three chain-ref-array [bind] chain-ref fast-intersect?
          two three chain-ref-array [bind] chain-ref fast-intersect? or false = if
            qnt 0 ?do \ four
              i to four
              one four chain-ref-array [bind] chain-ref fast-intersect?
              two four chain-ref-array [bind] chain-ref fast-intersect?
              three four chain-ref-array [bind] chain-ref fast-intersect? or or false = if
                qnt 0 ?do \ five
                  i to five
                  one five chain-ref-array [bind] chain-ref fast-intersect?
                  two five chain-ref-array [bind] chain-ref fast-intersect?
                  three five chain-ref-array [bind] chain-ref fast-intersect?
                  four five chain-ref-array [bind] chain-ref fast-intersect? or or or false = if
                    qnt 0 ?do \ six
                      one i chain-ref-array [bind] chain-ref fast-intersect?
                      two i chain-ref-array [bind] chain-ref fast-intersect?
                      three i chain-ref-array [bind] chain-ref fast-intersect?
                      four i chain-ref-array [bind] chain-ref fast-intersect?
                      five i chain-ref-array [bind] chain-ref fast-intersect? or or or or false = if
                        qnt 0 ?do \ seven
                          one i chain-ref-array [bind] chain-ref fast-intersect?
                          two i chain-ref-array [bind] chain-ref fast-intersect?
                          three i chain-ref-array [bind] chain-ref fast-intersect?
                          four i chain-ref-array [bind] chain-ref fast-intersect?
                          five i chain-ref-array [bind] chain-ref fast-intersect?
                          j i chain-ref-array [bind] chain-ref fast-intersect? or or or or or false = if
                            qnt 0 ?do \ eight
                              one i chain-ref-array [bind] chain-ref fast-intersect?
                              two i chain-ref-array [bind] chain-ref fast-intersect?
                              three i chain-ref-array [bind] chain-ref fast-intersect?
                              four i chain-ref-array [bind] chain-ref fast-intersect?
                              five i chain-ref-array [bind] chain-ref fast-intersect?
                              j i chain-ref-array [bind] chain-ref fast-intersect?
                              k i chain-ref-array [bind] chain-ref fast-intersect? or or or or or or false = if
                                one two three four five k j i 8-assembly [bind] group-lists group!
                                8-assembly [bind] group-lists group-dims@ drop 0 10 at-xy . one . two . ." < current size and indexs"
                              then
                            loop
                          then
                        loop
                      then
                    loop
                  then
                loop
              then
            loop
          then
        loop
      then
    loop
  loop ;

\\\
2 group-lists heap-new constant p-assembly

: make-ppassembly ( -- ) \ populate p-assembly with pairs of reference pieces from chain-ref-array
  chain-ref-array [bind] chain-ref quantity@ 0 ?do
    i chain-ref-array [bind] chain-ref chain-quantity@ 0 ?do
      j j chain-ref-array [bind] chain-ref next-chain@ drop
      p-assembly [bind] group-lists group!
    loop
  loop ;
make-ppassembly

0 value p-assy-array
: make-pparray ( -- )
  p-assembly [bind] group-lists group-dims@ swap 2
  multi-cell-array heap-new to p-assy-array
  p-assembly [bind] group-lists group-dims@ drop 0 ?do
    p-assembly [bind] group-lists group@> 2drop
    0 i p-assy-array [bind] multi-cell-array cell-array!
    1 i p-assy-array [bind] multi-cell-array cell-array!
  loop ;
make-pparray
\ p-assembly bind group-lists group-dims@ .s
p-assembly bind group-lists destruct
2 p-assembly bind group-lists construct

\ p-assy-array bind multi-cell-array cell-array-dimensions@ .s
: make-2assembly ( -- ) \ populate p-assembly with pairs of the first order assembly from p-assy-array
  p-assy-array [bind] multi-cell-array cell-array-dimensions@ 2drop 200 / 0 ?do
    the-board [bind] fast-puzzle-board clear-board
    0 i p-assy-array [bind] multi-cell-array cell-array@
    the-board [bind] fast-puzzle-board board-piece! drop
    1 i p-assy-array [bind] multi-cell-array cell-array@
    the-board [bind] fast-puzzle-board board-piece! drop \ test pair now on board
    p-assembly [bind] group-lists group-dims@ 0 10 at-xy . . i .
    p-assy-array [bind] multi-cell-array cell-array-dimensions@ 2drop 0 ?do
      0 i p-assy-array [bind] multi-cell-array cell-array@
      the-board [bind] fast-puzzle-board board-piece? invert
      1 i p-assy-array [bind] multi-cell-array cell-array@
      the-board [bind] fast-puzzle-board board-piece? invert
      or false = if \ if false then next pair index can be added to the group
        j i p-assembly [bind] group-lists group!
      then
    loop
  loop ;

make-2assembly
p-assembly bind group-lists group-dims@ .s
