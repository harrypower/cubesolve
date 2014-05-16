include pieces.fs
include ffl/snl.fs
\ using the forth foundation library generic single linked list code
\ *******************
\ Will find the total solutions and put them in a list
\ *******************
\ This structure is one way to do it!
\ begin-structure ts%  \ this is the solutions structure node
\ snn% +field ts>node  \ this is used to index the list of nodes ( it is a generic single linked list node structure )
\     field: ts>x          \ the solutions x value
\     field: ts>y          \ ts y value
\     field: ts>z          \ ts z value
\     field: ts>rot#       \ ts rotation #
\ end-structure
\ This structure is the same as above commented out structure
begin-structure ts%
aligned snn% +field ts>node
aligned 1 cells +field ts>x
aligned 1 cells +field ts>y
aligned 1 cells +field ts>z
aligned 1 cells +field ts>rot#
end-structure

: ts-new ( nx ny nz nrot# -- ts )  \ this will add a ts% structure to the list and returns ts ( also know as "snn" )
    ts% allocate throw
    >r
    r@ ts>node snn-init
    r@ ts>rot# !
    r@ ts>z !
    r@ ts>y !
    r@ ts>x !
    r> ;

snl-create thesolutions-list  \ get room for this linked list on the heap

: do-inner-solutions { nx ny nz -- }  \ go through all rotations for this board location and place 
    rotations 0                       \ piece in thesolutions-list if it can be placed
    ?DO
	i nx ny nz place-piece? false =
	if
	    nx ny nz i ts-new thesolutions-list snl-append
	then
    LOOP ;

: make-solutions-list ( -- ) \ search each board location for piece solutions and place in thesolutions-list
    clear-board 
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i do-inner-solutions
	    LOOP
	LOOP
    LOOP ;  

\ thesolutions-list snl-length@ .   \ this should produce 960 as that is how many solutions there are
\ 5 thesolutions-list snl-get ts>rot# @ .  \ this should produce the rotaion# for the sixth list entry ( 10 )

\ ***************************
\ Next words will try to do a back trace type method by first making a list of one solution and then back step and iterate
\ ***************************
25 constant total-pieces \ this is the amount of pieces needed to be placed to solve puzzle

begin-structure sl%
aligned snn% +field sl>node
aligned 1 cells +field sl>x
aligned 1 cells +field sl>y
aligned 1 cells +field sl>z
aligned 1 cells +field sl>rot#
aligned 1 cells +field sl>thepiece#
end-structure

snl-create a-solution-list a-solution-list snl-init
snl-create skip-list skip-list snl-init
snl-create b-solution-list b-solution-list snl-init

: sl-new ( nx ny nz nrot npiece -- sl ) \ will add a sl% structure to the list and returns sl ( a "snn" )
    sl% allocate throw
    >r
    r@ sl>node snn-init
    r@ sl>thepiece# !
    r@ sl>rot# !
    r@ sl>z !
    r@ sl>y !
    r@ sl>x !
    r> ;

: do-rotation-placement { nx ny nz -- }
    a-solution-list snl-length@ total-pieces = false =
    if
	rotations 0 ?DO
	    i nx ny nz place-piece? false =
	    if
		a-solution-list snl-empty? false =
		if
		    a-solution-list snl-last@ sl>thepiece# @ 1 + 
		    i nx ny nz ponboard
		    nx ny nz i a-solution-list snl-last@ sl>thepiece# @ 1 +
		else
		    1 i nx ny nz ponboard
		    nx ny nz i 1 
		then
		sl-new a-solution-list snl-append
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
    a-solution-list snl-length@ 0 ?DO
	i a-solution-list snl-get dup
	sl>thepiece# @ swap dup
	sl>rot# @ swap dup
	sl>x @ swap dup
	sl>y @ swap 
	sl>z @ ponboard
    LOOP ;

: in-skip-list? ( nx ny nz nrot npiece -- nflag ) \ nflag is true if data is in skip-list
    drop drop drop drop drop 
    false
;

: rerotation-placement { nx ny nz -- }
    b-solution-list snl-length@ total-pieces = false =
    if
	rotations 0 ?DO
	    i nx ny nz place-piece? 
	    nx ny nz i b-solution-list snl-last@ sl>thepiece# @ 1 + in-skip-list?
	    or false = 
	    if
		b-solution-list snl-empty? false =
		if
		    b-solution-list snl-last@ sl>thepiece# @ 1 +
		    i nx ny nz ponboard
		    nx ny nz i b-solution-list snl-last@ sl>thepiece# @ 1 +
		else
		    1 i nx ny nz ponboard
		    nx ny nz i 1
		then
		sl-new b-solution-list snl-append
		LEAVE
	    then
	LOOP
    then ;

: refill-holes ( -- )
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i piece-there? false =
		if
		    k j i rerotation-placement
		    b-solution-list snl-length@ total-pieces =
		    if
			LEAVE \ bail solution found
		    else
			b-solution-list snl-length@ 1 - b-solution-list snl-delete
			skip-list snl-append
		    then
		then
	    LOOP
	LOOP
    LOOP ;

: backup-list-once ( -- )
    \ remove last item from a list and place it in skip list
    \ clear b list
    \ copy all nodes in a list to b list
    a-solution-list snl-length@ 1 - a-solution-list snl-delete
    skip-list snl-init
    skip-list snl-append
    b-solution-list snl-init
    a-solution-list snl-length@ 0 ?DO
	i a-solution-list snl-get dup
	sl>x @ swap dup
	sl>y @ swap dup
	sl>z @ swap dup
	sl>rot# @ swap
	sl>thepiece# @ sl-new 
	b-solution-list snl-append
    LOOP ;

: find-many-solutions ( -- )
    clear-board
    fill-holes
    a-solution-list snl-length@ total-pieces = false =
    if
	a-solution-list snl-last@ sl>thepiece# @ 1 - 0 ?DO
	    backup-list-once
	    repopulate-board
	    refill-holes
	    b-solution-list snl-length@ total-pieces = if LEAVE then
	    i . cr
	LOOP
    then ;

    