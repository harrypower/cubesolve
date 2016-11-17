require c:\users\philip\documents\github\cubesolve\thepiece.fs

: wait-for-key ( -- ) \ if keyboard is pressed pause until it is pressed again
  key?
  if
    key drop 10 ms
    begin key? until
    key drop
  then ;

\ organized pieces object *******************************************************************************************************
object class
  destruction implementation
  selector pieces-intersect?
  selector pieces@
  selector piece-xyz@
  protected \ ********************************************************************************************************
  struct
    cell% field piece-address
  end-struct piece-list%
  piece-list%
    cell% field next-link
  end-struct pieces-link%
  struct
    cell% field pieces-link-list
    cell% field quantity
    cell% field index
  end-struct pieces-for-holes%
  cell% inst-var constructed?
  960 constant max-pieces
  5 constant hole-size
  inst-value piece-list-start \ piece-list array start
  inst-value union-list \ solution union list start
  inst-value current-union-index
  inst-value max-solution
  inst-value holes-list \ holes for pieces list start
  public \ ***********************************************************************************************************
  m: ( organized-pieces -- nlink-address ) \ allocate memory for the piece link
    pieces-link% %size dup allocate throw dup rot erase
  ;m method allocate-piece-link
  m: ( nx ny nz organized-pieces -- uholes-list-address )
    hole-size hole-size * * swap hole-size * + + pieces-for-holes% %size * holes-list + \ calculate address for hole place to store
  ;m method calculate-holes-address
  m: ( npiece nx ny nz organized-pieces -- ) \ store npiece at hole location nx ny nz in the next list spot and update the list information
    this [current] calculate-holes-address
    this [current] allocate-piece-link { uhole-address upiece-link-address }
    upiece-link-address piece-address !
    uhole-address quantity @ 0 =
    if
      upiece-link-address uhole-address pieces-link-list !
      1 uhole-address quantity !
    else
      uhole-address pieces-link-list @
      begin
        dup next-link @ dup 0 =
        if drop true
        else swap drop false
        then
      until
      upiece-link-address swap next-link !
      uhole-address quantity @ 1+ uhole-address quantity !
    then
  ;m method hole!
  m: ( nx ny nz organized-pieces -- npiece ) \ retreave the next piece from nx ny nz hole list
    \ npiece will be true if there are no pieces to retreave or the list has been indexed through one cycle and is going to restart next call
    this [current] calculate-holes-address dup dup dup index @ swap quantity @ rot pieces-link-list @ { uhole-address uindex uquantity uaddr }
    uquantity 0 =
    if
      true
    else
      uindex uquantity >=
      if
        0 uhole-address index ! \ start index at begining
        true \ tell caller at end of linked list
      else
        uaddr uindex 0 ?do next-link @ loop piece-address @
        uindex 1+ uhole-address index ! \ update index for next call
      then
    then
  ;m method hole@
  m: ( nx ny nz organized-pieces -- nquantity ) \ retreave quantity of the link lists stored at nx ny nz hole address
    this [current] calculate-holes-address
    quantity @
  ;m method hole-pieces-quantity@
  m: ( -- ) \ populate the holes piece list
    max-pieces 0 ?do
      i 0 i this piece-xyz@
      this hole!
    loop
  ;m method populate-hole-pieces
  m: ( -- ) \ deallocate memory allocated in the holes piece list
    hole-size 0 ?do
      hole-size 0 ?do
        hole-size 0 ?do
          i j k this calculate-holes-address pieces-link-list @
          dup 0 <>
          if
            begin \ pl
              dup \ pl pl
              next-link @ \ pl nl
              swap free throw \ nl
              dup 0 = \ nl nf
            until
          then
          drop
        loop
      loop
    loop
  ;m method depopulate-hole-pieces
  m: ( npiece ni organized-pieces -- ) \ store npiece in union list at ni location
    piece-list% %size * union-list piece-address + ! ;m method union!
  m: ( ni organized-pieces -- npiece ) \ retreave npiece from union list from ni location
    piece-list% %size * union-list piece-address + @ ;m method union@
  m: ( ni organized-pieces -- naddress-of-piece ) \ retrieve address of a piece object at index ni
    piece-list% %size * piece-list-start piece-address + @ ;m overrides pieces@
  m: ( npiece organized-pieces -- nflag ) \ test npiece in current union-list to see if it can be added to list
    \ nflag is false if npiece can be added to list ... true if npiece can not be added to list
    { npiece -- nflag }
    false current-union-index 0 ?do i this [current] union@ npiece this pieces-intersect? or loop
  ;m method in-union-list?
  m: ( organized-pieces -- ) \ increase max-solution if current-union-index is larger then max-solution
    current-union-index max-solution > if current-union-index [to-inst] max-solution then
  ;m method set-max-solution
  m: ( npiece organized-pieces -- )
    dup this [current] in-union-list? false =
    if
      current-union-index this [current] union!
      current-union-index 1+ [to-inst] current-union-index
      this [current] set-max-solution
    else
      drop
    then
  ;m method add-piece-to-union-list
  m: ( organized-pieces -- nsize ) \ return the size of the union list
    current-union-index ;m method union-size@
  m: ( ni organized-pieces -- ) \ change size of union list
    [to-inst] current-union-index ;m method union-size!
  m: ( np1 np2 organized-pieces -- nflag ) \ test if np1 collides with np2 ... return nflag true they intersect false they do not !
      this pieces@ collision-list?  ;m overrides pieces-intersect?
  m: ( nsub# npiece# organized-pieces -- nx ny nz ) \ retrieve the nsub xyz values for npiece#
    \ note this works because all piece objects can access all sub piece xyz values for all pieces ... so i just use the first object here!
    piece-list-start piece-address @ sub-piece@ ;m overrides piece-xyz@
  m: ( organized-pieces -- )
    constructed? constructed? @ <>
    if  \ only do this stuff once at first use of object or if destruct was used
      piece-list% %size max-pieces * dup allocate throw dup [to-inst] piece-list-start swap erase \ make room for piece-list
      max-pieces 0 ?do piece heap-new dup i piece-list% %size * piece-list-start piece-address + ! i swap new-piece! loop \ populate it
      piece-list% %size max-pieces * dup allocate throw dup [to-inst] union-list swap erase \ make room for union-list
      pieces-for-holes% %size hole-size hole-size * hole-size * * dup allocate throw dup [to-inst] holes-list swap erase \ make room for hole array ( x y z )
      this populate-hole-pieces
      constructed? constructed? ! \ set test to show constructed once
    then
    0 [to-inst] current-union-index
    0 [to-inst] max-solution
  ;m overrides construct
  m: ( organized-pieces -- )
    constructed? constructed? @ =
    if
      piece-list-start free throw
      union-list free throw
      this depopulate-hole-pieces
      holes-list free throw
      0 constructed? ! \ reset constructed test
    then
    0 [to-inst] current-union-index
    0 [to-inst] max-solution
    0 [to-inst] piece-list-start
    0 [to-inst] union-list
    0 [to-inst] holes-list
  ;m overrides destruct
  m: ( organized-pieces -- )
    cr this [parent] print cr
    current-union-index . ." current-union-index " cr
    max-solution . ." max-solution" cr
    piece-list-start . ." piece-list-start"  cr
    union-list . ." union-list" cr
    holes-list . ." holes-list" cr
  ;m overrides print
end-class organized-pieces

organized-pieces heap-new constant pieces

display-pieces heap-new constant show-it
display-pieces heap-new constant work-it

: find-hole ( -- ux uy uz )
  5 0 ?do
    5 0 ?do
      5 0 ?do
        i j k work-it display-piece@ true =
        if i j k unloop unloop unloop exit then 
      loop
    loop
  loop ;

: add-show-piece { npiece -- } \ simply display the board with npiece added to existing
  0 npiece pieces piece-xyz@ npiece show-it display-piece!
  1 npiece pieces piece-xyz@ npiece show-it display-piece!
  2 npiece pieces piece-xyz@ npiece show-it display-piece!
  3 npiece pieces piece-xyz@ npiece show-it display-piece!
  4 npiece pieces piece-xyz@ npiece show-it display-piece!
  show-it show-display ;

: show-a-piece ( npiece -- ) \ simply display one piece on the board
  show-it construct
  add-show-piece ;

: show-pieces ( nmax nmin -- ) \ loop through display of nmax to nmin pieces
  ?do i show-a-piece 400 ms wait-for-key loop ;

: show-union-piece { npiece ni -- } \ simply display the board with npiece added to existing board .. piece will be called ni on board
  0 npiece pieces piece-xyz@ ni show-it display-piece!
  1 npiece pieces piece-xyz@ ni show-it display-piece!
  2 npiece pieces piece-xyz@ ni show-it display-piece!
  3 npiece pieces piece-xyz@ ni show-it display-piece!
  4 npiece pieces piece-xyz@ ni show-it display-piece!
  show-it update-display ;

: show-union-pieces ( ni -- ) \ take the data from the current unionlist and display it
  show-it construct
  ( ni ) 0 ?do
    i pieces union@ i show-union-piece
  loop
  show-it update-display ;

: show-hole-pieces { ux uy uz -- } \ display the hole list pieces
  begin
    ux uy uz pieces hole@
    dup true <>
  while
    show-a-piece
    400 ms wait-for-key
  repeat
  drop ;

: show-all-hole-pieces ( -- )
  5 0 ?do
    5 0 ?do
      5 0 ?do
        i j k show-hole-pieces
      loop
    loop
  loop ;
\\\
\ 10 0 show-pieces
0 pieces add-piece-to-union-list
8 pieces add-piece-to-union-list
page
pieces union-size@ show-union-pieces
