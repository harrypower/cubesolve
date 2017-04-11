require ./Gforth-Objects/objects.fs
require ./newpieces.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  selector upiece@
  selector quantity@
  protected
  cell% inst-var pieces-array-start \ start of pieces array address
  cell% inst-var pieces-array-quantity \ quantity of pieces in array
  struct
    cell% field piece-cell
  end-struct piece-cell%
  m: ( upiece uindex piece-array -- ) \ store piece object into array
    piece-cell% %size * pieces-array-start @ + piece-cell !
  ;m method upiece!

  public
  m: ( upieces piece-array -- ) \ construct the array from the contents of upieces!  Note the size is fixed at construct time!
    { upieces } upieces [bind] pieces pieces-quantity@ dup pieces-array-quantity !
    piece-cell% %size * allocate throw pieces-array-start !
    pieces-array-quantity @ 0 ?do
      i upieces get-a-piece
      piece heap-new dup i this upiece!
      [bind] piece copy
    loop
  ;m overrides construct

  m: ( piece-array -- ) \ destruct the memory used!
    this quantity@ 0 ?do
      i this upiece@ dup [bind] piece destruct free throw
    loop
    pieces-array-start @ free throw
    0 pieces-array-start !
    0 pieces-array-quantity !
  ;m overrides destruct

  m: ( uindex piece-array -- upiece) \ retrieve upiece from array at uindex location
    piece-cell% %size * pieces-array-start @ + piece-cell @ ;m overrides upiece@

  m: ( piece-array -- nquantity ) \ return the array size
    pieces-array-quantity @ ;m overrides quantity@

end-class piece-array


\ ********************************************************************************************************************************
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
