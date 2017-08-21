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
  inst-value umax-x
  inst-value umax-y
  inst-value umax-z

  m: ( hole-array-piece-list -- uholex uholey uholez ) \ return hole address max values
    umax-x umax-y umax-z ;m method hole-address@

  m: ( uref-piece uholex uholey uholez hole-array-piece-list -- ) \ store uref-piece into hole list at uholex uholey uholez address
    hole-array @ [bind] multi-cell-array cell-array@
    [bind] double-linked-list ll-cell!
  ;m method next-piece-in-hole!

  m: ( a-ref-piece-array uholex uholey uholez hole-array-piece-list -- )
    0 { a-ref-piece-array uholex uholey uholez upiece }
    a-ref-piece-array [bind] piece-array quantity@ 0 ?do
      i a-ref-piece-array [bind] piece-array upiece@ to upiece
      upiece [bind] piece voxel-quantity@ 0 ?do
        i upiece [bind] piece get-voxel ( ux uy uz )
        uholez = swap uholey = and swap uholex = and true =
        if
          j uholex uholey uholez this next-piece-in-hole!
        then
      loop
    loop ;m method do-populate-holes

  m: ( a-ref-piece-array hole-array-piece-list -- ) \ populate the hole-array with lists of reference pieces that fit in each hole
    this hole-address@
    { a-ref-piece-array ux uy uz }
    uz 0 ?do \ z
      uy 0 ?do \ y
        ux 0 ?do \ x
          a-ref-piece-array i j k ( a-ref-piece-array x y z )
          this do-populate-holes
        loop
      loop
    loop ;m method populate-holes

  public
  m: ( upiece-array uboard hole-array-piece-list -- ) \ constructor
    \ takes upiece-array that should contain the reference pieces and organizes them for hole indexing or voxel indexing
    \ uboard should be puzzle-board that contains the size of the current puzzle being solved for
    \ upiece-array and uboard objects are not stored here or modified just data taken from them!
    [bind] board get-board-dims [to-inst] umax-z [to-inst] umax-y [to-inst] umax-x
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
    this hole-address@
    { ux uy uz }
    uz 0 ?do \ z
      uy 0 ?do \ y
        ux 0 ?do \ x
          i j k hole-array @ [bind] multi-cell-array cell-array@
          dup [bind] double-linked-list destruct
          free throw
        loop
      loop
    loop
    hole-array @ [bind] multi-cell-array destruct
    hole-array @ free throw
  ;m overrides destruct

  m: ( uholex uholey uholez hole-array-piece-list -- uref-piece nflag )
    \ uholex uholey uholez is the hole address identified from puzzle-board
    \ uref-piece is the next reference piece in the list that fits in that hole address
    \ nflag is false normaly after upiece retrieval
    \ nflag is true when for given hole address the piece list is at the end
    \ note when nflag is true the piece list at that hole address will reset to begining of list
    hole-array @ [bind] multi-cell-array cell-array@ { uobject }
    uobject [bind] double-linked-list ll-cell@  ( nref )
    uobject [bind] double-linked-list ll> ( nref nflag )
    dup true = if uobject [bind] double-linked-list ll-set-start then ( nref nflag )
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
\ 0 0 0 testapl next-ref-piece-in-hole@ .s cr
\ testapl hole-max-address@ .s cr

\ 0 0 0 testapl hole-list-quantity@ .s cr

board heap-new constant testboard

: keypause ( -- )
  begin key? until key drop ;

: seeholeonboard ( ux uy uz -- )
  testapl next-ref-piece-in-hole@ drop
  ref-piece-array [bind] piece-array upiece@
  testboard place-piece-on-board drop
  testboard see-board ;

: putholeonboard { ux uy uz -- }
  ux uy uz testapl hole-list-quantity@ 0 ?do
    puzzle-board [bind] board get-board-dims
    testboard [bind] board destruct
    testboard [bind] board construct
    testboard [bind] board set-board-dims
    ux uy uz seeholeonboard
    0 40 at-xy i . ." < index of >" ux . uy . uz .
    keypause
  loop ;

 2 2 2 putholeonboard

\\\
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

testapl destruct
