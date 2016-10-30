require c:\users\philip\documents\github\cubesolve\thepiece.fs

struct
  cell% field a-piece
end-struct piece-list%
create piece-list-start   \ list of addresses of piece objects
piece-list-start piece-list% %size 960 * dup allot erase

: populate-piece-list ( -- )  \ populate the complete 960 set of pieces with collision tables calculated
  960 0 do piece heap-new dup i piece-list% %size * piece-list-start + ! i swap newpiece! loop ;
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
  do i show-a-piece 1000 ms loop ;

: show-union-piece { npiece ni -- } \ simply display the board with npiece added to existing board .. piece will be calle ni on board
  0 npiece piece-xyz@ ni show-it displaypiece!
  1 npiece piece-xyz@ ni show-it displaypiece!
  2 npiece piece-xyz@ ni show-it displaypiece!
  3 npiece piece-xyz@ ni show-it displaypiece!
  4 npiece piece-xyz@ ni show-it displaypiece!
  show-it showdisplay ;

: show-union-pieces ( ni -- ) \ take the data from the current unionlist and display it
  show-it construct
  ( ni ) 0 do
    i union@ i show-union-piece
  loop
  show-it showdisplay ;
