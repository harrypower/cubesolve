require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./Gforth-Objects/object-struct-helper.fs

struct-base class
  protected
  struct
    cell% field length
    cell% field width
    cell% field height
  end-struct board-dims%
  public
  m: ( puzzle-board -- ) \ constructor
    1 board-dims% this [parent] construct ;m overrides construct
  m: ( ulength uwidth uheight puzzle-board -- ) \ store the dimensions
    0 this :: height !
    0 this :: width !
    0 this :: length ! ;m method dims!
  m: ( puzzle-board -- ulength uwidth uheight ) \ retrieve the dimensions
    0 this :: length @
    0 this :: width @
    0 this :: height @ ;m method dims@
  m: ( puzzle-board -- uvoxels ) \ total number of voxels the board contains
    0 this :: length @ 0 this :: width @ * 0 this :: height @ * ;m method voxel-qty
end-class puzzle-board

create puzzle-board-dimensions puzzle-board dict-new drop

struct-base class
  destruction implementation
  protected
  struct
    char% field voxel-l
    char% field voxel-w
    char% field voxel-h
  end-struct temp-voxel%
  cell% inst-var piece-list
  cell% inst-var voxel-list
  public
  m: ( pieces -- ) \ constructor Note memory allocated here so call destruct before calling this construct or memory leaks will happen
    double-linked-list heap-new piece-list !
    double-linked-list heap-new voxel-list !
    1 temp-voxel% this [parent] construct
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
      this [parent] destruct
    then ;m overrides destruct
  m: ( pieces -- ) \ add voxel list to the current piece in the piece-list
    voxel-list cell
    piece-list @ ll!
    double-linked-list heap-new voxel-list !
  ;m method define
  m: ( pieces -- uquantity ) \ return quantity of pieces
    piece-list @ ll-size@ ;m method piece-quantity
  m: ( ul uw uh pieces -- ) \ add voxel to voxel-list
    0 this :: voxel-h c!
    0 this :: voxel-w c!
    0 this :: voxel-l c!
    0 this :: size @ voxel-list @ ll!
  ;m method add-voxel
  m: ( uindex pieces -- ul uw uh ) \ retrieve voxel from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll@ drop
    dup >r voxel-l c@
    r@ voxel-w c@
    r> voxel-h c@ ;m method getvoxel
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
