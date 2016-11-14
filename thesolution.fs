require c:\users\philip\documents\github\cubesolve\thepiece.fs

: wait-for-key ( -- ) \ if keyboard is pressed pause until it is pressed again
  key?
  if
    key drop 10 ms
    begin key? until
    key drop
  then ;

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
