require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./newpuzzle.def
require ./piece-array.fs
require ./serialize-obj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -hole-array-piece-list
save-instance-data class
  destruction implementation
  selector index>xyz  ( uindex hole-array-piece-list -- uholex uholey uholez ) \ return the xyz hole address for the given uindex value
  protected
  cell% inst-var hole-array     \ address of the array that holds the hole reference lists
  inst-value umax-x
  inst-value umax-y
  inst-value umax-z
  inst-value z-mult
  inst-value y-mult

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

  m: ( nquantity hole-array-piece-list -- ) \ used by serialize-data! to restore data to object
    0 ?do
      -hole-array-piece-list this do-retrieve-inst-value
    loop ;m method serialize-inst-value-data!

  m: ( nquantity hole-array-piece-list -- ) \ used by serialize-data! to restore data to object
    0 ?do
      this do-retrieve-dnumber false = abort" index for hole list retreval not understood!"
      d>s 0 ?do
        this do-retrieve-dnumber false = abort" data for hole list retreval bad!"
        d>s j this index>xyz this next-piece-in-hole!
      loop
      save$ [bind] strings @$x s" hole-end" compare false <> abort" hole-end in hole list not found!"
    loop
  ;m method serialize-hole-index-data!

  m: ( hole-array-piece-list -- ) \ used by construct and serialize methods to create internal hole array
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
  ;m method create-hole-array
  public
  m: ( upiece-array hole-array-piece-list -- ) \ constructor
    \ takes upiece-array that should contain the reference pieces and organizes them for hole indexing or voxel indexing
    \ upiece-array object is not stored here or modified just data taken from!
    this [parent] construct
    z-puzzle-board [to-inst] umax-z y-puzzle-board [to-inst] umax-y x-puzzle-board [to-inst] umax-x
    umax-x umax-y * [to-inst] z-mult
    umax-x [to-inst] y-mult
    this create-hole-array
    this populate-holes
 ;m overrides construct

  m: ( hole-array-piece-list -- ) \ destructor
    this [parent] destruct
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

  m: ( uindex hole-array-piece-list -- ) \ reset the piece list at uindex
    this index>xyz hole-array @ [bind] multi-cell-array cell-array@
    [bind] double-linked-list ll-set-start
  ;m method reset-A-piece-list

  m: ( uholex uholey uholez hole-array-piece-list -- uhole-list-quantity ) \ returns the quantity of pieces in a given hole
    hole-array @ [bind] multi-cell-array cell-array@
    [bind] double-linked-list ll-size@
  ;m method hole-list-quantity@

  m: ( hole-array-piece-list -- uholex uholey uholez )  \ returns the total hole addresses for this reference puzzle passed to construct
    this hole-address@ ;m method hole-max-address@

  m: ( uindex hole-array-piece-list -- uholex uholey uholez ) \ return the xyz hole address for the given uindex value
    dup z-mult / dup z-mult * rot swap -
    dup y-mult / dup y-mult * rot swap -
    swap rot
  ;m overrides index>xyz

  m: ( hole-array-piece-list -- uindex ) \ return the max index size of the holes in this object
    umax-z 1 - z-mult * umax-y 1 - y-mult * + umax-x  +
  ;m method hole-max-index@

  m: ( uholex uholey uholez hole-array-piece-list -- uindex ) \ return the uindex for the given xyz hole address
    z-mult * swap y-mult * + +
  ;m method xyz>index

  m: ( uindex hole-array-piece-list -- uref-piece nflag )
    \ uindex is the hole index that will return the next uref-piece
    \ nflag is false normaly after uref-piece is retrieved from uindex hole
    \ nflag is true when for given hole index the piece list is at the end
    \ note when nflag is true the piece list at uindex hole address will be reset to begining of the list
    this index>xyz this next-ref-piece-in-hole@
  ;m method index-next-ref-piece@

  m: ( hole-array-piece-list -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
    ['] serialize-inst-value-data! this do-save-name
    5 this do-save-nnumber
    ['] umax-x this do-save-inst-value
    ['] umax-y this do-save-inst-value
    ['] umax-z this do-save-inst-value
    ['] z-mult this do-save-inst-value
    ['] y-mult this do-save-inst-value
    ['] serialize-hole-index-data! this do-save-name
    this hole-max-index@ this do-save-nnumber
    this hole-max-index@ 0 ?do
      i this index>xyz
      hole-array @ [bind] multi-cell-array cell-array@
      [bind] double-linked-list ll-size@
      this do-save-nnumber \ store the size of the data at this hole index
      i this index>xyz
      hole-array @ [bind] multi-cell-array cell-array@
      [bind] double-linked-list ll-set-start
      begin
        i this index-next-ref-piece@ swap
        this do-save-nnumber
      until
      s" hole-end" save$ [bind] strings !$x
    loop
    save$
  ;m overrides serialize-data@

  m: ( nstrings hole-array-piece-list -- ) \ nstrings contains serialized data to restore this object
    this destruct
    this [parent] construct
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this do-retrieve-data true = if d>s rot rot -hole-array-piece-list rot rot this $->method else 2drop 2drop true abort" hole inst-value data incorrect!" then
    this create-hole-array
    this do-retrieve-data true = if d>s rot rot -hole-array-piece-list rot rot this $->method else 2drop 2drop true abort" hole index data incorrect!" then
  ;m overrides serialize-data!
end-class hole-array-piece-list
' hole-array-piece-list is -hole-array-piece-list

\ ***************************************************************************************************************************************
\\\
require ./newpuzzle.def
require ./allpieces.fs
require ./fast-puzzle-board.fs
." seting up all reference data and lists!" cr
0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

ref-piece-array fast-puzzle-board heap-new constant puzzle-board

ref-piece-array hole-array-piece-list heap-new constant testapl
ref-piece-array hole-array-piece-list heap-new constant testapl2

cr
testapl hole-max-address@ . . . ." < should be 5 5 5 " cr
testapl hole-max-index@ . ." < should be 125" cr
124 testapl index>xyz .s ." < should be 4 4 4" cr
testapl xyz>index . ." < should be 124" cr
47 testapl index>xyz . . . ." < should be 1 4 2" cr
23 testapl index>xyz . . . ." < should be 0 4 3" cr
5  testapl index>xyz . . . ." < should be 0 1 0" cr
4 testapl index>xyz . . . ." < should be 0 0 4" cr
123 testapl index>xyz . . . ." < should be 4 4 3" cr
4 0 0 testapl xyz>index . ." < should be 4" cr
2 4 1 testapl xyz>index . ." < should be 47" cr
3 4 0 testapl xyz>index . ." < should be 23" cr cr

strings heap-new constant atemp$
testapl bind hole-array-piece-list serialize-data@
atemp$ bind strings copy$s
atemp$ testapl bind hole-array-piece-list serialize-data!
0 0 0 testapl bind hole-array-piece-list hole-list-quantity@ . ." < should be 6" cr
0 0 0 testapl2 bind hole-array-piece-list hole-list-quantity@ . ." < should be 6" cr
4 4 4 testapl bind hole-array-piece-list hole-list-quantity@ . ." < should be 6" cr
4 4 4 testapl2 bind hole-array-piece-list hole-list-quantity@ . ." < should be 6" cr
2 2 2 testapl bind hole-array-piece-list hole-list-quantity@ . ." < should be 36" cr
2 2 2 testapl2 bind hole-array-piece-list hole-list-quantity@ . ." < should be 36" cr


: keypause ( -- )
  begin key? until key drop ;
." press any key to continue test!" cr
keypause
." continuing!" cr
\ \\\
ref-piece-array fast-puzzle-board heap-new constant testboard

: seeholeonboard ( ux uy uz -- )
  testapl next-ref-piece-in-hole@ drop
  \ ref-piece-array [bind] piece-array upiece@
  testboard [bind] fast-puzzle-board board-piece! drop
  testboard [bind] fast-puzzle-board output-board ;

: putholeonboard { ux uy uz -- }
  ux uy uz testapl hole-list-quantity@ 0 ?do
    \ testboard [bind] fast-puzzle-board destruct
    \ testboard [bind] fast-puzzle-board construct
    testboard [bind] fast-puzzle-board clear-board
    ux uy uz seeholeonboard
    0 40 at-xy i . ." < index of >" ux . uy . uz .
    keypause
  loop ;

 2 2 2 putholeonboard
 0 0 0 putholeonboard
\ \\\
." press key to continue" cr keypause
." continuing!" cr

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

\\\
testapl bind hole-array-piece-list destruct
testboard bind fast-puzzle-board destruct
