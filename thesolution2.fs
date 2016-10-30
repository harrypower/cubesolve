require c:\users\philip\documents\github\cubesolve\thepiece.fs

object class
  destruction implementation
  protected \ *******************************************************************************************************
  struct
    cell% field pieceb
  end-struct piecepair%
  struct
    cell% field p1
    cell% field p2
  end-struct totalpairs%
  struct
    cell% field pairaddress
    cell% field pairamount
  end-struct pairlist%

  inst-value piecea
  inst-value addrpiecelist
  inst-value piecelistsize
  cell% inst-var 2piece-test
  960 constant pindex-max
  767000 constant pairindex-max
  inst-value piecea-object-addr
  inst-value totalpairlist
  inst-value pairlistsize
  inst-value pairlistindex

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
  m: ( npaddr npamount ni 2pieces -- ) \ store data into pairlistindex
  \  dup . space rot dup . space rot dup . rot cr \ just to see it
    pairlist% %size * pairlistindex + dup
    rot swap pairamount !
    pairaddress !
  ;m method npairlistindex!
  m: ( ni 2pieces -- npaddr npamount ) \ retrieve data from pairlistindex
    pairlist% %size * pairlistindex + dup pairaddress @
    swap pairamount @
  ;m method npairlistindex@
  m: ( 2pieces -- ) \ generate and store the pairlistindex data from the totalpairs data
    0 0 0 0 { nstartindex ncurpamount ntemp nlastpamount }
    pairlistsize 0 do
      nstartindex i this [current] npair@ drop dup to ntemp <
      if
        nlastpamount ncurpamount nstartindex this [current] npairlistindex!
        ntemp to nstartindex
        0 to ncurpamount
        i to nlastpamount
      else
        ncurpamount 1 + to ncurpamount
      then
    loop
    \ the last value needs to be stored in data set
    nlastpamount  pairlistsize nlastpamount - nstartindex this [current] npairlistindex!
  ;m method populatepairlistindex
  public \ ***********************************************************************************************************
  m: ( 2pieces -- ) \ construct a piece pair list
    \ make room to store pindex-max numbers for the pieceb value and store n into piecea then find all the pieceb values for piecea
    2piece-test 2piece-test @ <> \ to allocate this on the heap only once at fist construct execution time and after destruct method
    if
      piecepair% %alignment piecepair% %size pindex-max * %allocate throw [to-inst] addrpiecelist
      piece heap-new [to-inst] piecea-object-addr
      totalpairs% %alignment totalpairs% %size pairindex-max * %allocate throw [to-inst] totalpairlist
      pairlist% %alignment pairlist% %size pindex-max * %allocate throw [to-inst] pairlistindex
    then
    addrpiecelist pindex-max erase
    totalpairlist pairindex-max erase
    0 [to-inst] piecelistsize
    0 [to-inst] pairlistsize
    2piece-test 2piece-test ! \ set test now that construct has run once
    this [current] populatetotalpairs
    this [current] populatepairlistindex
  ;m overrides construct
  m: ( 2pieces -- ) \ to release memory of this pair list
    addrpiecelist free throw
    totalpairlist free throw
    pairlistindex free throw
    0 [to-inst] addrpiecelist
    0 [to-inst] piecelistsize
    0 [to-inst] pairlistsize
    0 [to-inst] pairlistindex
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
  m: ( npiecea 2pieces -- npiecebtotal ) \ return the total quantity of piece b parts for a given piecea value
    this [current] npairlistindex@ swap drop
  ;m method ngettotalpieceb@
  m: ( npiecea nindex 2pieces -- npieceb ) \ return piece b given the piece a and index
    swap this [current] npairlistindex@ drop + this [current] npair@ swap drop
  ;m method ngetpieceb@
  m: ( -- ) \ print some internal variables for testing
    ." piecea " piecea . cr
    ." addrpiecelist " addrpiecelist . cr
    ." piecelistsize " piecelistsize . cr
    ." 2piece-test contents " 2piece-test @ . cr
    ." 2piece-test address " 2piece-test . cr
    ." totalpairlist " totalpairlist . cr
    ." pairlistsize " pairlistsize . cr
    ." pairlistindex " pairlistindex . cr
  ;m overrides print
end-class 2pieces

2pieces dict-new constant apair
cr apair totalsize@ . ." the total 2 piece list!" cr ( is 256344 with adjacent test or 766056  with no adjacent testing of pieces )

struct
  cell% field a-piece
end-struct piece-list%
create piece-list-start   \ list of addresses of piece objects
piece-list-start piece-list% %size 960 * dup allot erase

: populate-piece-list ( -- )  \ populate the complete 960 set of pieces with collision tables calculated
  960 0 ?do piece heap-new dup i piece-list% %size * piece-list-start + ! i swap newpiece! loop ;
populate-piece-list
: piece-test ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
  piece-list% %size * piece-list-start a-piece + @ collisionlist? ;
: piece-xyz@ ( nsub# npiece# -- nx ny nz ) \ return the x y z values of npiece# givin the nsub# of the piece
  piece-list-start a-piece @ subpiece@ ;
  \ note this works because any piece object can return all the xyz info for any nsub# given any npiece#
  \ so i am just using the first piece object in the piece-list-start list of addresses for piece objects

create union-list    \ list of pieces for union solution
union-list piece-list% %size 960 * dup allot erase

: union! ( npiece ni -- ) \ store npiece in union list at ni location
  piece-list% %size * union-list a-piece + ! ;
: union@ ( ni -- npiece ) \ retreave npiece from union list from ni location
  piece-list% %size * union-list a-piece + @ ;

0 value current-index \ used to keep track of current location in union list to work on
: in-union-list? { npiece -- nflag } \ test npiece in current union-list to see if it can be added to list
  \ nflag is false if npiece can be added to list ... true if npiece can not be added to list
  current-index 0 > if
    false current-index 0 ?do i union@ npiece piece-test or loop
  else
    false
  then ;

: fullsolution ( nstart nend -- ) \ top level word to solve puzzle
  page
  swap ?do  \ the first piece to place in union list and piece to use to populate find sub list to pair with other pieces
    0 0 at-xy i . ." outside loop!      "
    960 0 ?do \ the second piece to place in union list only if it pairs with first piece
    \ if both pieces do not collid then add to union list
    \ use first piece to get second list of all pieces that work with it...

    loop
  loop
;

displaypieces heap-new constant show-it

: show-a-pair ( npair -- ) \ display npair on the board
  show-it construct
  apair ngetpair@ { paira pairb }
  0 paira piece-xyz@ paira show-it displaypiece!
  1 paira piece-xyz@ paira show-it displaypiece!
  2 paira piece-xyz@ paira show-it displaypiece!
  3 paira piece-xyz@ paira show-it displaypiece!
  4 paira piece-xyz@ paira show-it displaypiece!
  0 pairb piece-xyz@ pairb show-it displaypiece!
  1 pairb piece-xyz@ pairb show-it displaypiece!
  2 pairb piece-xyz@ pairb show-it displaypiece!
  3 pairb piece-xyz@ pairb show-it displaypiece!
  4 pairb piece-xyz@ pairb show-it displaypiece!
  show-it showdisplay ;

: showpairs ( nmax -- )
  0 do i show-a-pair 4000 ms loop ;

: add-show-piece { npiece -- } \ simply display the board with npiece added to existing
  0 npiece piece-xyz@ npiece show-it displaypiece!
  1 npiece piece-xyz@ npiece show-it displaypiece!
  2 npiece piece-xyz@ npiece show-it displaypiece!
  3 npiece piece-xyz@ npiece show-it displaypiece!
  4 npiece piece-xyz@ npiece show-it displaypiece!
  show-it showdisplay ;

: show-a-piece ( npiece -- ) \ simply display one piece on the board
  show-it construct
  add-show-piece ;

: show-pieces ( nmax nmin -- ) \ loop through display of nmax to nmin pieces
  ?do i show-a-piece 1000 ms loop ;

: show-union-piece { npiece ni -- } \ simply display the board with npiece added to existing board .. piece will be calle ni on board
  0 npiece piece-xyz@ ni show-it displaypiece!
  1 npiece piece-xyz@ ni show-it displaypiece!
  2 npiece piece-xyz@ ni show-it displaypiece!
  3 npiece piece-xyz@ ni show-it displaypiece!
  4 npiece piece-xyz@ ni show-it displaypiece!
  show-it showdisplay ;

: show-union-pieces ( ni -- ) \ take the data from the current unionlist and display it
  show-it construct
  ( ni ) 0 ?do
    i union@ i show-union-piece
  loop
  show-it showdisplay ;
