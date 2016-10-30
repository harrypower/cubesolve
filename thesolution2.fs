require c:\users\philip\documents\github\cubesolve\thepiece.fs

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
