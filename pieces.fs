require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

object class
  selector clear-board
  protected
  inst-value x-max
  inst-value y-max
  inst-value z-max
  cell% inst-var board-a
  cell% inst-var board-bytes
  cell% inst-var board-bits
  public
  m: ( uxmax uymax uzmax -- )
    [to-inst] z-max
    [to-inst] y-max
    [to-inst] x-max
    x-max y-max z-max
    * * board-bits !
    board-bits @ 8 / aligned dup
    8 * board-bits @ > if drop board-bits @ 8 / 1+ aligned then
    board-bytes !
    board-bytes @ allocate throw board-a !
    this clear-board
  ;m method set-board-dims
  m: ( ux uy uz -- )
    z-max y-max * *
    swap x-max * + + dup 8 / { ubit ubyte }
    board-a @ ubyte + c@
    1 ubit ubyte 8 * - lshift or
    board-a @ ubyte + c!
  ;m method set-board-voxel
  m: ( -- uboard-> ubytes )
    board-a @ board-bytes @
  ;m method get-board
  m: ( -- )
    board-a @ board-bytes @ erase
  ;m overrides clear-board
end-class voxel-board-mapping

\ voxel-board-mapping heap-new constant test

object class
  protected
  cell% field length
  cell% field width
  cell% field height
  public
  m: ( ulength uwidth uheight puzzle-board -- ) \ store the dimensions
    this height ! this width ! this length ! ;m method dims!
  m: ( ulength uwidth uheight puzzle-board -- ) \ store the dimensions
    this length @ this width @ this height @ ;m method dims@
  m: ( puzzle-board -- uvoxels ) \ total number of voxels the board contains
    this dims@ * * ;m method voxel-qty
end-class puzzle-board

create puzzle-board-dimensions puzzle-board dict-new drop

object class
  destruction implementation
  protected
  struct
    char% field voxel-x
    char% field voxel-y
    char% field voxel-z
  end-struct temp-voxel%
  cell% inst-var voxels
  cell% inst-var voxels-size
  cell% inst-var piece-list
  cell% inst-var voxel-list
  public
  m: ( pieces -- ) \ constructor Note memory allocated here so call destruct before calling this construct or memory leaks will happen
    double-linked-list heap-new piece-list !
    double-linked-list heap-new voxel-list !
    temp-voxel% %size dup voxels-size ! allocate throw voxels !
    voxels @ voxels-size @ erase
  ;m overrides construct
  m: ( pieces -- ) \ destructor
    piece-list @ 0 <> voxel-list @ 0 <> and if
      piece-list @ ll-set-start
      piece-list @ ll-size@ 0 ?do
        piece-list @ ll@ drop @ destruct
        piece-list @ ll@> 2drop @ free throw
      loop
      piece-list @ destruct
      piece-list @ free throw
      voxel-list @ destruct
      voxel-list @ free throw
      0 piece-list !
      0 voxel-list !
      voxels @ free throw
    then ;m overrides destruct
  m: ( pieces -- ) \ add voxel list to the current piece in the piece-list
    voxel-list cell
    piece-list @ ll!
    double-linked-list heap-new voxel-list !
  ;m method define
  m: ( pieces -- uquantity ) \ return quantity of pieces
    piece-list @ ll-size@ ;m method piece-quantity
  m: ( ux uy uz pieces -- ) \ add voxel to voxel-list
    voxels @ voxel-z c!
    voxels @ voxel-y c!
    voxels @ voxel-x c!
    voxels @ voxels-size @ voxel-list @ ll!
  ;m method add-voxel
  m: ( uindex pieces -- ux uy uz ) \ retrieve voxel from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll@ drop
    dup >r voxel-x c@
    r@ voxel-y c@
    r> voxel-z c@ ;m method getvoxel
  m: ( unindex pieces -- ) \ move voxel list pointer to next voxel from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll> if piece-list @ ll@ drop @ ll-set-start then ;m method nextvoxel
  m: ( uindex pieces -- usize ) \ return voxel quantity from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll-size@ ;m method voxel-quantity
end-class pieces

create puzzle-pieces pieces dict-new drop

include ./puzzle.def

object class
  destruction implementation
  protected
  inst-value total-voxels
  inst-value voxel-bytes
  inst-value pieces->
  inst-value board->
  public
  m: ( upieces uvoxelsize translated-pieces -- )
  \ constructor Note memory allocated here so call destruct before calling this construct or memory leaks will happen
  \ upieces is the pieces object address that contains the current puzzle pieces to work with
  \ uvoxelsize is the total voxels of the board for this puzzle
    dup [to-inst] total-voxels  \ receives uvoxelsize from stack ( is the total voxels of the board )
    8 / aligned dup
    8 * total-voxels < if drop total-voxels 8 / 1 + aligned then
    [to-inst] voxel-bytes
    [to-inst] pieces->  \ receives upieces from stack
    voxel-bytes allocate throw [to-inst] board->
  ;m overrides construct
  m: ( translated-pieces -- ) \ destructor
    board-> free
  ;m overrides destruct
end-class translated-pieces
