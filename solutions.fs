include pieces.fs
include ffl/snl.fs
include ffl/car.fs
include ffl/scl.fs
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

8 car-create corners

snl-create corner0 
snl-create corner1
snl-create corner2
snl-create corner3
snl-create corner4
snl-create corner5 
snl-create corner6 
snl-create corner7

: create-corners ( -- )
    corner0 0 corners car-set
    corner1 1 corners car-set
    corner2 2 corners car-set
    corner3 3 corners car-set
    corner4 4 corners car-set
    corner5 5 corners car-set
    corner6 6 corners car-set
    corner7 7 corners car-set ;
create-corners

: #cpas! { nrot# nx ny nz ncornersindex -- }
    nx ny nz nrot# ts-new 
    ncornersindex corners car-get snl-append ;

: #cpas@ { ncornersindex ncpasindex -- nrot# nx ny nz }
    ncpasindex 
    ncornersindex corners car-get
    snl-get dup
    ts>rot# @ swap dup
    ts>x @ swap dup
    ts>y @ swap 
    ts>z @ ;

: isit-corner? { nx ny nz nrot -- nflag } \ nflag is true if a corner piece is found
                                                \ nflag is false if it is a noncorner piece
    1 nrot nx ny nz ponboard
    0 0 0 piece-there? dup if nrot nx ny nz 0 #cpas! then
    0 0 4 piece-there? dup if nrot nx ny nz 1 #cpas! then
    0 4 0 piece-there? dup if nrot nx ny nz 2 #cpas! then
    0 4 4 piece-there? dup if nrot nx ny nz 3 #cpas! then
    4 0 0 piece-there? dup if nrot nx ny nz 4 #cpas! then
    4 0 4 piece-there? dup if nrot nx ny nz 5 #cpas! then
    4 4 0 piece-there? dup if nrot nx ny nz 6 #cpas! then
    4 4 4 piece-there? dup if nrot nx ny nz 7 #cpas! then
    or or or or or or or  
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
\ noncorner-list snl-length@ . \ this should produce 864

\ ***********************************
\ This code will find all corner combinations that exist from corners array that contains organized corners
\ Note make-solutions-list needs to be run to make the data these next words work with
\ **********************************
begin-structure corner%  \ this is the solutions structure node
snn% +field corner>node  \ this is used to index the list of nodes 
field: corner>crnindex-0   \ index value to corners array for 1 corner
field: corner>cpasindex-0  \ index value to link list pointed to in give corners array for 1 corner
field: corner>crnindex-1
field: corner>cpasindex-1
field: corner>crnindex-2
field: corner>cpasindex-2
field: corner>crnindex-3
field: corner>cpasindex-3
field: corner>crnindex-4
field: corner>cpasindex-4
field: corner>crnindex-5
field: corner>cpasindex-5
field: corner>crnindex-6
field: corner>cpasindex-6
field: corner>crnindex-7
field: corner>cpasindex-7
end-structure

snl-create corner-solutions-list
8 car-create corner-index

: corner-new
    ( crni-0 cpasi-0 crni-1 cpasi-1 crni-2 cpasi-2 crni-3 cpasi-3 crni-4 cpasi-4 crni-5 cpasi-5 crni-6 cpasi-6 crni-7 cpasi-7 -- corner )  
    corner% allocate throw
    >r
    r@ corner>node snn-init
    r@ corner>cpasindex-7 !
    r@ corner>crnindex-7 !
    r@ corner>cpasindex-6 !
    r@ corner>crnindex-6 !
    r@ corner>cpasindex-5 !
    r@ corner>crnindex-5 !
    r@ corner>cpasindex-4 !
    r@ corner>crnindex-4 !
    r@ corner>cpasindex-3 !
    r@ corner>crnindex-3 !
    r@ corner>cpasindex-2 !
    r@ corner>crnindex-2 !
    r@ corner>cpasindex-1 !
    r@ corner>crnindex-1 !
    r@ corner>cpasindex-0 !
    r@ corner>crnindex-0 !
    r> ;

: clr-corner-index ( -- )
    8 0 ?DO 0 i corner-index car-set LOOP ;

clr-corner-index

: inc-ncorner-index ( nindex -- nflag ) \ nflag is false for no overflow true for overflow happened
    >r r@ corner-index car-get
    dup 11 =
    if
	drop 0 r@ corner-index car-set
	true 
    else
	1 + r@ corner-index car-set
	false
    then r> drop ;

7 value cornercount  \ this value is 7 for all corners or 6 one less of all corners. 
: next-corner-index ( -- nflag ) \ nflag is false if more numbers to go thru yet and true if at the end of numbers
    0
    BEGIN
	dup inc-ncorner-index
	if
	    dup cornercount =  
	    if
		drop true true
	    else
		1 + false
	    then
	else
	    drop false true 
	then
    UNTIL ;

: corner? ( -- nflag ) \ nflag is false if a corner combo was found nflag is true if not found
    try
	clear-board
	8 0 ?DO
	    i dup corner-index car-get #cpas@ place-piece?
	    false =
	    if
		i 1 + i dup corner-index car-get #cpas@ ponboard
	    else
		true throw 
	    then
	LOOP
	false
    restore	
    endtry ;

: make-corners-list ( ncornerselection -- ) \ ncornerselection is 0 to 11 only 
    6 to cornercount \ makes next-corner-index only count 7 of the 8 corners 
    clr-corner-index
    7 corner-index car-set \ this sets the 8th corner to the value ncornerselection
    begin
	corner?
	false =
	if
	    8 0 ?DO
		i i corner-index car-get 
	    LOOP
	then
	next-corner-index
    until ;
\ turns out there are 23378748 working corner combinations of 429981696 possible or 5.43% or so
\ 0 make-corners-list corner-solutions-list snl-length@ . will produce 1836247 combos or 12 of complete list
 
: dcorners ( -- ) \ test word to see the current corner-index values
    8 0 ?DO
	i corner-index car-get . 
    LOOP cr ;
: d#cpas@-list ( -- ) \ test word to see the current populated corner total combonations
    8 0 ?do
	12 0 ?do
	    j i #cpas@ j . i . . . . . cr
	loop
    loop ;

: ponboard-ncorner ( ncornerindex -- )  \ word to place a corner combination working list on the board 
    corner-solutions-list snl-get >r
    1 r@ corner>crnindex-0 @
    r@ corner>cpasindex-0 @ #cpas@ ponboard
    2 r@ corner>crnindex-1 @
    r@ corner>cpasindex-1 @ #cpas@ ponboard
    3 r@ corner>crnindex-2 @
    r@ corner>cpasindex-2 @ #cpas@ ponboard
    4 r@ corner>crnindex-3 @
    r@ corner>cpasindex-3 @ #cpas@ ponboard
    5 r@ corner>crnindex-4 @
    r@ corner>cpasindex-4 @ #cpas@ ponboard
    6 r@ corner>crnindex-5 @
    r@ corner>cpasindex-5 @ #cpas@ ponboard
    7 r@ corner>crnindex-6 @
    r@ corner>cpasindex-6 @ #cpas@ ponboard
    8 r@ corner>crnindex-7 @
    r@ corner>cpasindex-7 @ #cpas@ ponboard
    r> drop ;

scl-new value reduced-noncorner-list
scl-new value wnc-solution-list

: make-reduced-noncorner-list ( -- ) \ creates a reduced noncorner list from original non corner list 
    noncorner-list snl-length@ 0 ?DO
	clear-board
	0 ponboard-ncorner
	i noncorner-list snl-get dup
	ts>rot# @ swap dup
	ts>x @ swap dup
	ts>y @ swap 
	ts>z @
	place-piece? false =
	if
	    i reduced-noncorner-list scl-append
	then
    LOOP ;

: work-onit  ( ncornersolutions -- )
    clear-board
    ponboard-ncorner
    reduced-noncorner-list scl-length@ 0 ?DO
	i reduced-noncorner-list scl-get 
	noncorner-list snl-get dup
	ts>rot# @ swap dup
	ts>x @ swap dup
	ts>y @ swap 
	ts>z @ 
	place-piece? false =
	if
	    i wnc-solution-list scl-append
	    wnc-solution-list scl-length@ 8 +
	    i reduced-noncorner-list scl-get
	    noncorner-list snl-get dup
	    ts>rot# @ swap dup
	    ts>x @ swap dup
	    ts>y @ swap 
	    ts>z @ 
	    ponboard
	then
    LOOP ;

0 value sizenow

: do-corners&noncorners ( -- )
    corner-solutions-list snl-length@ 0 ?DO
	i work-onit
	wnc-solution-list scl-length@ 17 >=
	if
	    displayboard
	    LEAVE
	else
	    wnc-solution-list scl-length@ sizenow >
	    if
		i . wnc-solution-list scl-length@ dup . cr
		to sizenow
	    then
	    wnc-solution-list scl-clear
	then
    LOOP ;

: testone ( -- )
    make-solutions-list
    0 make-corners-list
    reduced-noncorner-list scl-clear
    wnc-solution-list scl-clear
    make-reduced-noncorner-list
    \ do-corners&noncorners
    ;

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


