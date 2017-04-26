require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs

\ 0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
\ constant ref-piece-list                                       \ this is the reference list of piece`s created above
\ ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  protected
  cell% inst-var hole-array     \ address of the array that holds the hole reference lists
  cell% inst-var hole-size      \ the size of the hole-array in terms of how many holes or voxel
  cell% inst-var a-ref-piece-array  \ the upiece-array object passed to constructor used as the reference
  cell% inst-var the-puzzle-board   \ the board oject that is passed to construct used to size the hole-array and hole-size values

  m: ( hole-array-piece-list -- ) \ populate the hole-array with lists of reference pieces that fit in each hole
    the-puzzle-board @ [bind] board get-board-dims
    0 ?do \ z
      0 ?do \ y
        0 ?do \ x
          i j k \ x y z
        loop
      loop
    loop
  ;m method populate-holes
  public
  m: ( upiece-array uboard hole-array-piece-list -- ) \ constructor
    \ takes upiece-array that should contain the reference pieces and oranizes them for hole indexing or voxel indexing
    \ uboard should be puzzle-board that contains the size of the current puzzle being solved for
    the-puzzle-board !
    a-ref-piece-array !
    the-puzzle-board @ [bind] board get-board-dims * * hole-size !
    hole-size @ 1 multi-cell-array heap-new hole-array !
    hole-size @ 0 ?do
      double-linked-list heap-new i hole-array @ [bind] multi-cell-array cell-array!
    loop
    this populate-holes
  ;m overrides construct

  m: ( hole-array-piece-list -- ) \ destructor
  ;m overrides destruct

  m: ( uhole-index hole-array-piece-list -- ux uy uz )

  ;m method hole-location@

  m: ( uhole-index hole-array-piece-list -- upiece nflag )
    \ uhole-index is the reference index indicating all the holes or voxels identified from puzzle-board

  ;m method next-piece-in-hole@

  m: ( uhole-index hole-array-piece-list -- uhole-list-quantity )
    hole-array @ [bind] multi-cell-array cell-array-dimensions@ drop
  ;m method hole-list-quantity@

  m: ( hole-array-piece-list -- uhole-size )
    hole-size @ ;m method hole-amounts@
end-class hole-array-piece-list
