
require piece_object.fs

25 constant total-pieces
24 constant total-orientations
125 constant total-locations
struct
    cell% field apiece
end-struct pieces%

create pieces-array
pieces-array pieces% %size total-pieces * dup allot erase
piece heap-new constant testpiece
variable working-pieces 0 working-pieces ! \ note working-pieces starts at 0 because there are 0 working pieces 
\ now i have an array of cell size variables to hold the piece objects
\ This array will constitute the 25 pieces to solve the puzzle.

: piece@ ( nindex -- piece ) \ return piece object address stored in pieces-array 
    pieces-array apiece pieces% %size rot * + @ ;
: piece! ( piece nindex -- ) \ store piece object address in pieces-array
    pieces-array apiece pieces% %size rot * + ! ;
: make-pieces ( -- ) \ instantate total-pieces worth of piece objects and put into pieces-array
    total-pieces 0 do
	piece heap-new i piece!
    loop ;
make-pieces
: place-piece ( norient nloc  -- nflag ) \ test npiece at norient and nloc for fit on board and with other pieces
    testpiece set-piece                  \ return true if successfull false if fit not possible
    true =
    if
	false
    else
	false
	working-pieces @ 0 ?do
	    testpiece i piece@ compair-pieces +
	loop
	false = if true else false then
    then ;

: solveit ( -- )
    total-locations 0 ?do
	total-orientations 0 ?do
	    i j place-piece true =
	    if
		i j working-pieces @ piece@ set-piece drop
		working-pieces @ 1 + working-pieces !
	    then
	    working-pieces @ total-pieces  >= if leave then 
	loop
	working-pieces @ total-pieces  >= if leave then
    loop ;

: displayboard ( -- )
    working-pieces @ 0 ?do
	i . ."  *************" 
	i piece@ print 
    loop ;

: solveit2 { nstart -- }
    total-locations total-orientations * nstart ?do
	i total-orientations mod i total-locations /
	2dup place-piece true =
	if
	    working-pieces @ piece@ set-piece drop
	    working-pieces @ 1 + working-pieces !
	else
	    2drop
	then
	working-pieces @ total-pieces >= if leave then 
    loop ;

