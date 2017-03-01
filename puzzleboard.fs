require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs

object class
  destruction implementation
  selector set-board-dims
  protected
  struct
    cell% field board-cell
  end-struct board-cell%
  cell% inst-var board-array        \ pointer to cell sized array that contains index #'s refering to pieces in the board-pieces-list
  cell% inst-var board-pieces-list  \ pointer to a pieces objects that contain piece objects that are currently in the board-array
\  cell% inst-var a-voxel            \ pointer to a voxel object
\  cell% inst-var a-piece            \ pointer to a piece object
  inst-value x-max                  \ values containing the x y and z max board dimensions
  inst-value y-max
  inst-value z-max
  inst-value z-mult                 \ calculated value for the z multiplier used to find board-array memory location
  m: ( ux uy uz board -- uaddr ) \ calculated board-array address
    z-mult * swap x-max * + + board-cell% %size * board-cell board-array @ + ;m method calc-board-array
    \ z-mult * swap y-max * + + cell * board-array @ + ;m method calc-board-array
  m: ( uvalue ux uy uz board -- ) \ place uvalue into the board-array at location ux uy uz
    this calc-board-array ! ;m method board-array!
  m: ( ux uy uz board -- uvalue ) \ retrieve uvalue from board-array at location ux uy uz
    this calc-board-array @ ;m method board-array@
  m: ( upiece board -- ) \ add upiece to board piece list
    board-pieces-list @ [bind] pieces add-a-piece ;m method board-pieces!
  m: ( uindex board -- upiece ) \ get uindex piece from board list
    board-pieces-list @ [bind] pieces get-a-piece ;m method board-pieces@
  public
  m: ( board -- ) \ constructor
    pieces heap-new board-pieces-list !
    0 0 0 this set-board-dims
  ;m overrides construct
  m: ( board -- ) \ destructor
    board-array @ free throw
    board-pieces-list @ [bind] pieces destruct
    board-pieces-list @ free throw
  ;m overrides destruct
  m: ( ux uy uz board -- ) \ set max board size and allocate the board-array memory
    [to-inst] z-max [to-inst] y-max [to-inst] x-max
    x-max y-max * z-max * board-cell% %size * allocate throw board-array !
    x-max y-max * [to-inst] z-mult
    x-max 0 ?do
      y-max 0 ?do
        z-max 0 ?do
          true k j i this board-array! \ place true into array to show no pieces
        loop
      loop
    loop ;m overrides set-board-dims
  m: ( board -- ux-max uy-max uz-max ) \ get dimensions of this board
    x-max y-max z-max ;m method get-board-dims
  m: ( board -- uquantity ) \ return how many pieces are currently on the board
    board-pieces-list @ [bind] pieces pieces-quantity@ ;m method board-piece-quantity@
  m: ( ux uy uz board -- nflag ) \ ux uy uz is a voxel to test if it can be placed on an empty board
    \ nflag is true if ux uy uz can be on the board
    \ nflag is false if ux uy uz can not be on the board
    dup z-max < swap 0 >= and swap
    dup y-max < swap 0 >= and and swap
    dup x-max < swap 0 >= and and ;m method voxel-on-board?
  m: ( upiece board -- nflag ) \ test if upiece can be placed on an empty board nflag is true if piece can be placed false if not
    { upiece } true
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel this voxel-on-board? and
    loop ;m method piece-on-board?
  m: ( upiece board -- nflag ) \ test if upiece could be placed on the current populated board
    \ nflag is true if upiece can be placed on the current board
    \ nflag is flase if upiece could not be placed on current board due to a piece intersection or a board boundry issue.
    { upiece }
    upiece this piece-on-board? true = if
      this board-piece-quantity@ 0= if
        true
      else
        0 this board-piece-quantity@ 0 ?do
          i this board-pieces@
          upiece [bind] piece intersect? or
        loop
        invert
      then
    else
      false
    then ;m method piece-on-this-board?
  m: ( upiece board -- nflag ) \ place upiece on the current board if it can be placed without intersecting with other pieces
    \ nflag is true if upiece was place on the board
    \ nflag is false if upiece either intersected with another piece or exceded the board boundrys
    dup this piece-on-this-board? true = if
        \ dup \ need to update the board array here 
        this board-pieces! true
    else
      drop false
    then ;m method place-piece-on-board
  m: ( uindex board -- upiece ) \ retrieve uindex piece from this board in the form of a piece object
    this board-pieces@ ;m method nget-board-piece
\  m: ( ux uy uz board -- uvalue ) \ test word to see board numbers at ux uy uz
\    this board-array@ ;m method see-board-x
\  m: ( uvalue ux uy uz board -- ) \ test word to uvalue board number at ux uy uz
\    this board-array! ;m method set-board-x
end-class board

board heap-new constant puzzle-board

include ./newpuzzle.def

puzzle-pieces pieces-quantity@ . ." quantity" cr
puzzle-board board-piece-quantity@ . ." b qnt " cr
0 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 0" cr
1 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 1" cr
2 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 2" cr
3 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 3" cr
4 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 4" cr
5 puzzle-pieces get-a-piece puzzle-board place-piece-on-board . ."  piece 5" cr
puzzle-board board-piece-quantity@ . ." board pieces" space .s cr

\\\
puzzle-pieces pieces-quantity@ . ." pieces" space .s cr
puzzle-board board-piece-quantity@ . ." board pieces" space .s cr
0 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 0" cr
1 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 1" cr
2 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 2" cr
3 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 3" cr
4 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 4" cr
5 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 5" cr

0 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 0" cr
1 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 1" cr
2 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 2" cr
3 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 3" cr
4 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 4" cr
5 puzzle-pieces get-a-piece puzzle-board piece-on-this-board? . ."  piece 5" cr

\\\
0 0 0 puzzle-board voxel-on-board? . .s cr
4 4 4 puzzle-board voxel-on-board? . .s cr
5 5 5 puzzle-board voxel-on-board? . .s cr
-1 -1 0 puzzle-board voxel-on-board? . .s cr

\\\
board heap-new constant testboard
cr
: seetheboard
  5 0 ?do
    5 0 ?do
      5 0 ?do
        i j k testboard see-board-x . ." value " i . ." x " j . ." y " k . ." z " .s cr
      loop
    loop
  loop ;

5 5 5 testboard set-board-dims
seetheboard
testboard get-board-dims . . . cr

256 0 0 0 testboard set-board-x
0 0 0 testboard see-board-x . cr
1 0 0 testboard see-board-x . cr .s cr
92392323 3 1 4 testboard set-board-x
234 0 2 4 testboard set-board-x
1932 2 0 1 testboard set-board-x
1984 3 3 3 testboard set-board-x
2016 4 4 4 testboard set-board-x
seetheboard
.s cr
