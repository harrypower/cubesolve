require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

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

require ./puzzle.def

\\\
object class
  destruction implementation
  protected
  inst-value total-voxels
  struct
    char% field voxel-x
    char% field voxel-y
    char% field voxel-z
  end-struct temp-voxel%
  cell% inst-var piece-list
  cell% inst-var voxel-list

  public
  m: ( uvoxelsize translated-pieces -- ) \ constructor Note memory allocated here so call destruct before calling this construct or memory leaks will happen
    dup [to-inst] total-voxels
    8 / aligned dup
    8 * total-voxels < if drop total-voxels 8 / 1 + aligned then
    dup
    double-linked-list heap-new piece-list !
    double-linked-list heap-new voxel-list !
    1 temp-voxel% this [parent] construct
  ;m overrides construct
end-class translated-pieces
