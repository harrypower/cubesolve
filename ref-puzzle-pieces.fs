require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  protected
  cell% inst-var number-buffer  \ a place to put a number from stack to retrieve it simply
  cell% inst-var hole-array     \ address of the array that holds the hole reference lists
  cell% inst-var a-ref-piece-array  \ the upiece-array object passed to constructor used as the reference
  cell% inst-var the-puzzle-board   \ the board oject that is passed to construct used to size the hole-array and hole-size values

  m: ( hole-array-piece-list -- uholex uholey uholez ) \ return hole address max values
    the-puzzle-board @ [bind] board get-board-dims
  ;m method hole-address@

  m: ( uref-piece uholex uholey uholez hole-array-piece-list -- ) \ store uref-piece into hole list at uholex uholey uholez address
    hole-array @ [bind] multi-cell-array cell-array@
    swap number-buffer ! number-buffer swap cell swap
    [bind] double-linked-list ll!
  ;m method next-piece-in-hole!

  m: ( uholex uholey uholez hole-array-piece-list -- )
    0 { uholex uholey uholez upiece }
    a-ref-piece-array @ [bind] piece-array quantity@ 0 ?do
      i a-ref-piece-array @ [bind] piece-array upiece@ to upiece
      upiece [bind] piece voxel-quantity@ 0 ?do
        i upiece [bind] piece get-voxel ( ux uy uz )
        uholez = swap uholey = and swap uholex = and true =
        if
          j uholex uholey uholez this next-piece-in-hole!
        then
      loop
    loop
  ;m method do-populate-holes

  m: ( hole-array-piece-list -- ) \ populate the hole-array with lists of reference pieces that fit in each hole
    this hole-address@
    { ux uy uz }
    uz 0 ?do \ z
      uy 0 ?do \ y
        ux 0 ?do \ x
          i j k \ x y z
          this do-populate-holes
        loop
      loop
    loop
  ;m method populate-holes

  public
  m: ( upiece-array uboard hole-array-piece-list -- ) \ constructor
    \ takes upiece-array that should contain the reference pieces and organizes them for hole indexing or voxel indexing
    \ uboard should be puzzle-board that contains the size of the current puzzle being solved for
    the-puzzle-board !
    a-ref-piece-array !
    this hole-address@ 3 multi-cell-array heap-new hole-array !
    this hole-address@
    { ux uy uz }
    uz 0 ?do \ z
      uy 0 ?do \ y
        ux 0 ?do \ x
          double-linked-list heap-new i j k hole-array @ [bind] multi-cell-array cell-array!
        loop
      loop
    loop
    this populate-holes
  ;m overrides construct

  m: ( hole-array-piece-list -- ) \ destructor
  ;m overrides destruct

  m: ( uholex uholey uholez hole-array-piece-list -- uref-piece nflag )
    \ uholex uholey uholez is the hole address identified from puzzle-board
    \ uref-piece is the next reference piece in the list that fits in that hole address
    \ nflag is false normaly after upiece retrieval
    \ nflag is true when for givin hole address the piece list is at the end
    \ note when nflag is true the piece list at that hole address will reset to begining of list
    hole-array @ [bind] multi-cell-array cell-array@ { uobject }
    uobject [bind] double-linked-list ll@> ( uaddr usize nflag -- )
    true = if
      uobject [bind] double-linked-list ll-set-start
      number-buffer swap move
      number-buffer @
      true
    else
      number-buffer swap move
      number-buffer @
      false
    then ( uref nflag -- )
  ;m method next-ref-piece-in-hole@

  m: ( uholex uholey uholez hole-array-piece-list -- uhole-list-quantity ) \ returns the quantity of pieces in a given hole
    hole-array @ [bind] multi-cell-array cell-array@
    [bind] double-linked-list ll-size@
  ;m method hole-list-quantity@

  m: ( hole-array-piece-list -- uholex uholey uholez )  \ returns the total hole addresses for this reference puzzle passed to construct
    this hole-address@ ;m method hole-max-address@
end-class hole-array-piece-list

\ ***************************************************************************************************************************************
\\\
ref-piece-array puzzle-board hole-array-piece-list heap-new constant testapl
0 0 0 testapl next-ref-piece-in-hole@ .s cr
testapl hole-max-address@ .s cr

0 0 0 testapl hole-list-quantity@ .s cr

: seeallholes ( -- )
  testapl hole-max-address@
  { ux uy uz }
  ux 0 ?do
    uy 0 ?do
      uz 0 ?do
        k j i testapl hole-list-quantity@ . space
        k . ."  x " j . ." y " i . ." z" cr
      loop
    loop
  loop
;

seeallholes
