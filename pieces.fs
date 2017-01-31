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

include ./puzzle.def
