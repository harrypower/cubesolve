require c:\users\philip\documents\github\cubesolve\thepiece.fs

object class
  destruction implementation

  struct
      char% field pieceb
  end-struct piecepair%

  inst-value piecea
  inst-value addrpiecelist
  inst-value piecelistsize
  cell% inst-var 2piece-test
  960 constant pindex-max

  m: ( n 2pieces -- ) \ construct a piece pair list
    \ make room to store pindex-max numbers for the pieceb value and store n into piecea then find all the pieceb values from piecea
    2piece-test 2piece-test @ <> \ to allocate this on the heap only once at fist construct execution time
    if piecepair% %alignment piecepair% %size pindex-max * %allocate throw [to-inst] addrpiecelist then
    [to-inst] piecea
    2piece-test 2piece-test ! \ set test now that construct has run once
  ;m overrides construct
  m: ( 2pieces -- ) \ to release memory of this pair list
    addrpiecelist free throw
    0 [to-inst] addrpiecelist
    0 2piece-test !
  ;m overrides destruct
  m: ( nindex 2pieces -- npiecea npieceb ) \ for the present nindex return npiecea and npieceb values to stack
  ;m method npair!

  m: ( 2pieces -- nsize )
    piecelistsize
  ;m method piecelistsize@
  m: ( -- ) \ print some internal variables for testing
    ." piecea " piecea . cr
    ." addrpiecelist " addrpiecelist . cr
    ." piecelistsize " piecelistsize . cr
    ." 2piece-test contents " 2piece-test @ . cr
    ." 2piece-test address " 2piece-test . cr
  ;m overrides print
end-class 2pieces
