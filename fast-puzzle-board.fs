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
  inst-value board-array            \ mcda of current board locations index numbers inside them ( piece voxels ) this is used for the terminal display
  inst-value board-pieces-list      \ double-linked-list of current pieces on board. if this equals max-board-pieces then puzzle is solved
  inst-value ref-piece-array        \ pieces-array that is a copy of uref-piece-array passed to this object
  inst-value max-board-array-index  \ how many voxel the board contains in total
  inst-value max-board-pieces       \ how many pieces the board needs to solve puzzle
  inst-value non-piece              \ a value that repesents no piece on the board

  m: ( uref-piece fast-puzzle-board -- ) \ place piece on display board
    0 { uref-piece upiece } uref-piece ref-piece-array [bind] piece-array upiece@ to upiece
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel  ( x y z )
      x-puzzle-board y-puzzle-board * * ( x y z*scale )
      swap x-puzzle-board * + ( x y*scale+z*scale )
      + uref-piece swap
      board-array [bind] multi-cell-array cell-array!
    loop
  ;m method put-on-display-board!

  m: ( uref-piece fast-puzzle-board -- ) \ remove piece from display board
    0 { uref-piece upiece } uref-piece ref-piece-array [bind] piece-array upiece@ to upiece
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel  ( x y z )
      x-puzzle-board y-puzzle-board * * ( x y z*scale )
      swap x-puzzle-board * + ( x y*scale+z*scale )
      + non-piece swap
      board-array [bind] multi-cell-array cell-array!
    loop
  ;m method remove-from-display-board

  m: ( fast-puzzle-board -- ) \ file board-array with non piece
    max-board-array-index 0 ?do
      non-piece i board-array [bind] multi-cell-array cell-array!
    loop
  ;m method empty-board
  public
  m: ( uref-piece-array fast-puzzle-board -- ) \ constructor
    \ uref-piece-array is a piece-array object that contains all the pieces this puzzle board can place on it. This  uref-piece-array contents are copied into this object uref-piece-array itself is not stored.
    this [parent] construct
    x-puzzle-board y-puzzle-board z-puzzle-board * * 999 + [to-inst] non-piece
    x-puzzle-board y-puzzle-board z-puzzle-board * * [to-inst] max-board-array-index
    max-board-array-index 1 multi-cell-array heap-new [to-inst] board-array
    this [current] empty-board
    double-linked-list heap-new [to-inst] board-pieces-list
    puzzle-pieces piece-array heap-new [to-inst] ref-piece-array  \ note this instantiates the object of piece-array quickly but the next line of code will destruct and construct the object again with uref-piece-array data ( more or less just starting the object here!)
    [bind] piece-array serialize-data@ ref-piece-array [bind] piece-array serialize-data! \ now copy uref-piece-array to ref-piece-array
    0 ref-piece-array [bind] piece-array upiece@ [bind] piece voxel-quantity@ max-board-array-index swap / [to-inst] max-board-pieces
    \ idea here is the all pieces in ref-piece-array are same size so use that first piece to calculate max-board-pieces
  ;m overrides construct
  m: ( fast-puzzle-board -- ) \ destructor
    this [parent] destruct
  ;m overrides destruct

  m: ( fast-puzzle-board -- uindex ) \ return the max board index address
    max-board-array-index ;m method max-board-index@

  m: ( fast-puzzle-board -- ) \ terminal display
    this [current] max-board-index@ 0 ?do
      i board-array [bind] multi-cell-array cell-array@ . cr
    loop
  ;m method output-board

  m: ( fast-puzzle-board -- uquantity ) \ return current board piece quantity
    board-pieces-list [bind] double-linked-list ll-size@ ;m method board-pieces@

  m: ( fast-puzzle-board -- uquantity ) \ return quantity of pieces to solve this board puzzle
    max-board-pieces ;m method max-board-pieces@

  m: ( fast-puzzle-board -- ux uy uz ) \ return the board dimensions
    x-puzzle-board y-puzzle-board z-puzzle-board  ;m method board-dims@

  m: ( uref-piece fast-puzzle-board -- nflag ) \ test if uref-piece can be placed in current board
    \ nflag is true if uref-piece can be placed on the current board
    \ nflag is false if uref-piece can not be placed on current board
    { uref-piece }
    this [current] board-pieces@ 0 > if
      board-pieces-list [bind] double-linked-list ll-set-start
      \ ." start of board-pieces-list" cr
      begin
        board-pieces-list [bind] double-linked-list ll-cell@
        \ dup . ." board-pieces-list item " cr
        uref-piece ref-piece-array [bind] piece-array fast-intersect? true =
        \ dup . ." fast-intersect?" cr
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
    then
  ;m method board-piece?

  m: ( uref-piece fast-puzzle-board -- nflag ) \ put uref-piece on board and in board array for display only if uref-piece does not intersect with other pieces!
    \ nflag is true if uref-piece was place on board
    \ nflag is false if uref-piece was not placed on board
    { uref-piece } uref-piece this [current] board-piece? true = if
      uref-piece board-pieces-list [bind] double-linked-list ll-cell!
      uref-piece this [current] put-on-display-board!
      true
    else
      false
    then
  ;m method board-piece!

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
    \ put code here to stringafy the data
    save$
  ;m overrides serialize-data@

  m: ( nstrings fast-puzzle-board -- ) \ nstrings contains serialized data to restore this object
    this [current] destruct
    this [parent] construct
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB inst-value data incorrect!" then
    this [current] do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this [current] $->method else 2drop 2drop true abort" FPB indexed reference data incorrect!" then
  ;m overrides serialize-data!

  m: ( fast-puzzle-board -- ) \ print stuff for testing
    ref-piece-array [bind] piece-array quantity@ . ." ref-piece-array quantity" cr
  ;m overrides print
end-class fast-puzzle-board
' fast-puzzle-board is -fast-puzzle-board

\ **********************************************************************************************************************************************************************
\ \\\
require ./allpieces.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is never used directly but produces ref piece list only
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list bind pieces pieces-quantity@ . ." < should be 480!" cr
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster
ref-piece-array bind piece-array quantity@ . ." < should be 480!" cr

ref-piece-array fast-puzzle-board heap-new constant testfastb
cr testfastb max-board-index@ . ." < should be 125!" cr

testfastb max-board-pieces@ . ." < should be 25!" cr

testfastb bind fast-puzzle-board print cr

0 testfastb bind fast-puzzle-board board-piece? . ." should be true or -1" cr
0 testfastb bind fast-puzzle-board board-piece!
5 testfastb bind fast-puzzle-board board-piece? . ." should be false or 0" cr
10 testfastb bind fast-puzzle-board board-piece? . ." should be true or -1" cr
10 testfastb bind fast-puzzle-board board-piece!
15 testfastb bind fast-puzzle-board board-piece? . ." should be false or 0" cr

testfastb bind fast-puzzle-board output-board cr
." should be with two pieces" cr
testfastb bind fast-puzzle-board remove-last-piece
testfastb bind fast-puzzle-board output-board cr
." should be only one piece" cr
testfastb bind fast-puzzle-board clear-board cr
testfastb bind fast-puzzle-board output-board cr

\\\
utime
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
0 testfastb bind fast-puzzle-board board-piece? drop
utime 2swap d- d. ." < time it takes for fast piece test!" cr

require ./puzzleboard.fs
board heap-new constant puzzle-board
x-puzzle-board y-puzzle-board z-puzzle-board puzzle-board bind board set-board-dims
0 ref-piece-array bind piece-array upiece@ constant testpiece
utime
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
testpiece puzzle-board bind board piece-on-board? drop
utime 2swap d- d. ." < time it takes for slow piece test!" cr

\\\
require ./puzzleboard.fs
board heap-new constant puzzle-board
x-puzzle-board y-puzzle-board z-puzzle-board puzzle-board bind board set-board-dims
0 ref-piece-array bind piece-array upiece@
puzzle-board bind board place-piece-on-board drop
puzzle-board bind board see-board

4000 ms

puzzle-board bind board construct
x-puzzle-board y-puzzle-board z-puzzle-board puzzle-board bind board set-board-dims
8 ref-piece-array bind piece-array upiece@
puzzle-board bind board place-piece-on-board drop
puzzle-board bind board see-board
