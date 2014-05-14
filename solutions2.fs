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
aligned cell +field ts>x
aligned cell +field ts>y
aligned cell +field ts>z
aligned cell +field ts>rot#
end-structure

: ts-new ( nx ny nz nrot# -- ts )  \ this will add a tsnode% structure to the list and returns ts ( also a "snn" )
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