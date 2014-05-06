include pieces.fs

\ *************
\ these structures and words are an attempt to understand total rotational combinations
\ *************
x-count y-count * z-count * rotations * constant total-possibilitys
variable total-solutions 0 total-solutions !

loc%
    cell% field rot#
end-struct solution-list%

create thesolutions   \ a table of possible rotational solutions that can be used on the board
thesolutions solution-list% %size total-possibilitys * dup allot erase


: solutions! ( nx ny nz nrot nindex -- ) \ store rotational possible placement in solution-list%
    solution-list% %size * thesolutions + dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x ! ;

: solutions@ ( nindex -- nx ny nz nrot ) \ retreave rotational possible placemet in the solution-list% at nindex location
    \ note the list will retreave empty entry in the list as 0 rotation value
    solution-list% %size * thesolutions + dup
    x @ swap dup
    y @ swap dup
    z @ swap
    rot# @ ;

: do-inner-solutions { nindex nx ny nz -- nindex1 }

    rotations 0
    ?DO
	i nx ny nz place-piece? false =
	if
	    nx ny nz i nindex solutions!
	    nindex 1 + to nindex
	then
    LOOP
    nindex ;

: make-solutions-list ( -- ) \ populate thesolutions list
    clear-board 0
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i do-inner-solutions
	    LOOP
	LOOP
    LOOP total-solutions ! ;  \ note only here is the variable total-solutions correctly populated

\ ************
\ These next words are about solving one path of the puzzle to see how it could be done
\ ************

25 constant total-pieces \ this is the amount of pieces needed to be placed to solve puzzle
0 value working-piece  \ this will be the piece that is being worked on now
create thepieces-solution  \ a table for working on the final solutions.  Will contain a list of total-pieces as that is the amount needed to solve this puzzle
thepieces-solution solution-list% %size total-pieces * dup allot erase
solution-list%
    cell% field thepiece#
end-struct working-solution-list% 
create thelast-solution \ the last rotation solution found
thelast-solution working-solution-list% %size dup allot erase

: thepieces-solution! ( nx ny nz nrot npiece -- ) \ store the piece into thepieces-solution table
    solution-list% %size * thepieces-solution + dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x ! ;

: thepieces-solution@ ( npiece -- nx ny nz nrot ) \ retreave the piece at nindex ( note nindex can not exceed total-pieces - 1 )
    solution-list% %size * thepieces-solution + dup
    x @ swap dup
    y @ swap dup
    z @ swap
    rot# @ ;

: thelast-solution! ( nx ny nz nrot thepiece# -- )
    thelast-solution thepiece# !
    thelast-solution rot# !
    thelast-solution z !
    thelast-solution y !
    thelast-solution x ! ;

: thelast-solution@ ( -- nx ny nz nrot thepiece# )
    thelast-solution x @
    thelast-solution y @
    thelast-solution z @
    thelast-solution rot# @
    thelast-solution thepiece# @ ;

: do-rotation-placement { nx ny nz -- }
    working-piece total-pieces <  
    if
	rotations 0 ?DO
	    i nx ny nz place-piece? false =
	    if
		working-piece 1 + i nx ny nz ponboard
		nx ny nz i working-piece thepieces-solution!
		nx ny nz i working-piece thelast-solution!
		working-piece 1 + to working-piece
		LEAVE
	    then
	LOOP
    then
;

: find-first-solution ( -- )
    clear-board 
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i piece-there? false =
		if
		    k j i do-rotation-placement
		then
	    LOOP
	LOOP
    LOOP
;

: repopulate-board ( -- )
    clear-board
    thelast-solution thepiece# @ 0 ?DO
	i 1 + i thepieces-solution@ swap 2swap rot ponboard 
    LOOP ;

: fill-holes ( -- )
    thelast-solution thepiece# @ to working-piece
    \ need to iterate through the display for holes and try to solve them 

;

: isit-solved? ( -- nflag ) \ nflag is true when puzzle is solved
    thelast-solution thepiece# @ total-pieces 1 - = ;

: backup-one-piece ( -- )
    thelast-solution thepiece# @ 1 - dup >r
    thepieces-solution@ r>
    thelast-solution! ;

: find-many-solutions ( -- )
    clear-board 0 to working-piece
    find-first-solution
    begin
	repopulate-board
	fill-holes
	isit-solved? true =
	if
	    true
	else
	    backup-one-piece
	    \ false
	    true \ just for testing
	then
    until ;

