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
  inst-value board-pieces-list      \ double-linked-list of current pieces on board. if this equals max-board-piecs then puzzle is solved
  inst-value ref-piece-array        \ pieces-array that is a copy of uref-piece-array passed to this object
  inst-value max-board-array-index  \ how many voxel the board contains in total
  inst-value max-board-pieces       \ how many pieces the board needs to solve puzzle
  public
  m: ( uref-piece-array fast-puzzle-board -- ) \ constructor
    \ uref-piece-array is a piece-array object that contains all the pieces this puzzle board can place on it. This  uref-piece-array contents are copied into this object uref-piece-array itself is not stored.
    this [parent] construct
    x-puzzle-board y-puzzle-board z-puzzle-board * * [to-inst] max-board-array-index
    max-board-array-index 1 multi-cell-array heap-new [to-inst] board-array
    double-linked-list heap-new [to-inst] board-pieces-list
    puzzle-pieces piece-array heap-new [to-inst] ref-piece-array  \ just to form this object
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
  ;m method output-board

  m: ( fast-puzzle-board -- uquantity ) \ return current board piece quantity
    board-pieces-list [bind] double-linked-list ll-size@ ;m method board-pieces@

  m: ( uref-piece fast-puzzle-board -- nflag ) \ test if uref-piece can be placed in current board
  ;m method board-piece?

  m: ( uref-piece fast-puzzle-board -- nflag ) \ test if uref-piece can be place on an this empty board
  ;m method empty-board-place-piece?

  m: ( uref-piece fast-puzzle-board -- ) \ put uref-piece on board and in board array for display only if uref-piece does not intersect with other pieces!
  ;m method board-piece!

  m: ( uindex fast-puzzle-board -- uref-piece ) \ get uindex uref-piece from board piece list
    board-pieces-list [bind] double-linked-list nll-cell@  ;m method nboard-piece@

  m: ( fast-puzzle-board -- ux uy uz ) \ return the board dimensions
  ;m method board-dims@

  m: ( fast-puzzle-board -- nflag ) \ test if voxel ux uy uz is on this board
  \ nflag is true if it is false if it is not
  ;m method voxel-this-board?

  m: ( fast-puzzle-board -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
    \ put code here to stringafy the data
    save$
  ;m overrides serialize-data@

  m: ( nstrings fast-puzzle-board -- ) \ nstrings contains serialized data to restore this object
    this destruct
    this [parent] construct
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this $->method else 2drop 2drop true abort" FPB inst-value data incorrect!" then
    this do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this $->method else 2drop 2drop true abort" FPB indexed reference data incorrect!" then
  ;m overrides serialize-data!
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
