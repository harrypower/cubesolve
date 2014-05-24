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

: in-skip-list? ( nz ny nz nrot npiece -- nflag ) \ nflag is true only if data is in skip list 
    false { nx ny nz nrot npiece nflag } \ nflag is true if data is in skip-list
    skip-list snl-length@ 0 ?DO
	i skip-list snl-get dup
	sl>thepiece# @ npiece = swap dup
	sl>rot# @ nrot = swap dup
	sl>x @ nx = swap dup
	sl>y @ ny = swap 
	sl>z @ nz =
	and and and and dup to nflag true = if leave then
    LOOP nflag ;

: rerotation-placement ( nx ny nz -- nflag ) \ nflag is true only if b-solution-list is added to 
    false { nx ny nz nflag -- nflag1 } \ nflag1 is true only if list added to 
    b-solution-list snl-length@ total-pieces = false =
    if 
	rotations 0 ?DO
	    i nx ny nz place-piece? ~~ ." place a piece" cr
	    b-solution-list snl-empty? true =
	    if
		nx ny nz i 1 in-skip-list?
	    else
		nx ny nz i b-solution-list snl-last@ sl>thepiece# @ 1 + in-skip-list?
	    then
	    or false = ~~ ." skip a piece now" cr
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
		true to nflag
		LEAVE ~~ ." the leave in rerota..." cr
	    then
	LOOP
    then
    nflag ;

: refill-holes ( -- )
    x-count 0 ?DO
	y-count 0 ?DO
	    z-count 0 ?DO
		k j i piece-there? false =
		if  ~~ ." piece-there?" cr
		    k j i rerotation-placement true = 
		    if
			b-solution-list snl-length@ total-pieces = false = 
			if  ~~ ." here!" cr
			    b-solution-list snl-length@ 1 - b-solution-list snl-delete dup dup
			    sl>x @ swap dup
			    sl>y @ swap dup
			    sl>z @ swap dup
			    sl>rot# @ swap 
			    sl>thepiece# @
			    sl-new skip-list snl-append
			    snn-free
			    repopulate-board
			then
		    then
		then
	    LOOP
	LOOP
    LOOP ;

: backup-list-once ( -- ) 
    \ remove last item from a list and place it in skip list
    \ clear b list
    \ copy all nodes in a list to b list
    a-solution-list snl-length@ 1 - a-solution-list snl-delete dup dup
    skip-list snl-init
    sl>x @ swap dup
    sl>y @ swap dup
    sl>z @ swap dup
    sl>rot# @ swap
    sl>thepiece# @ sl-new
    skip-list snl-append
    snn-free
    b-solution-list snl-init
    a-solution-list snl-length@ 0 ?DO
	i a-solution-list snl-get dup
	sl>x @ swap dup
	sl>y @ swap dup
	sl>z @ swap dup
	sl>rot# @ swap
	sl>thepiece# @ 
	sl-new b-solution-list snl-append
    LOOP ;

: find-many-solutions ( -- )
    clear-board
    fill-holes
    a-solution-list snl-length@ total-pieces = false =
    if
	a-solution-list snl-last@ sl>thepiece# @  0 ?DO
	    backup-list-once 
	    repopulate-board   \ ~~ ." after repopu" cr
	    refill-holes  \ ~~ ." refill-ho" cr
	    b-solution-list snl-length@ total-pieces = if LEAVE then
	    ~~ ." backup index #" i . cr
	LOOP
    then ;



\ ****************************
\ the above back trace type method did not work or at least i do not understand how to proceed
\ The following will be what i call combination reduction brute force method
\ ****************************

begin-structure dsl%  \ this is double solution list structure
    snn% +field dsl>node  
    field: dsl>a          
    field: dsl>b          
end-structure

snl-create twolistsolutions-list  \ get room for this linked list on the heap

: dsl-new ( na nb -- dsl.snn )  \ this will add a dsl% structure to the list and returns dsl.snn
    dsl% allocate throw
    >r
    r@ dsl>node snn-init
    r@ dsl>a !
    r@ dsl>b !
    r> ;

: do-double-inner-loops 0 { ndsl>a snn snn1 -- }
    clear-board
    1 snn ts>rot# @ snn ts>x @ snn ts>y @ snn ts>z @ ponboard
    thesolutions-list snl-length@ 0 ?DO
	i thesolutions-list snl-get to snn1 snn1 ts>rot# @ snn1 ts>x @ snn1 ts>y @ snn1 ts>z @ place-piece?
	false =
	if
	    ndsl>a i dsl-new twolistsolutions-list snl-append
	then
    LOOP ;

: make-double-solution-list ( -- )
    clear-board
    thesolutions-list snl-init
    make-solutions-list
    thesolutions-list snl-length@ 0 ?DO
	i i thesolutions-list snl-get do-double-inner-loops  
    LOOP ;

snl-create fourlistsolutions-list

: do-quad-inner-loops 0 { ndsl>a snn snn1 -- }
    twolistsolutions-list snl-length@ 0 ?DO
	clear-board
	1 snn dsl>a @ thesolutions-list snl-get dup
	ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ ponboard
	2 snn dsl>b @ thesolutions-list snl-get dup
	ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ ponboard
	i twolistsolutions-list snl-get to snn1
	snn1 dsl>a @ thesolutions-list snl-get dup
	ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ place-piece?
	false =
	if
	    3 snn1 dsl>a @ thesolutions-list snl-get dup
	    ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ ponboard
	    snn1 dsl>b @ thesolutions-list snl-get dup
	    ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ place-piece?
	else
	    true \ this means this quad group does not work 
	then
	false =
	if
	    ndsl>a i dsl-new fourlistsolutions-list snl-append
	then
    LOOP ;

: make-quad-solution-list ( -- )  \ this will find list of 4 pieces on the board that work togeter
    clear-board                   \ note this will take a long time to solve.  test showed 64 per second out of 586 billion combinations so that is 290 years on the virtual box used for testing... 
    thesolutions-list snl-init
    twolistsolutions-list snl-init
    make-double-solution-list
    twolistsolutions-list snl-length@ 0 ?DO
	i i twolistsolutions-list snl-get do-quad-inner-loops
    LOOP ;

\ ********************
\ Ok the idea of  double list is good but adding a double double list takes too long to calculate
\ So now i will try to add one at a time from double to triple etc to see if this starts to reduce the list size
\ ********************

begin-structure tsl%  \ this is three solution list structure
snn% +field tsl>node
field: tsl>a
field: tsl>b
field: tsl>c
end-structure

snl-create threelistsolutions-list

: tsl-new ( na nb nc -- tsl.snn )  \ this will add a tsl% structure to the list and returns tsl.snn
    tsl% allocate throw
    >r
    r@ tsl>node snn-init
    r@ tsl>a !
    r@ tsl>b !
    r@ tsl>c !
    r> ;

: do-three-inner-loops 0 { ntsl>a snn snn1 -- }
    thesolutions-list snl-length@ 0 ?DO
	clear-board
	1 snn dsl>a @ thesolutions-list snl-get dup
	ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ ponboard
	2 snn dsl>b @ thesolutions-list snl-get dup
	ts>rot# @ swap dup ts>x @ swap dup ts>y @ swap ts>z @ ponboard
	i thesolutions-list snl-get to snn1
	snn1 ts>rot# @ snn1 ts>x @ snn1 ts>y @ snn1 ts>z @ place-piece?
	false =
	if
	    snn dsl>a @
	    snn dsl>b @
	    i tsl-new threelistsolutions-list snl-append
	then
    LOOP ;

: make-three-solution-list ( -- )
    clear-board
    thesolutions-list snl-init
    twolistsolutions-list snl-init
    make-double-solution-list
    twolistsolutions-list snl-length@ 0 ?DO
	\ utime
	i i twolistsolutions-list snl-get do-three-inner-loops
	\ utime 2swap d- d. ."  " i . cr
    LOOP ;

