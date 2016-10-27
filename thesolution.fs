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
  cell% field onepiece
end-struct plist%
create pliststart   \ list addresses of piece objects
pliststart plist% %size 960 * dup allot erase

: poppiecelist ( -- )
  960 0 do piece heap-new dup i plist% %size * pliststart + ! i swap newpiece! loop ;
poppiecelist
: piecetest ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
  0 { np1 np2 tempaddr }
  np1 np2 plist% %size * pliststart onepiece + @ dup to tempaddr collisionlist? ;
: piecexyz@ ( nsub# npiece# -- nx ny nz ) \ return the x y z values of npiece# givin the nsub# of the piece
  pliststart onepiece @ subpiece@ ;

create unionlist    \ list of pieces for union test
unionlist plist% %size 960 * dup allot erase

: union! ( npiece ni -- ) \ store npiece in union list at ni location
  plist% %size * unionlist onepiece + ! ;
: union@ ( ni -- npiece ) \ retreave npiece from union list from ni location
  plist% %size * unionlist onepiece + @ ;
0 value pa
0 value pb
0 value testpiece
0 value currenttestindex
: checklist { npiece -- nflag } \ test npiece in current union list to see if it can be added to list
  false currenttestindex 0 do i union@ npiece piecetest or loop ;
: testit ( -- )
  pa 0 apair ngetpieceb@ to pb
  pa 0 union!
  pb 1 union!
  2 to currenttestindex
  pa apair ngettotalpieceb@ 0 do
    pa i apair ngetpieceb@ to testpiece
    pb apair ngettotalpieceb@ 0 do
      pb i apair ngetpieceb@ testpiece = if testpiece checklist false =
        if testpiece currenttestindex union! currenttestindex 1 + to currenttestindex then
      then
    loop
  loop ;

displaypieces heap-new constant showit

: showapair ( npair -- ) \ display npair on the board
  showit construct
  apair ngetpair@ { paira pairb }
  0 paira piecexyz@ paira showit displaypiece!
  1 paira piecexyz@ paira showit displaypiece!
  2 paira piecexyz@ paira showit displaypiece!
  3 paira piecexyz@ paira showit displaypiece!
  4 paira piecexyz@ paira showit displaypiece!
  0 pairb piecexyz@ pairb showit displaypiece!
  1 pairb piecexyz@ pairb showit displaypiece!
  2 pairb piecexyz@ pairb showit displaypiece!
  3 pairb piecexyz@ pairb showit displaypiece!
  4 pairb piecexyz@ pairb showit displaypiece!
  showit showdisplay ;

\ 0 showapair
: showpairs ( nmax -- )
  0 do i showapair 4000 ms loop ;

: addshowpiece { npiece -- } \ simply display the board with npiece added to existing
  0 npiece piecexyz@ npiece showit displaypiece!
  1 npiece piecexyz@ npiece showit displaypiece!
  2 npiece piecexyz@ npiece showit displaypiece!
  3 npiece piecexyz@ npiece showit displaypiece!
  4 npiece piecexyz@ npiece showit displaypiece!
  showit showdisplay ;

: showapiece ( npiece -- ) \ simply display one piece on the board
  showit construct
  addshowpiece ;

: showpieces ( nmax nmin -- ) \ loop through display of nmax to nmin pieces
  do i showapiece 1000 ms loop ;

: showunionpiece { npiece ni -- } \ simply display the board with npiece added to existing
  0 npiece piecexyz@ ni showit displaypiece!
  1 npiece piecexyz@ ni showit displaypiece!
  2 npiece piecexyz@ ni showit displaypiece!
  3 npiece piecexyz@ ni showit displaypiece!
  4 npiece piecexyz@ ni showit displaypiece!
  showit showdisplay ;

: showunionpieces ( -- ) \ take the data from the current unionlist and display it
  showit construct
  currenttestindex 0 do
    i union@ i showunionpiece
  loop
  showit showdisplay ;

\\\ looks like 3 and 5 piece lists will not be able to fit in memory to solve
\ **************************************************************************************
0 value 3piecesums
21000000 constant 3piecemax

struct
  cell% field paira
  cell% field piece2
end-struct 3plist%
3plist% %size 3piecemax * dup allocate throw dup value 3plistaddr swap erase

: 2plistpieces@ ( ni -- npa npb ) \ retreave npa and npb from ni index
  apair ngetpair@ ;
: 3plistpieces@ ( ni -- npa npb npc )
  3plist% %size * { theaddr }
  theaddr 3plistaddr paira + @ 2plistpieces@
  theaddr 3plistaddr piece2 + @ ;
: 3plistpieces! ( npaira np2 ni -- )
  3plist% %size * { theaddr }
  theaddr 3plistaddr paira + rot swap !
  theaddr 3plistaddr piece2 + ! ;

: populate3pieces ( -- )
  960 0 do \ i . ." outer " 3piecesums . ." total" cr
    apair totalsize@ 0 do
      i 2plistpieces@ j piecetest swap j piecetest
      or 0 = if
        \ i j 3piecesums 3plistpieces!
        3piecesums 1 + to 3piecesums
      then
    loop
  loop ;
\ populate3pieces  \ uncomment this to generate the 3piece list
3piecesums . ."  the total 3 piece list!" cr ( is 20647344 )

0 value 5piecesums
: calc5pieces ( -- )
  0 0 0 0 0 { p3a p3b p3c p2a p2b }
  apair totalsize@ 0 do i . cr
    3piecesums 0 do
      i 3plistpieces@ to p3c to p3b to p3a
      j 2plistpieces@ to p2b to p2a
      p2a p3a piecetest
      p2a p3b piecetest
      p2a p3c piecetest
      p2b p3a piecetest
      p2b p3b piecetest
      p2b p3c piecetest
      or or or or or 0 = if
        5piecesums 1 + to 5piecesums
      then
    loop
  loop ;
