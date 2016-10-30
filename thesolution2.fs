require c:\users\philip\documents\github\cubesolve\thepiece.fs

struct
  cell% field apiece
end-struct plist%
create pliststart   \ list of addresses of piece objects
pliststart plist% %size 960 * dup allot erase

: populate-piece-list ( -- )
  960 0 do piece heap-new dup i plist% %size * pliststart + ! i swap newpiece! loop ;
poppiecelist
: piece-test ( np1 np2 -- nflag ) \ test if np1 intersects np2 if nflag is true then they intersect if false they do not intersect
  0 { np1 np2 tempaddr }
  np1 np2 plist% %size * pliststart apiece + @ dup to tempaddr collisionlist? ;
: piece-xyz@ ( nsub# npiece# -- nx ny nz ) \ return the x y z values of npiece# givin the nsub# of the piece
  pliststart apiece @ subpiece@ ;

create unionlist    \ list of pieces for union solution
unionlist plist% %size 960 * dup allot erase

: union! ( npiece ni -- ) \ store npiece in union list at ni location
  plist% %size * unionlist apiece + ! ;
: union@ ( ni -- npiece ) \ retreave npiece from union list from ni location
  plist% %size * unionlist apiece + @ ;
