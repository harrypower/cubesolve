require ./Gforth-Objects/objects.fs
require ./newpieces.fs
require ./Gforth-Objects/mdca-obj.fs
require ./serialize-obj.fs


[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -piece-array
save-instance-data class
  destruction implementation
  selector upiece@
  selector quantity@
  protected
  cell% inst-var pieces-array \ mdca object array that contains piece objects for faster retrieval
  cell% inst-var pieces-array-quantity \ quantity of pieces in pieces-array
  cell% inst-var intersect-array \ will be the object to contain the 2d reference intersect array ( uses mdca object )
  m: ( upiece uindex piece-array -- ) \ store piece object into array
    pieces-array @ [bind] multi-cell-array cell-array!
  ;m method upiece!
  m: ( nflag uindex0 uindex1 piece-array -- ) \ store nflag in 2d intersect-array for fast intersect testing
    intersect-array @ [bind] multi-cell-array cell-array!
  ;m method uintersect-array!
  m: ( piece-array -- ) \ used only by serialize-data! to restore the data for this object
    this do-retrieve-inst-var
    pieces-array-quantity @ 1 multi-cell-array heap-new pieces-array !
    pieces-array-quantity @ dup 2 multi-cell-array heap-new intersect-array !
  ;m method serialize-piece-array-quantity!
  m: ( piece-array -- ) \ used only by serialize-data! to restore the data for this object

  ;m method serialize-piece-array-data!
  m: ( piece-array -- ) \ used only by serialize-data! to restore the data for this object

  ;m method serialize-intersect-array-data!
  public
  m: ( upieces piece-array -- ) \ construct the array from the contents of upieces!  Note the size is fixed at construct time!
    \ also construct the intersect array of reference pieces.
    this [parent] construct
    { upieces } upieces [bind] pieces pieces-quantity@ dup pieces-array-quantity !
    1 multi-cell-array heap-new pieces-array !
    pieces-array-quantity @ 0 ?do
      i upieces get-a-piece
      piece heap-new dup i this upiece!
      [bind] piece copy
    loop
    pieces-array-quantity @ dup 2 multi-cell-array heap-new intersect-array !
    pieces-array-quantity @ 0 ?do
      pieces-array-quantity @ 0 ?do
        i this upiece@
        j this upiece@ [bind] piece intersect?
        i j this uintersect-array!
      loop
    loop
  ;m overrides construct

  m: ( piece-array -- ) \ destruct the memory used!
    this [parent] destruct
    this quantity@ 0 ?do
      i this upiece@ dup [bind] piece destruct free throw
    loop
    pieces-array @ [bind] multi-cell-array destruct
    pieces-array @ free throw
    0 pieces-array-quantity !
    intersect-array @ [bind] multi-cell-array destruct
    intersect-array @ free throw
    0 intersect-array !
  ;m overrides destruct

  m: ( uindex piece-array -- upiece) \ retrieve upiece from array at uindex location
    pieces-array @ [bind] multi-cell-array cell-array@ ;m overrides upiece@

  m: ( uindex0 uindex1 piece-array -- nflag ) \ return nflag from intersect-array to get fast intersect detection for uindex0 and uindex1 pieces
    \ nflag is true if an intersection between uindex0 and uindex1 is found
    \ nflag is false if no intersection is found
    intersect-array @ [bind] multi-cell-array cell-array@
  ;m method fast-intersect?

  m: ( piece-array -- nquantity ) \ return the array size
    pieces-array-quantity @ ;m overrides quantity@

  m: ( piece-array -- nstrings ) \ to save this data
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
    ['] serialize-piece-array-quantity! this do-save-name
    this quantity@ this do-save-nnumber
    ['] pieces-array-quantity this do-save-inst-var
    ['] serialize-piece-array-data! this do-save-name
    this quantity@ this do-save-nnumber
    pieces-array-quantity @ 0 ?do
      i this upiece@ dup
      i this do-save-nnumber \ store index #
      [bind] piece voxel-quantity@ dup
      this do-save-nnumber \ store voxel size #
      0 ?do
        dup i swap [bind] piece get-voxel
        this do-save-nnumber \ x
        this do-save-nnumber \ y
        this do-save-nnumber \ z
        s" voxel" save$ [bind] strings !$x
      loop drop
      s" piece" save$ [bind] strings !$x
    loop
    ['] serialize-intersect-array-data! this do-save-name
    this quantity@ this do-save-nnumber
    \ now the loop of the intersect-array data
    pieces-array-quantity @ 0 ?do
      pieces-array-quantity @ 0 ?do
        i this do-save-nnumber
        j this do-save-nnumber
        i j this fast-intersect?
        this do-save-nnumber
        s" intersect" save$ [bind] strings !$x
      loop
    loop
    save$
  ;m overrides serialize-data@
  m: ( nstrings piece-array -- ) \ to restore previously saved data
    this destruct
    this [parent] construct
    save$ [bind] strings copy$s \ saves the strings object data to be used for retrieval
    this do-retrieve-data true = if 2drop -piece-array rot rot this $->method else 2drop 2drop abort" restore piece array quantity incorrect!" then
    this do-retrieve-data true = if 2drop -piece-array rot rot this $->method else 2drop 2drop abort" restore piece array data incorrect!" then
    this do-retrieve-data true = if 2drop -piece-array rot rot this $->method else 2drop 2drop abort" restore intersect array data incorrect!" then
  ;m overrides serialize-data!
end-class piece-array
' piece-array is -piece-array

\ ********************************************************************************************************************************
\ \\\
require ./puzzleboard.fs
board heap-new constant puzzle-board
require ./newpuzzle.def \ this is the definition of the puzzle to be solved

require ./allpieces.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

ref-piece-array bind piece-array serialize-data@ constant test$s

test$s $qty . ." the total items in the reference data"
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
test$s @$x dump
