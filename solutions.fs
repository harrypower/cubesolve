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
create theskip-list
theskip-list working-solution-list% %size total-pieces * dup allot erase

: thepieces-solution! ( nx ny nz nrot npiece -- ) \ store the piece into thepieces-solution table
    solution-list% %size * thepieces-solution + dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x ! ;

: thepieces-solution@ ( npiece -- nx ny nz nrot ) \ retreave the piece at npiece ( note npiece can not exceed total-pieces - 1 )
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

0 value skiplistindex
false value skiptestflag
: skiplist! ( nx ny nz nrot npiece# -- )
    working-solution-list% %size skiplistindex * theskip-list + dup
    thepiece# rot swap ! dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x !
    skiplistindex 1 + to skiplistindex
    skiplistindex total-pieces =
    if
	skiplistindex 1 - to skiplistindex
    then ;

: skiplistclear ( -- )
    theskip-list working-solution-list% %size total-pieces * erase
    0 to skiplistindex ;

: skiplisttest { nx ny nz nrot npiece# -- nflag } \ nflag is true only if the skip should be performed
    false to skiptestflag
    thelast-solution thepiece# @ total-pieces = false =
    if
	skiplistindex 0 ?DO
	    working-solution-list% %size i * theskip-list + dup
	    x @ nx = swap dup
	    y @ ny = swap dup
	    z @ nz = swap dup
	    rot# @ nrot = swap 
	    thepiece# @ npiece# = drop true
	    and and and and
	    true = if true to skiptestflag LEAVE then 
	LOOP
    then
    skiptestflag ;

: do-rotation-placement { nx ny nz -- }
    working-piece total-pieces <  
    if
	rotations 0 ?DO
	    i nx ny nz place-piece? \ false =
	    nx ny nz i working-piece skiplisttest or false =
	    if
		working-piece 1 + i nx ny nz ponboard
		nx ny nz i working-piece thepieces-solution!
		nx ny nz i working-piece skiplist!
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
		    \ ." skip" skiplistindex . cr
		then
	    LOOP
	LOOP
    LOOP ;

: repopulate-board ( -- )
    clear-board
    thelast-solution thepiece# @ 0 ?DO
	i 1 + i thepieces-solution@ swap 2swap rot ponboard 
    LOOP ;

0 value lastfailed-solution
: find-many-solutions { nstart -- }
    clear-board
    1 to working-piece
    skiplistclear
    1 nstart 0 0 0 place-pieceonboard
    fill-holes 
    working-piece 1 - thepieces-solution@ working-piece 1 - thelast-solution!
    thelast-solution@ skiplist!
    working-piece 1 - to lastfailed-solution
    begin
	repopulate-board
	lastfailed-solution to working-piece
	skiplistclear
	fill-holes
	working-piece total-pieces 1 - >= 
	if
	    true ." winner " nstart . cr
	else
	    \ skiplistclear
	    working-piece 1 - thepieces-solution@ working-piece 1 - thelast-solution! 
	    \ thelast-solution@ skiplist!
	    lastfailed-solution 1 - to lastfailed-solution
	    lastfailed-solution 0 < if true else false then
	then
	working-piece . thelast-solution@ . . . . . lastfailed-solution . cr 
	\ theskip-list x @ .
	\ theskip-list y @ .
	\ theskip-list z @ .
	\ theskip-list rot# @ .
	\ theskip-list thepiece# @ . cr cr \ 500 ms
    until 
;

\ ****************************
\ These next words will take the data table made from the total rotatianl combinations and
\ make groupings of rotational combinations.  This would allow groups of 2 , 3 , 4  or more so then
\ it is a matter of grouping groups that work together to solve the final puzzle
\ ****************************

