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
  protected \ ********************************************************************************************************
  struct
    cell% field piece-address
  end-struct piece-list%
  struct
    cell% field x
    cell% field y
    cell% field z
    cell% field piece-linked-list
    cell% field quantity
  end-struct piece-hole-list%
  cell% inst-var constructed?
  960 constant max-pieces
  inst-value piece-list-start \ piece-list array start
  public \ ***********************************************************************************************************
  m: ( ni organized-pieces -- naddress-of-piece ) \ retrieve address of a piece object
    piece-list% %size * piece-list-start piece-address + @
  ;m method pieces@
  m: ( np1 np2 organized-pieces -- nflag ) \ test if np1 collides with np2 ... return nflag true they intersect false they do not !
    this [current] pieces@ collision-list?
  ;m method pieces-intersect?
  m: ( nsub# npiece# organized-pieces -- nx ny nz ) \ retrieve the nsub xyz values for npiece#
    \ note this works because all piece objects can access all sub piece xyz values for all pieces ... so i just use the first object here!
    piece-list-start piece-address @ sub-piece@
  ;m method piece-xyz@
  m: ( organized-pieces -- )
    constructed? constructed? @ <>
    if  \ only do this stuff once at first use of object or if destruct was used
      piece-list% %size max-pieces * dup allocate throw dup [to-inst] piece-list-start swap erase
      max-pieces 0 ?do piece heap-new dup i piece-list% %size * piece-list-start piece-address + ! i swap new-piece! loop
      constructed? constructed? ! \ set test to show constructed once
    then
  ;m overrides construct
  m: ( organized-pieces -- )
    constructed? constructed? @ =
    if
      piece-list-start free throw
      0 constructed? ! \ reset constructed test
    then
  ;m overrides destruct

end-class organized-pieces

organized-pieces heap-new constant pieces

display-pieces heap-new constant show-it

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

10 0 show-pieces

\\\ will delete the following once the object contains the functionality of the following!
struct
  cell% field piece-address
end-struct piece-list%
create piece-list-start   \ list of addresses of piece objects
piece-list-start piece-list% %size 960 * dup allot erase

: populate-piece-list ( -- )  \ populate the complete 960 set of pieces with collision tables calculated
  960 0 ?do piece heap-new dup i piece-list% %size * piece-list-start + ! i swap new-piece! loop ;
populate-piece-list
: piece-object-address@ ( ni -- npiece-object-address ) \ simply return the address of the piece object created in populate-piece-list
  piece-list% %size * piece-list-start piece-address + @ ;
: piece-test? ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
  piece-object-address@ collision-list? ;
: piece-xyz@ ( nsub# npiece# -- nx ny nz ) \ return the x y z values of npiece# givin the nsub# of the piece
  piece-list-start piece-address @ sub-piece@ ;
  \ note this works because any piece object can return all the xyz info for any nsub# given any npiece#
  \ so i am just using the first piece object in the piece-list-start list of addresses for piece objects

create union-list    \ list of pieces for union solution
union-list piece-list% %size 960 * dup allot erase

: union! ( npiece ni -- ) \ store npiece in union list at ni location
  piece-list% %size * union-list piece-address + ! ;
: union@ ( ni -- npiece ) \ retreave npiece from union list from ni location
  piece-list% %size * union-list piece-address + @ ;

0 value current-test-index
0 value max-solution

: in-union-list? { npiece -- nflag } \ test npiece in current union-list to see if it can be added to list
  \ nflag is false if npiece can be added to list ... true if npiece can not be added to list
  current-test-index 0 > if
    false current-test-index 0 ?do i union@ npiece piece-test? or loop
  else
    false
  then ;
: set-max-solution ( -- ) \ increase max-solution if current-test-index is larger then max-solution
  current-test-index max-solution > if current-test-index to max-solution then ;

display-pieces heap-new constant show-it

: add-show-piece { npiece -- } \ simply display the board with npiece added to existing
  0 npiece piece-xyz@ npiece show-it display-piece!
  1 npiece piece-xyz@ npiece show-it display-piece!
  2 npiece piece-xyz@ npiece show-it display-piece!
  3 npiece piece-xyz@ npiece show-it display-piece!
  4 npiece piece-xyz@ npiece show-it display-piece!
  show-it show-display ;

: show-a-piece ( npiece -- ) \ simply display one piece on the board
  show-it construct
  add-show-piece ;

: show-pieces ( nmax nmin -- ) \ loop through display of nmax to nmin pieces
  ?do i show-a-piece 400 ms wait-for-key loop ;

: show-union-piece { npiece ni -- } \ simply display the board with npiece added to existing board .. piece will be called ni on board
  0 npiece piece-xyz@ ni show-it display-piece!
  1 npiece piece-xyz@ ni show-it display-piece!
  2 npiece piece-xyz@ ni show-it display-piece!
  3 npiece piece-xyz@ ni show-it display-piece!
  4 npiece piece-xyz@ ni show-it display-piece!
  show-it update-display ;

: show-union-pieces ( ni -- ) \ take the data from the current unionlist and display it
  show-it construct
  ( ni ) 0 ?do
    i union@ i show-union-piece
  loop
  show-it update-display ;
