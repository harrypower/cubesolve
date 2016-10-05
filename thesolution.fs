require c:\users\philip\documents\github\cubesolve\thepiece.fs

object class
  destruction implementation

  struct
      cell% field pieceb
  end-struct piecepair%

  inst-value piecea
  inst-value addrpiecelist
  inst-value piecelistsize
  cell% inst-var 2piece-test
  960 constant pindex-max
  inst-value piecea-object-addr

  \ protected \ ********************************************************************************************************
  m: ( np ni 2pieces -- ) \ store pieceb number at index
    piecepair% %size * addrpiecelist + pieceb !
  ;m method npieceb!
  m: ( ni 2pieces -- np ) \ retrieve pieceb number from index
    piecepair% %size * addrpiecelist + pieceb @
  ;m method npieceb@

  m: ( 2pieces -- ) \ populate the pieceb list using piecea data
    pindex-max 0 do
      i piecea-object-addr collisionlist?
      false =
      if
        i space piecelistsize this [current] npieceb!
        piecelistsize 1 + [to-inst] piecelistsize
      then
    loop
  ;m method populatepieceb
  public \ ***********************************************************************************************************
  m: ( n 2pieces -- ) \ construct a piece pair list
    \ make room to store pindex-max numbers for the pieceb value and store n into piecea then find all the pieceb values for piecea
    2piece-test 2piece-test @ <> \ to allocate this on the heap only once at fist construct execution time and after destruct method
    if
      piecepair% %alignment piecepair% %size pindex-max * %allocate throw [to-inst] addrpiecelist
      piece heap-new [to-inst] piecea-object-addr
    then
    addrpiecelist pindex-max erase
    0 [to-inst] piecelistsize
    dup [to-inst] piecea
    piecea-object-addr newpiece!
    this [current] populatepieceb
    2piece-test 2piece-test ! \ set test now that construct has run once
  ;m overrides construct
  m: ( 2pieces -- ) \ to release memory of this pair list
    addrpiecelist free throw
    0 [to-inst] addrpiecelist
    0 [to-inst] piecelistsize
    0 2piece-test !
    piecea-object-addr destruct
    piecea-object-addr free throw
  ;m overrides destruct
  m: ( nindex 2pieces -- npiecea npieceb ) \ for the present nindex return npiecea and npieceb values to stack
    this [current] npieceb@ piecea swap
  ;m method npair@
  m: ( 2pieces -- nsize )
    piecelistsize
  ;m method pairlistsize@
  m: ( -- ) \ print some internal variables for testing
    ." piecea " piecea . cr
    ." addrpiecelist " addrpiecelist . cr
    ." piecelistsize " piecelistsize . cr
    ." 2piece-test contents " 2piece-test @ . cr
    ." 2piece-test address " 2piece-test . cr
  ;m overrides print
end-class 2pieces

0 2pieces dict-new constant a
a print cr
0 a npair@ . . cr
1 a npair@ . . cr
2 a npair@ . . cr

: testsize a pairlistsize@ 0 do i a npair@ swap . . cr loop ;
testsize
