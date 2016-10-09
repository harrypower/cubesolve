require c:\users\philip\documents\github\cubesolve\thepiece.fs

object class
  destruction implementation

  struct
    cell% field pieceb
  end-struct piecepair%
  struct
    cell% field p1
    cell% field p2
  end-struct totalpairs%

  inst-value piecea
  inst-value addrpiecelist
  inst-value piecelistsize
  cell% inst-var 2piece-test
  960 constant pindex-max
  767000 constant pairindex-max
  inst-value piecea-object-addr
  inst-value totalpairlist
  inst-value pairlistsize

  protected \ ********************************************************************************************************
  m: ( np ni 2pieces -- ) \ store pieceb number at index
    piecepair% %size * addrpiecelist + pieceb !
  ;m method npieceb!
  m: ( ni 2pieces -- np ) \ retrieve pieceb number from index
    piecepair% %size * addrpiecelist + pieceb @
  ;m method npieceb@
  m: ( na nb ni 2pieces -- ) \ store pair in totalpairlist
    totalpairs% %size * totalpairlist + dup ( na nb addr addr )
    rot swap p2 !
    p1 !
  ;m method npair!
  m: ( ni 2pieces -- na nb ) \ retrieve pair from totalpairlist
    totalpairs% %size * totalpairlist + dup
    p1 @ swap p2 @
  ;m method npair@
  m: ( 2pieces -- ) \ populate the pieceb list using piecea data
    pindex-max 0 do
      i piecea-object-addr collisionlist? false =
      i piecea-object-addr adjacent? true = and
      if
        i piecelistsize this [current] npieceb!
        piecelistsize 1 + [to-inst] piecelistsize
      then
    loop
  ;m method populatepieceb
  m: ( n 2pieces -- ) \ calculate the pair that goes with value n
    0 [to-inst] piecelistsize
    dup [to-inst] piecea
    piecea-object-addr newpiece!
    this [current] populatepieceb
  ;m method calcpair
  m: ( 2pieces -- ) \ populate all the possible pairs in puzzle
    0 [to-inst] pairlistsize
    pindex-max 0 do
      i this [current] calcpair
      piecelistsize 0 do
        piecea i this [current] npieceb@ pairlistsize this [current] npair!
        pairlistsize 1 + [to-inst] pairlistsize
      loop
    loop
  ;m method populatetotalpairs
  public \ ***********************************************************************************************************
  m: ( 2pieces -- ) \ construct a piece pair list
    \ make room to store pindex-max numbers for the pieceb value and store n into piecea then find all the pieceb values for piecea
    2piece-test 2piece-test @ <> \ to allocate this on the heap only once at fist construct execution time and after destruct method
    if
      piecepair% %alignment piecepair% %size pindex-max * %allocate throw [to-inst] addrpiecelist
      piece heap-new [to-inst] piecea-object-addr
      totalpairs% %alignment totalpairs% %size pairindex-max * %allocate throw [to-inst] totalpairlist
    then
    addrpiecelist pindex-max erase
    totalpairlist pairindex-max erase
    0 [to-inst] piecelistsize
    0 [to-inst] pairlistsize
    2piece-test 2piece-test ! \ set test now that construct has run once
    this [current] populatetotalpairs
  ;m overrides construct
  m: ( 2pieces -- ) \ to release memory of this pair list
    addrpiecelist free throw
    totalpairlist free throw
    0 [to-inst] addrpiecelist
    0 [to-inst] piecelistsize
    0 [to-inst] pairlistsize
    0 2piece-test !
    piecea-object-addr destruct
    piecea-object-addr free throw
  ;m overrides destruct

  m: ( 2pieces -- nsize )
    pairlistsize
  ;m method totalsize@
  m: ( ni 2pieces -- npiecea npieceb ) \ return the pair for ni
    this [current] npair@
  ;m method ngetpair@
  m: ( -- ) \ print some internal variables for testing
    ." piecea " piecea . cr
    ." addrpiecelist " addrpiecelist . cr
    ." piecelistsize " piecelistsize . cr
    ." 2piece-test contents " 2piece-test @ . cr
    ." 2piece-test address " 2piece-test . cr
    ." totalpairlist " totalpairlist . cr
    ." pairlistsize " pairlistsize . cr
  ;m overrides print
end-class 2pieces

struct
  cell% field onepiece
end-struct plist%
create pliststart
pliststart plist% %size 960 * dup allot erase

: poppiecelist ( -- )
  960 0 do piece heap-new dup i plist% %size * pliststart + ! i swap newpiece! loop ;
poppiecelist
: piecetest ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
  0 { np1 np2 tempaddr }
  np1 np2 plist% %size * pliststart onepiece + @ dup to tempaddr collisionlist? false =
  if np1 tempaddr adjacent? true =
    if false else true then
  else
    true
  then ;
  \ c false a true yes     false false yes
  \ c false a false no     false true  no
  \ c true a true no       true  false no
  \ c true a false no      true  true  no


0 value 2pairsums
2pieces dict-new constant apair
apair totalsize@ ." totalsize = " . cr

260000 constant 2pairmax  \ 256344 is the actual value

struct
  cell% field paira
  cell% field pairb
end-struct 2plist%
2plist% %size 2pairmax * dup allocate throw dup value 2plistaddr swap erase

: 2plistaddres! ( npa npb npc ni -- )
;
: 2plistaddres@ ( ni -- npa npb npc )
;
: calc2pair ( -- )
  apair totalsize@ 0 do i . ." outer " 2pairsums . ." total" cr
    apair totalsize@ 0 do
      i apair ngetpair@ drop j apair ngetpair@ drop piecetest
      i apair ngetpair@ swap drop j apair ngetpair@ drop piecetest
      i apair ngetpair@ drop j apair ngetpair@ swap drop piecetest
      i apair ngetpair@ swap drop j apair ngetpair@ swap drop piecetest
      or or or 0 = if 2pairsums 1 + to 2pairsums then
    loop
  loop ;
\ calc2pair 2pairsums cr ." total pairs of pairs is " . cr
