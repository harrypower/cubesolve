require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./Gforth-Objects/mdca-obj.fs
require ./piece-array.fs
require ./serialize-obj.fs
require ./newpuzzle.def \ this is the definition of the puzzle to be solved

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -fast-puzzle-board
save-instance-data class
  destruction implementation
  protected
  inst-value x-max
  inst-value y-max
  inst-value z-max
  inst-value z-mult                 \ calculated value for the z multiplier used to find board-array memory location
  inst-value x-display-offset
  inst-value y-display-offset
  inst-value z-display-offset
  inst-value max-board-array-index  \ how many voxel the board contains in total
  inst-value max-board-pieces       \ how many pieces the board needs to solve puzzle
  inst-value board-array            \ mcda of current board locations index numbers inside them ( piece voxels ) this is used for the terminal display
  inst-value board-pieces-list      \ double-linked-list of current pieces on board. if this equals max-board-pieces then puzzle is solved
  inst-value ref-piece-array        \ pieces-array that is a copy of uref-piece-array passed to this object
  inst-value serialize-temp-string$ \ used only to process serialized data
  m: ( uref-piece fast-puzzle-board -- ) \ place piece on display board
    0 { uref-piece upiece } uref-piece ref-piece-array [bind] piece-array upiece@ to upiece
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel  ( x y z )
      z-mult * ( x y z*scale )
      swap x-max * + ( x y*scale+z*scale )
      + uref-piece swap
      board-array [bind] multi-cell-array cell-array!
    loop
  ;m method put-on-display-board!

  m: ( uref-piece fast-puzzle-board -- ) \ remove piece from display board
    0 { uref-piece upiece } uref-piece ref-piece-array [bind] piece-array upiece@ to upiece
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel  ( x y z )
      z-mult * ( x y z*scale )
      swap x-max * + ( x y*scale+z*scale )
      + true swap
      board-array [bind] multi-cell-array cell-array!
    loop
  ;m method remove-from-display-board

  m: ( fast-puzzle-board -- ) \ file board-array with non piece
    max-board-array-index 0 ?do
      true i board-array [bind] multi-cell-array cell-array!
    loop
  ;m method empty-board

  m: ( ntotal-inst-vars fast-puzzle-board -- ) \ used only by serialize-data! to restore the data for this object
    0 ?do
      -fast-puzzle-board this do-retrieve-inst-value
    loop
  ;m method serialize-fpb-inst-values!

  m: ( nquantity fast-puzzle-board -- ) \ used only by serialize-data! to restore board-array data for this object
    0 ?do
      this do-retrieve-dnumber true <> abort" board-array serialized data bad!"
      d>s i board-array [bind] multi-cell-array cell-array!
    loop
  ;m method serialize-board-array!

  m: ( nquantity fast-puzzle-board -- ) \ used only by serialize-data! to restore board-pieces-list data for this object
    0 ?do
      this do-retrieve-dnumber true <> abort" board-pieces-list data bad!"
      d>s board-pieces-list [bind] double-linked-list ll-cell!
    loop
  ;m method serialize-board-pieces-list!

  m: ( nquantity fast-puzzle-board -- ) \ used only by serialize-data! to restore ref-piece-array data for this object
    serialize-temp-string$ [bind] strings destruct
    serialize-temp-string$ [bind] strings construct
    begin
      save$ [bind] strings @$x 2dup
      s" End of ref-piece-array data!" compare 0 = if 2drop true else serialize-temp-string$ [bind] strings !$x false then
    until
    serialize-temp-string$ ref-piece-array [bind] piece-array serialize-data!
    ref-piece-array [bind] piece-array quantity@ = invert abort" ref-piece-array data final size does not match saved size!"
  ;m method serialize-ref-piece-array!

  public
  m: ( uref-piece-array fast-puzzle-board -- ) \ constructor
    \ uref-piece-array is a piece-array object that contains all the pieces this puzzle board can place on it. This  uref-piece-array contents are copied into this object uref-piece-array itself is not stored.
    this [parent] construct
    x-puzzle-board [to-inst] x-max
    y-puzzle-board [to-inst] y-max
    z-puzzle-board [to-inst] z-max
    x-max y-max z-max * * [to-inst] max-board-array-index
    max-board-array-index 1 multi-cell-array heap-new [to-inst] board-array
    this [current] empty-board
    double-linked-list heap-new [to-inst] board-pieces-list
    puzzle-pieces piece-array heap-new [to-inst] ref-piece-array  \ note this instantiates the object of piece-array quickly but the next line of code will destruct and construct the object again with uref-piece-array data ( more or less just starting the object here!)
    [bind] piece-array serialize-data@ ref-piece-array [bind] piece-array serialize-data! \ now copy uref-piece-array to ref-piece-array
    0 ref-piece-array [bind] piece-array upiece@ [bind] piece voxel-quantity@ max-board-array-index swap / [to-inst] max-board-pieces
    \ idea here is the all pieces in ref-piece-array are same size so use that first piece to calculate max-board-pieces
    x-max y-max * [to-inst] z-mult
    ref-piece-array [bind] piece-array quantity@ s>f flog fround f>s 3 + [to-inst] x-display-offset ( max number + padding + space between )
    1 [to-inst] y-display-offset ( line spacing )
    y-max y-display-offset * 1 + [to-inst] z-display-offset ( y display size + 1 line seperator )
    strings heap-new [to-inst] serialize-temp-string$
  ;m overrides construct
  m: ( fast-puzzle-board -- ) \ destructor to release all allocated memory
    this [parent] destruct
    board-array [bind] multi-cell-array destruct
    board-array free throw
    board-pieces-list [bind] double-linked-list destruct
    board-pieces-list free throw
    ref-piece-array [bind] piece-array destruct
    ref-piece-array free throw
    0 [to-inst] max-board-pieces
    0 [to-inst] max-board-array-index
    serialize-temp-string$ [bind] strings destruct
    serialize-temp-string$ free throw
  ;m overrides destruct

  m: ( fast-puzzle-board -- uindex ) \ return the max board index address or max voxel count
    max-board-array-index ;m method max-board-index@

  m: ( uvoxelindex fast-puzzle-board -- uref-piece ) \ given uvoxelindex return the uref-piece that is located on this board
    \ note uref-piece could return as true indicating that there is no ref-piece on the board at uvoxelindex
    \ uref-piece returned is true or a uref-piece value from 0 to max reference pieces
    board-array [bind] multi-cell-array cell-array@ ;m method nvoxel@

  m: ( fast-puzzle-board -- ) \ terminal display
    page
    z-max 0 ?do
      y-max 0 ?do
        x-max 0 ?do
          i x-display-offset * j y-display-offset * k z-display-offset * + at-xy
          i j k z-mult * ( x y z*scale )
          swap x-max * + ( x y*scale+z*scale )
          + board-array [bind] multi-cell-array cell-array@
          dup true = if drop ." *****" else u. then
        loop
      loop
    loop ;m method output-board

  m: ( fast-puzzle-board -- uquantity ) \ return current board piece quantity
    board-pieces-list [bind] double-linked-list ll-size@ ;m method board-pieces@

  m: ( fast-puzzle-board -- uquantity ) \ return quantity of pieces to solve this board puzzle
    max-board-pieces ;m method max-board-pieces@

  m: ( fast-puzzle-board -- ux uy uz ) \ return the board dimensions
    x-max y-max z-max  ;m method board-dims@

  m: ( uref-piece fast-puzzle-board -- nflag ) \ test if uref-piece can be placed in current board
    \ nflag is true if uref-piece can be placed on the current board
    \ nflag is false if uref-piece can not be placed on current board
    { uref-piece }
    this [current] board-pieces@ 0 > if
      board-pieces-list [bind] double-linked-list ll-set-start
      begin
        board-pieces-list [bind] double-linked-list ll-cell@
        uref-piece ref-piece-array [bind] piece-array fast-intersect? true =
        if
          true true
        else
          board-pieces-list [bind] double-linked-list ll>
          if false true else false then
        then
      until
      invert
    else
      true
    then ;m method board-piece?

  m: ( uref-piece fast-puzzle-board -- nflag ) \ put uref-piece on board and in board array for display only if uref-piece does not intersect with other pieces!
    \ nflag is true if uref-piece was place on board
    \ nflag is false if uref-piece was not placed on board
    { uref-piece } uref-piece this [current] board-piece? true = if
      uref-piece board-pieces-list [bind] double-linked-list ll-cell!
      uref-piece this [current] put-on-display-board!
      true
    else
      false
    then ;m method board-piece!

  m: ( uindex fast-puzzle-board -- uref-piece ) \ get uref-piece from board piece list at uindex location
    board-pieces-list [bind] double-linked-list nll-cell@  ;m method nboard-piece@

  m: ( uref-piece fast-puzzle-board -- ) \ remove last piece put on this board
    board-pieces-list [bind] double-linked-list ll-set-end
    board-pieces-list [bind] double-linked-list ll-cell@
    this [current] remove-from-display-board
    board-pieces-list [bind] double-linked-list delete-last
  ;m method remove-last-piece

  m: ( fast-puzzle-board -- ) \ empty board of its pieces but keep the internal references to pieces so construct does not need to be used
    board-array [bind] multi-cell-array destruct
    max-board-array-index 1 board-array [bind] multi-cell-array construct
    this [current] empty-board
    board-pieces-list [bind] double-linked-list destruct
    board-pieces-list [bind] double-linked-list construct
  ;m method clear-board

  m: ( fast-puzzle-board -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct

    ['] serialize-fpb-inst-values! this do-save-name
    9 this do-save-nnumber  \ there are 9 inst-var saved here to serialize and retrieve later

    ['] x-max this do-save-inst-value
    ['] y-max this do-save-inst-value
    ['] z-max this do-save-inst-value
    ['] z-mult this do-save-inst-value
    ['] x-display-offset this do-save-inst-value
    ['] y-display-offset this do-save-inst-value
    ['] z-display-offset this do-save-inst-value
    ['] max-board-array-index this do-save-inst-value
    ['] max-board-pieces this do-save-inst-value

    ['] serialize-board-array! this do-save-name
    board-array [bind] multi-cell-array cell-array-dimensions@ drop dup this do-save-nnumber
    0 ?do
      i board-array [bind] multi-cell-array cell-array@ this do-save-nnumber
    loop

    ['] serialize-board-pieces-list! this do-save-name
    board-pieces-list [bind] double-linked-list ll-size@ dup this do-save-nnumber
    board-pieces-list [bind] double-linked-list ll-set-start
    0 ?do
      board-pieces-list [bind] double-linked-list ll-cell@ this do-save-nnumber
    loop

    ['] serialize-ref-piece-array! this do-save-name
    ref-piece-array [bind] piece-array quantity@ this do-save-nnumber
    ref-piece-array [bind] piece-array serialize-data@ save$ [bind] strings copy$s
    s" End of ref-piece-array data!" save$ [bind] strings !$x
    save$
  ;m overrides serialize-data@

  m: ( nstrings fast-puzzle-board -- ) \ nstrings contains serialized data to restore this object
    this [current] destruct
    this [parent] construct
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB inst-value data incorrect!" then

    strings heap-new [to-inst] serialize-temp-string$
    max-board-array-index 1 multi-cell-array heap-new [to-inst] board-array \ construct board-array
    this [current] empty-board                                              \ empty the board
    double-linked-list heap-new [to-inst] board-pieces-list                 \ construct board-pieces-list
    puzzle-pieces piece-array heap-new [to-inst] ref-piece-array            \ construct ref-piece-array but only as a place holder to be created with real data next

    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB board-array data incorrect!" then
    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB board-pieces-list data incorrect!" then
    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB ref-piece-array data incorrect!" then
  ;m overrides serialize-data!

  m: ( fast-puzzle-board -- ) \ print stuff for testing
    ref-piece-array [bind] piece-array quantity@ . ." ref-piece-array quantity" cr
    board-array [bind] multi-cell-array cell-array-dimensions@ . . ." board-array dimensions" cr
    board-pieces-list [bind] double-linked-list ll-size@ . ." board-pieces-list size" cr
    x-max . ." x-max" cr
    y-max . ." y-max" cr
    z-max . ." z-max" cr
    z-mult . ." z-mult" cr
    x-display-offset . ." x-display-offset" cr
    y-display-offset . ." y-display-offset" cr
    z-display-offset . ." z-display-offset" cr
    max-board-array-index . ." max-board-array-index" cr
    max-board-pieces . ." max-board-pieces" cr
  ;m overrides print
end-class fast-puzzle-board
' fast-puzzle-board is -fast-puzzle-board

\ **********************************************************************************************************************************************************************
\\\
require ./allpieces.fs
.s cr
0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is never used directly but produces ref piece list only
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list bind pieces pieces-quantity@ . ." < should be 480!" cr
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster
ref-piece-array bind piece-array quantity@ . ." < should be 480!" cr
.s cr
ref-piece-array fast-puzzle-board heap-new constant testfastb
cr testfastb max-board-index@ . ." < should be 125!" cr
.s cr
testfastb max-board-pieces@ . ." < should be 25!" cr

testfastb bind fast-puzzle-board print cr
.s cr
0 testfastb bind fast-puzzle-board board-piece? . ." should be true or -1" cr
0 testfastb bind fast-puzzle-board board-piece! . ." should be true " cr
5 testfastb bind fast-puzzle-board board-piece? . ." should be false or 0" cr
10 testfastb bind fast-puzzle-board board-piece? . ." should be true or -1" cr
10 testfastb bind fast-puzzle-board board-piece! . ." should be true " cr
15 testfastb bind fast-puzzle-board board-piece? . ." should be false or 0" cr
.s cr
testfastb bind fast-puzzle-board output-board cr
." should be with two pieces" cr
testfastb bind fast-puzzle-board remove-last-piece
testfastb bind fast-puzzle-board output-board cr
." should be only one piece" cr
testfastb bind fast-puzzle-board clear-board cr
testfastb bind fast-puzzle-board output-board cr
.s cr

strings heap-new constant testserialize$
0 testfastb bind fast-puzzle-board board-piece! . ." should be true " cr
10 testfastb bind fast-puzzle-board board-piece! . ." should be true " cr
testfastb bind fast-puzzle-board print
.s cr ." data before serializing! " cr
.s cr
testfastb bind fast-puzzle-board serialize-data@
.s cr
testserialize$ bind strings copy$s
.s cr
testserialize$ testfastb bind fast-puzzle-board serialize-data!
.s cr
testfastb bind fast-puzzle-board print
." data after serializing! " cr
testfastb bind fast-puzzle-board output-board cr

\ ******************************************************************************************************************************************
\ the following was used to confirm speed improvement of this fast-puzzle-board over board
\\\
0 testfastb bind fast-puzzle-board board-piece! . ." < should be true. placed 0 on board for testing speed!" cr
10 testfastb bind fast-puzzle-board board-piece! . ." < should be true. placed 10 on board for testing speed!" cr
: fasttest
utime
480 0 ?do
  i testfastb [bind] fast-puzzle-board board-piece? drop
loop
utime 2swap d- d. ." < time it takes for fast piece test!" cr
;
.s cr
require ./puzzleboard.fs
board heap-new constant puzzle-board
x-puzzle-board y-puzzle-board z-puzzle-board puzzle-board bind board set-board-dims
0 ref-piece-array bind piece-array upiece@ puzzle-board bind board place-piece-on-board drop
10 ref-piece-array bind piece-array upiece@ puzzle-board bind board place-piece-on-board drop
: slowtest
utime
480 0 ?do
  i ref-piece-array [bind] piece-array upiece@
  puzzle-board [bind] board piece-on-board? drop
loop
utime 2swap d- d. ." < time it takes for slow piece test!" cr
;
.s cr
0 ref-piece-array bind piece-array upiece@ constant 0item
400 ref-piece-array bind piece-array upiece@ constant 400item
.s cr
: slowtest1
utime
480 0 ?do
  0item puzzle-board [bind] board piece-on-board? drop
loop
utime 2swap d- d. ." < time it takes for 0item slow piece test!" cr
;

: slowtest2
utime
480 0 ?do
  400item puzzle-board [bind] board piece-on-board? drop
loop
utime 2swap d- d. ." < time it takes for 400item slow piece test!" cr
;

fasttest .s cr
slowtest .s cr
slowtest1 .s cr
slowtest2 .s cr
