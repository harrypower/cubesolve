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
  inst-value x-max                  \ values containing the x y and z max board dimensions
  inst-value y-max
  inst-value z-max
  inst-value z-mult                 \ calculated value for the z multiplier used to find board-array memory location
  inst-value x-display-size
  inst-value y-display-size
  inst-value z-display-size
  m: ( ux uy uz board -- uaddr ) \ calculated board-array address
    z-mult * swap x-max * + + board-cell% %size * board-cell board-array @ + ;m method calc-board-array
  m: ( uvalue ux uy uz board -- ) \ place uvalue into the board-array at location ux uy uz
    this calc-board-array ! ;m method board-array!
  m: ( ux uy uz board -- uvalue ) \ retrieve uvalue from board-array at location ux uy uz
    this calc-board-array @ ;m method board-array@
  m: ( uvalue upiece board -- ) \ place voxels of upiece onto board-array with the uvalue as a upiece reference
    { uvalue upiece }
    upiece [bind] piece voxel-quantity@ 0 ?do
      uvalue i upiece [bind] piece get-voxel this board-array!
    loop
  ;m method piece-to-board-array!
  m: ( upiece board -- ) \ add upiece to board piece list
    board-pieces-list @ [bind] pieces add-a-piece ;m method board-pieces!
  m: ( uindex board -- upiece ) \ get uindex piece from board list
    board-pieces-list @ [bind] pieces get-a-piece ;m method board-pieces@
  public
  m: ( board -- ) \ constructor
    pieces heap-new board-pieces-list !
    0 0 0 this set-board-dims
    6 [to-inst] x-display-size
    1 [to-inst] y-display-size
    x-display-size y-display-size * [to-inst] z-display-size
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
    try
    dup z-max < swap 0 >= and invert throw
    dup y-max < swap 0 >= and invert throw
    dup x-max < swap 0 >= and invert throw
    false
    restore if drop drop drop false else true then
    endtry
  ;m method voxel-on-board?
  m: ( upiece board -- nflag ) \ test if upiece can be placed on an empty board nflag is true if piece can be placed false if not
    { upiece }
    try
      upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel this voxel-on-board? invert throw
    loop
    false
    restore invert
    endtry
  ;m method piece-on-board?
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
        dup this board-piece-quantity@ swap this piece-to-board-array!
        this board-pieces! true
    else
      drop false
    then ;m method place-piece-on-board
  m: ( uindex board -- upiece ) \ retrieve uindex piece from this board in the form of a piece object
    this board-pieces@ ;m method nget-board-piece
  m: ( board -- ) \ crude terminal board display
    page
    z-max 0 ?do
      y-max 0 ?do
        x-max 0 ?do
          i x-display-size * j y-display-size * k z-display-size * + at-xy
          i j k this board-array@ dup true = if drop ." *****" else u. then
        loop
      loop
    loop
  ;m method see-board
end-class board

board heap-new constant puzzle-board

require ./newpuzzle.def

\ **********************************************************************************************************************
