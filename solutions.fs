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
snl-create corner-list
snl-create noncorner-list

: isit-corner? { nx ny nz nrot -- nflag } \ nflag is true if a corner piece is found
                                                \ nflag is false if it is a noncorner piece
    nx 0 = nx 4 = or
    ny 0 = ny 4 = or
    nz 0 = nz 4 = or
    and and \ false is noncorner true is corner 
    1 nrot nx ny nz ponboard
    0 0 0 piece-there?
    0 0 4 piece-there?
    0 4 0 piece-there?
    0 4 4 piece-there?
    4 0 0 piece-there?
    4 0 4 piece-there?
    4 4 0 piece-there?
    4 4 4 piece-there?
    or or or or or or or or 
    clear-board ;

: do-inner-solutions { nx ny nz -- }  \ go through all rotations for this board location and place
    rotations 0                       \ piece in either corner-list or noncorner list if it can be placed
    ?DO
	i nx ny nz place-piece? false =
	if
	    nx ny nz i isit-corner?
	    true =
	    if
		nx ny nz i ts-new corner-list snl-append
	    else
		nx ny nz i ts-new noncorner-list snl-append
	    then
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
\ corner-list snl-length@ . \ this should produce 96
\ noncorner-list snl-lenght@ . \ this should produce 864
\ ****************************
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


