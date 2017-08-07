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
  cell% inst-var pieces-array \ pieces mdca object holding the array
  cell% inst-var pieces-array-quantity \ quantity of pieces in array
  cell% inst-var intersect-array \ will be the object to contain the 2d reference intersect array ( uses mdca object )
  m: ( upiece uindex piece-array -- ) \ store piece object into array
    pieces-array @ [bind] multi-cell-array cell-array!
  ;m method upiece!
  m: ( nflag uindex0 uindex1 piece-array -- ) \ store nflag in 2d intersect-array for fast intersect testing
    intersect-array @ [bind] multi-cell-array cell-array!
  ;m method uintersect-array!
  m: ( piece-array -- ) \ used only by serialize-data@ to restore the data for this object

  ;m method serialize-piece-array!
  public
  m: ( upieces piece-array -- ) \ construct the array from the contents of upieces!  Note the size is fixed at construct time!
    \ also construct the intersect array of reference pieces.
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
    ['] serialize-piece-array! this do-save-name
    \ this voxel-quantity@ this do-save-nnumber
    \ this voxel-quantity@ 0 ?do
    \   i this get-voxel
    \   this do-save-nnumber \ z
    \   this do-save-nnumber \ y
    \   this do-save-nnumber \ x
    \ loop
    save$
  ;m overrides serialize-data@
  m: ( nstrings piece-array -- ) \ to restore previously saved data
    this destruct
    this construct
    save$ [bind] strings copy$s \ saves the strings object data to be used for retrieval
    this do-retrieve-data true = if d>s rot rot -piece rot rot this $->method else 2drop 2drop abort" restore piece data incorrect!" then
  ;m overrides serialize-data!
end-class piece-array
' piece-array is -piece-array

\ ********************************************************************************************************************************
\\\
require ./allpieces.fs

0 puzzle-pieces make-all-pieces heap-new constant testmap
constant thelist

thelist bind pieces pieces-quantity@ . ." the size of all!" cr

thelist piece-array heap-new constant testarray

testarray quantity@ . ." the size of array!" cr

2 testarray upiece@ voxel-quantity@ . ." should be 5" cr

board heap-new constant testboard1
board heap-new constant testboard2
5 5 5 testboard1 set-board-dims
5 5 5 testboard2 set-board-dims

470 testarray upiece@ testboard1 place-piece-on-board . cr cr cr
470 thelist get-a-piece testboard2 place-piece-on-board . cr
testboard1 see-board cr
testboard2 see-board cr

testarray destruct ." done!" cr
