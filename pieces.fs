require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

object class
  destruction implementation
  selector clear-board
  protected
  inst-value x-max
  inst-value y-max
  inst-value z-max
  cell% inst-var board-a
  cell% inst-var board-bytes
  cell% inst-var board-bits
  public
  m: ( voxel-board-mapping -- ) \ destructor
    board-a @ free throw  ;m overrides destruct
  m: ( uxmax uymax uzmax voxel-board-mapping -- )
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
  m: ( voxel-board-mapping -- uxmax uymax uxmax ) \ return the board dimentions
    x-max y-max z-max ;m method get-board-dims
  m: ( ux uy uz voxel-board-mapping -- )
    z-max y-max * *
    swap x-max * + + dup 8 / { ubit ubyte }
    board-a @ ubyte + c@
    1 ubit ubyte 8 * - lshift or
    board-a @ ubyte + c!
  ;m method set-board-voxel
  m: ( voxel-board-mapping -- uboard-> ubytes )
    board-a @ board-bytes @
  ;m method get-board
  m: ( voxel-board-mapping -- )
    board-a @ board-bytes @ erase
  ;m overrides clear-board
end-class voxel-board-mapping

create puzzle-board-dimensions voxel-board-mapping dict-new drop

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

voxel-board-mapping class
\ this class has methods to take pieces object and create all the translated pieces in a link list of board images
\ the data for this object is collected from puzzle-pieces and puzzle-board-dimensions objects
  destruction implementation
  protected
  inst-value pieces->
  inst-value translated-pieces->
  public
  m: ( translated-pieces -- )
    puzzle-pieces [to-inst] pieces->
    double-linked-list heap-new [to-inst] translated-pieces->
    puzzle-board-dimensions get-board-dims this set-board-dims
  ;m overrides construct
  m: ( translated-pieces -- ) \ destructor
    this destruct
    translated-pieces-> destruct
  ;m overrides destruct
end-class translate-pieces

create translated-pieces translate-pieces dict-new drop
translated-pieces get-board-dims . . . cr
