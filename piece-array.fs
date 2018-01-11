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
  inst-value temp$
  inst-value temp-piece
  m: ( upiece uindex piece-array -- ) \ store piece object into array
    pieces-array @ [bind] multi-cell-array cell-array!
  ;m method upiece!
  m: ( nflag uindex0 uindex1 piece-array -- ) \ store nflag in 2d intersect-array for fast intersect testing
    intersect-array @ [bind] multi-cell-array cell-array!
  ;m method uintersect-array!
  m: ( pieces -- ) \ used to parse save$ for one piece data to restore
    temp$ [bind] strings destruct
    temp$ [bind] strings construct
    begin
      save$ [bind] strings @$x
      2dup s" piece-end" str= true = if
        2drop true
      else
        temp$ [bind] strings !$x false
      then
    until
  ;m method parse-piece
  m: ( piece-array -- ) \ used only by serialize-data! to restore the data for this object
    -piece-array this do-retrieve-inst-var
  ;m method serialize-piece-array-quantity!
  m: ( upiece-amount piece-array -- ) \ used only by serialize-data! to restore the data for this object
    0 ?do
      this parse-piece
      temp$ temp-piece [bind] piece serialize-data!
      temp-piece i this upiece!
      piece heap-new [to-inst] temp-piece
    loop ;m method serialize-piece-array-data!
  m: ( piece-array -- ) \ used by serialize-data! and construct to restore and create the intersect data for this object
    pieces-array-quantity @ dup 2 multi-cell-array heap-new intersect-array !
    this quantity@ 0 ?do
      this quantity@ 0 ?do
        i this upiece@
        j this upiece@ [bind] piece intersect?
        i j this uintersect-array!
      loop
    loop ;m method serialize-intersect-array-data!
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
    this serialize-intersect-array-data!
    strings heap-new [to-inst] temp$
    piece heap-new [to-inst] temp-piece
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
    temp$ [bind] strings destruct
    temp$ free throw
    temp-piece [bind] piece destruct
    temp-piece free throw
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
      i this upiece@ [bind] piece serialize-data@
      save$ [bind] strings copy$s
      s" piece-end" save$ [bind] strings !$x
    loop
    ['] serialize-intersect-array-data! this do-save-name
    this quantity@ this do-save-nnumber
    save$
  ;m overrides serialize-data@
  m: ( nstrings piece-array -- ) \ to restore previously saved data
    this destruct
    this [parent] construct
    strings heap-new [to-inst] temp$
    piece heap-new [to-inst] temp-piece
    save$ [bind] strings copy$s \ saves the strings object data to be used for retrieval
    this do-retrieve-data true = if 2drop -piece-array rot rot this $->method else 2drop 2drop true abort" restore piece array quantity incorrect!" then
    pieces-array-quantity @ 1 multi-cell-array heap-new pieces-array !
    this do-retrieve-data true = if d>s rot rot -piece-array rot rot this $->method else 2drop 2drop true abort" restore piece array data incorrect!" then
    this do-retrieve-data true = if 2drop -piece-array rot rot this $->method else 2drop 2drop true abort" restore intersect array data incorrect!" then
  ;m overrides serialize-data!
end-class piece-array
' piece-array is -piece-array

\ ********************************************************************************************************************************
\\\
require ./newpuzzle.def \ this is the definition of the puzzle to be solved

require ./allpieces.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

ref-piece-array bind piece-array serialize-data@ value test$s
strings heap-new value test2$s
strings heap-new value test3$s

test$s $qty . ." the total items in the reference data" cr
: seeit ( namount nstart -- ) ?do i . ." : " i test$s []@$ drop type cr loop ;
100 0 seeit
8648 8600 seeit

test$s test2$s bind strings copy$s
ref-piece-list piece-array heap-new constant test-serialize

test2$s test-serialize bind piece-array serialize-data!
test-serialize bind piece-array serialize-data@ to test3$s

: $confirm2 \ used to confirm original serialized string matches second serialized string
  test$s [bind] strings reset
  test3$s [bind] strings reset
  test$s [bind] strings $qty 0 ?do
    test$s @$x
    test3$s @$x
    compare 0<> if i . ."  failed here! string compare" cr true throw then
  loop ." first serialized string matches second serialized string!" cr ;
test3$s bind strings $qty . ." size of test3$s!" cr
$confirm2

: ref-piece-compare
  ref-piece-array [bind] piece-array quantity@ 0 ?do
    i ref-piece-array [bind] piece-array upiece@
    i test-serialize [bind] piece-array upiece@
    [bind] piece same?
    false = if i . ." failed same? test here" cr true throw then
  loop ." Passed piece voxel compare test!" cr ;
ref-piece-compare

: fast-intersect-compare
  ref-piece-array [bind] piece-array quantity@  0 ?do
    ref-piece-array [bind] piece-array quantity@ 0 ?do
      i j ref-piece-array [bind] piece-array fast-intersect?
      i j test-serialize [bind] piece-array fast-intersect?
      <> if i . j . ." failed here! object compare" cr true throw then
    loop
  loop ." Passed fast-intersect test!" cr ;
fast-intersect-compare
