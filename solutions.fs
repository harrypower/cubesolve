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
\ A flaw with this method is back tracing for solutions means you need to remember multipul failed
\ piece placement to find the combinations that work. This is not a good way to do it!

25 constant total-pieces \ this is the amount of pieces needed to be placed to solve puzzle
0 value working-piece  \ this will be the piece that is being worked on now
create thepieces-solution  \ a table for working on the final solutions.  Will contain a list of total-pieces as that is the amount needed to solve this puzzle
thepieces-solution solution-list% %size total-pieces * dup allot erase
create thelastpieces-solution  \ a table that will be the working copy of the final solutions .
thelastpieces-solution solution-list% %size total-pieces * dup allot erase

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

: skiptest ( nx ny nz nrot -- nflag ) \ nflag is true only if the skip should be performed
    thelast-solution thepiece# @ total-pieces = 
    if
	2drop 2drop false
    else
	thelast-solution rot# @ = swap
	thelast-solution z @ = and swap
	thelast-solution y @ = and swap
	thelast-solution x @ = and
	working-piece thelast-solution thepiece# @ = and 
    then ;

: do-rotation-placement { nx ny nz -- }
    working-piece total-pieces <  
    if
	rotations 0 ?DO
	    i nx ny nz place-piece? \ false =
	    nx ny nz i skiptest or false = 
	    if
		working-piece 1 + i nx ny nz ponboard
		nx ny nz i working-piece thepieces-solution!
		working-piece 1 + to working-piece
		LEAVE
	    then
	LOOP
    then ;

: fill-holes ( -- )
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i piece-there? false =
		if
		    k j i do-rotation-placement
		then
	    LOOP
	LOOP
    LOOP ;

: repopulate-board ( -- )
    clear-board
    thelast-solution thepiece# @ 0 ?DO
	i 1 + i thepieces-solution@ swap 2swap rot ponboard 
    LOOP ;

: find-many-solutions ( -- )
    clear-board
    0 to working-piece
    total-pieces thelast-solution thepiece# ! \ to ensure skip test is not done for first solution finding
    fill-holes 
    working-piece 1 - thepieces-solution@ working-piece 1 - thelast-solution!
    begin
	repopulate-board
	working-piece 1 - to working-piece
	fill-holes
	working-piece total-pieces >= 
	if
	    true
	else
	    working-piece 1 - thepieces-solution@ working-piece 1 - thelast-solution! 
	    false
	then
	working-piece . thelast-solution@ . . . . . cr 500 ms
    until
;

\ ****************************
\ These next words will take the data table made from the total rotatianl combinations and
\ make groupings of rotational combinations.  This would allow groups of 2 , 3 , 4  or more so then
\ it is a matter of grouping groups that work together to solve the final puzzle
\ ****************************

