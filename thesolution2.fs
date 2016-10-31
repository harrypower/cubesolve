require c:\users\philip\documents\github\cubesolve\thepiece.fs

object class
  destruction implementation
  protected \ *******************************************************************************************************
  struct
    cell% field pieceb
  end-struct piece-pair%
  struct
    cell% field p1
    cell% field p2
  end-struct total-pairs%
  struct
    cell% field pair-address
    cell% field pair-qnt
  end-struct pair-list%

  inst-value piecea
  inst-value addr-piece-list
  inst-value piece-list-size
  cell% inst-var 2piece-test
  960 constant piece-index-max
  767000 constant pair-index-max
  inst-value piecea-object-addr
  inst-value total-pair-list
  inst-value pair-list-qnt
  inst-value pair-list-index

  m: ( np ni 2pieces -- ) \ store pieceb number at index
    piece-pair% %size * addr-piece-list pieceb + !
  ;m method npieceb!
  m: ( ni 2pieces -- np ) \ retrieve pieceb number from index
    piece-pair% %size * addr-piece-list pieceb + @
  ;m method npieceb@
  m: ( na nb ni 2pieces -- ) \ store pair in total-pair-list
    total-pairs% %size * total-pair-list + dup ( na nb addr addr )
    rot swap p2 !
    p1 !
  ;m method npair!
  m: ( ni 2pieces -- na nb ) \ retrieve pair from total-pair-list
    total-pairs% %size * total-pair-list + dup
    p1 @ swap p2 @
  ;m method npair@
  m: ( 2pieces -- ) \ populate the pieceb list using piecea data
    piece-index-max 0 ?do
      i piecea-object-addr collisionlist? false =
      if
        i piece-list-size this [current] npieceb!
        piece-list-size 1 + [to-inst] piece-list-size
      then
    loop
  ;m method populate-pieceb
  m: ( npiece 2pieces -- ) \ calculate the pair piece that goes with npiece
    0 [to-inst] piece-list-size
    dup [to-inst] piecea
    piecea-object-addr newpiece!
    this [current] populate-pieceb
  ;m method calculate-pair
  m: ( 2pieces -- ) \ populate all the possible pairs in puzzle
    0 [to-inst] pair-list-qnt
    piece-index-max 0 ?do
      i this [current] calculate-pair
      piece-list-size 0 ?do
        piecea i this [current] npieceb@ pair-list-qnt this [current] npair!
        pair-list-qnt 1 + [to-inst] pair-list-qnt
      loop
    loop
  ;m method populate-total-pairs
  m: ( npairaddr npair-qnt ni 2pieces -- ) \ store data into pair-list-index
  \  dup . space rot dup . space rot dup . rot cr \ just to see it
    pair-list% %size * pair-list-index + dup
    rot swap pair-qnt !
    pair-address !
  ;m method npair-list-index!
  m: ( ni 2pieces -- npairaddr npairqnt ) \ retrieve data from pair-list-index
    pair-list% %size * pair-list-index + dup pair-address @
    swap pair-qnt @
  ;m method npair-list-index@
  m: ( 2pieces -- ) \ generate and store the pair-list-index data from the totalpairs data
    0 0 0 0 { nstart-index ncurrent-piece-qnt ntemp nlast-piece-qnt }
    pair-list-qnt 0 ?do
      nstart-index i this [current] npair@ drop dup to ntemp <
      if
        nlast-piece-qnt ncurrent-piece-qnt nstart-index this [current] npair-list-index!
        ntemp to nstart-index
        0 to ncurrent-piece-qnt
        i to nlast-piece-qnt
      else
        ncurrent-piece-qnt 1 + to ncurrent-piece-qnt
      then
    loop
    \ the last value needs to be stored in data set
    nlast-piece-qnt  pair-list-qnt nlast-piece-qnt - nstart-index this [current] npair-list-index!
  ;m method populate-pair-list-index
  public \ ***********************************************************************************************************
  m: ( 2pieces -- ) \ construct a piece pair list
    \ make room to store piece-index-max numbers for the pieceb value and store n into piecea then find all the pieceb values for piecea
    2piece-test 2piece-test @ <> \ to allocate this on the heap only once at fist construct execution time and after destruct method
    if
      piece-pair% %alignment piece-pair% %size piece-index-max * %allocate throw [to-inst] addr-piece-list
      piece heap-new [to-inst] piecea-object-addr
      total-pairs% %alignment total-pairs% %size pair-index-max * %allocate throw [to-inst] total-pair-list
      pair-list% %alignment pair-list% %size piece-index-max * %allocate throw [to-inst] pair-list-index
    then
    addr-piece-list piece-pair% %size piece-index-max * erase
    total-pair-list total-pairs% %size pair-index-max * erase
    0 [to-inst] piece-list-size
    0 [to-inst] pair-list-qnt
    2piece-test 2piece-test ! \ set test now that construct has run once
    this [current] populate-total-pairs
    this [current] populate-pair-list-index
  ;m overrides construct
  m: ( 2pieces -- ) \ to release memory of this pair list
    addr-piece-list free throw
    total-pair-list free throw
    pair-list-index free throw
    0 [to-inst] addr-piece-list
    0 [to-inst] piece-list-size
    0 [to-inst] pair-list-qnt
    0 [to-inst] pair-list-index
    0 2piece-test !
    piecea-object-addr destruct
    piecea-object-addr free throw
  ;m overrides destruct
  m: ( 2pieces -- nsize )
    pair-list-qnt
  ;m method total-pair-qnt@
  m: ( ni 2pieces -- npiecea npieceb ) \ return the pair for ni
    this [current] npair@
  ;m method nget-pair@
  m: ( npiecea 2pieces -- npiecebqnt ) \ return the total quantity of piece b parts for a given piecea value
    this [current] npair-list-index@ swap drop
  ;m method nget-total-pieceb@
  m: ( npiecea nindex 2pieces -- npieceb ) \ return piece b given the piece a and index
    swap this [current] npair-list-index@ drop + this [current] npair@ swap drop
  ;m method nget-pieceb@
  m: ( -- ) \ print some internal variables for testing
    ." piecea " piecea . cr
    ." addr-piece-list " addr-piece-list . cr
    ." piece-list-size " piece-list-size . cr
    ." 2piece-test contents " 2piece-test @ . cr
    ." 2piece-test address " 2piece-test . cr
    ." total-pair-list " total-pair-list . cr
    ." pair-list-qnt " pair-list-qnt . cr
    ." pair-list-index " pair-list-index . cr
  ;m overrides print
end-class 2pieces

2pieces dict-new constant the-pairs
cr the-pairs total-pair-qnt@ . ." the total 2 piece list!" cr ( is 256344 with adjacent test or 766056  with no adjacent testing of pieces )

struct
  cell% field a-piece
end-struct piece-list%
create piece-list-start   \ list of addresses of piece objects
piece-list-start piece-list% %size 960 * dup allot erase

: populate-piece-list ( -- )  \ populate the complete 960 set of pieces with collision tables calculated
  960 0 ?do piece heap-new dup i piece-list% %size * piece-list-start + ! i swap newpiece! loop ;
populate-piece-list
: piece-test? ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
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

0 value test-piece
0 value current-test-index
0 value max-solution
0 value piece-a
0 value piece-b

: in-union-list? { npiece -- nflag } \ test npiece in current union-list to see if it can be added to list
  \ nflag is false if npiece can be added to list ... true if npiece can not be added to list
  current-test-index 0 > if
    false current-test-index 0 ?do i union@ npiece piece-test? or loop
  else
    false
  then ;
: set-max-solution ( ncurrent-test-index -- ) \ increase max-solution if current-test-index is larger then max-solution
  dup max-solution > if to max-solution else drop then ;
: solve-a-pair { npa npb -- } \ will solve the npa npb pair
  \ npa . ." npa " npb . ." npb    " cr
  npa the-pairs nget-total-pieceb@ 0 ?do
    npa i the-pairs nget-pieceb@ to test-piece
    npb the-pairs nget-total-pieceb@ 0 ?do
      npb i the-pairs nget-pieceb@ test-piece =
        if test-piece in-union-list? false =
          if
            test-piece current-test-index union! current-test-index 1 + dup to current-test-index set-max-solution
          then
        then
    loop
  loop ;

: solve-test ( -- )
  piece-a 0 the-pairs nget-pieceb@ to piece-b
  piece-a 0 union!
  piece-b 1 union!
  2 to current-test-index
  piece-a piece-b solve-a-pair
  piece-a . ." piece-a " piece-b . ." piece-b " current-test-index . ." size " cr ;

: fullsolution ( nstart nend -- ) \ top level word to solve puzzle
  page
  swap ?do  \ the first piece to place in union list and piece to use to populate find sub list to pair with other pieces
    0 5 at-xy piece-a . ." pa " max-solution . ." The current max solution!      " cr
    i to piece-a
    i the-pairs nget-total-pieceb@ 0 ?do
      piece-a i the-pairs nget-pieceb@ to piece-b
      piece-a 0 union!
      piece-b 1 union!
      2 to current-test-index
      0 0 at-xy piece-a . ." pa " piece-b . ." pb " i . ." i -- now testing!      " cr
      piece-a piece-b solve-a-pair
      current-test-index 25 >=
      if
        0 10 at-xy piece-a . ." pa " i . ." index " piece-b . ." pb " current-test-index . ." size     " cr
        0 15 at-xy ." A solution has been found!    " cr unloop unloop exit
      then
      current-test-index 24 >=
      if
        0 20 at-xy ." 24 pieces found!"
        0 21 at-xy piece-a . ." pa " i . ." index " piece-b . ." pb " current-test-index . ." size      " cr
      then
    loop
  loop ;

displaypieces heap-new constant show-it

: show-a-pair ( npair -- ) \ display npair on the board
  show-it construct
  the-pairs nget-pair@ { paira pairb }
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

: show-pairs ( nmax nmin -- ) \ display loop through nmin to nmax pairs from the-pairs object
  ?do i show-a-pair 1000 ms loop ;

: show-group ( npiece nindex -- )
  0 { paira nindex pairb -- }
  show-it construct
  paira nindex the-pairs nget-pieceb@ to pairb
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

: see-group ( npiece -- )
  dup the-pairs nget-total-pieceb@ 0 ?do dup i show-group 1000 ms loop ;

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
