
struct
    cell% field x
    cell% field y
    cell% field z
end-struct loc%

struct
    loc% field a
    loc% field b
    loc% field c
    loc% field d
    loc% field e
end-struct blc%

create pattern-list
pattern-list blc% %size 24 * dup allot erase  \ this makes an array of 24 of the blc% starting at pattern-list address
\ note when using this array bounds are not checked at all!

: pl-index! ( nvalue pattern-list-addr nindex -- )
    blc% %size * + ! ;

: pl-index@ ( pattern-list-addr nindex -- nvalue )
    blc% %size * + @ ;

: xyz! { nx ny nz npattern-list-addr nindex -- } \ store xyz into pattern-list at index nindex
    nx npattern-list-addr x nindex pl-index!
    ny npattern-list-addr y nindex pl-index!
    nz npattern-list-addr z nindex pl-index! ;

: xyz@ { npattern-list-addr nindex -- x y z } \ retreave xyz from pattern-list at index nindex
    npattern-list-addr x nindex pl-index@
    npattern-list-addr y nindex pl-index@
    npattern-list-addr z nindex pl-index@ ;
    
\ pattern-list a 0 xyz@ . . .  (  this will display the xyz values in pattern a 0 )
\ pattern-list a x blc% %size 1 * + @ .  ( this will display the x value for pattern a 1 )
\ 3 pattern-list a x 5 pl-index!  ( this will store value 3 into pattern-list a x index 5 )
\ pattern-list a x 5 pl-index@  ( this will retreave the value at pattern-list a x index 5 )
\

0 0 0 pattern-list a 0 xyz!
0 1 0 pattern-list b 0 xyz!
0 2 0 pattern-list c 0 xyz!
1 2 0 pattern-list d 0 xyz!
1 3 0 pattern-list e 0 xyz!

0 0 0 pattern-list a 1 xyz!
0 1 0 pattern-list b 1 xyz!
0 2 0 pattern-list c 1 xyz!
0 2 1 pattern-list d 1 xyz!
0 3 1 pattern-list e 1 xyz!

0 0 0 pattern-list a 2 xyz!
0 1 0 pattern-list b 2 xyz!
0 2 0 pattern-list c 2 xyz!
-1 2 0 pattern-list d 2 xyz!
-1 3 0 pattern-list e 2 xyz!

0 0 0 pattern-list a 3 xyz!
0 1 0 pattern-list b 3 xyz!
0 2 0 pattern-list c 3 xyz!
0 2 -1 pattern-list d 3 xyz!
0 3 -1 pattern-list e 3 xyz!

struct
    cell% field piece#
    cell% field piece-rot#
end-struct board%

create thepuzzle
thepuzzle board% %size  5 * 5 * 5 * dup allot erase \ this makes the working board to solve puzzle
\ remember thepuzzle is only a 5 x 5 x 5 dimentioned board% structure

: xyz-puzzle-index@ ( nthepuzzle-addr nx ny nz -- nvalue )
    \ retreieve thepuzzle structure with nx ny nz coordinates
    25 * swap
    5 * + + 
    board% %size * + @ ;

: xyz-puzzle-index! ( nvalue nthepuzzle-addr nx ny nz -- ) 
    \ store thepuzzle nvalue at nthepuzzle-addr nx ny nz location
    25 * swap
    5 * + +
    board% %size * + ! ;

: displayboard ( -- )
    cr s" X Y Z0 1 2 3 4" type cr s" **************" type
    5 0 ?DO
	5 0 ?DO
	    cr i . j . s" :" type
	    thepuzzle piece# i j 0 xyz-puzzle-index@ .
	    thepuzzle piece# i j 1 xyz-puzzle-index@ .
	    thepuzzle piece# i j 2 xyz-puzzle-index@ .
	    thepuzzle piece# i j 3 xyz-puzzle-index@ .
	    thepuzzle piece# i j 4 xyz-puzzle-index@ .
	LOOP
	cr s" **************" type
    LOOP ;
: piece-there? { nxboard nyboard nzboard -- nflag } \ nflag is false if no piece is on the board at location
    thepuzzle piece# nxboard nyboard nzboard xyz-puzzle-index@
    0 = if false else true then ;

: piece-valid? ( nvalue -- nflag ) \ nflag is false if piece is able to be places on board
    dup 4 > swap 0 < or ;

: place-piece? ( npiecerot nxboard nyboard nzboard ) \ nflag is false if piece can be placed
    0
    { npiecerot nxboard nyboard nzboard tmpflag -- nflag }    
    pattern-list a x npiecerot pl-index@ nxboard +  dup piece-valid?
    pattern-list a y npiecerot pl-index@ nyboard +  dup piece-valid? rot or 
    pattern-list a z npiecerot pl-index@ nzboard +  dup piece-valid? rot or to tmpflag
    piece-there? tmpflag or to tmpflag 
    pattern-list b x npiecerot pl-index@ nxboard +  dup piece-valid?
    pattern-list b y npiecerot pl-index@ nyboard +  dup piece-valid? rot or
    pattern-list b z npiecerot pl-index@ nzboard +  dup piece-valid? rot or tmpflag or to tmpflag
    piece-there? tmpflag or to tmpflag
    pattern-list c x npiecerot pl-index@ nxboard +  dup piece-valid?
    pattern-list c y npiecerot pl-index@ nyboard +  dup piece-valid? rot or
    pattern-list c z npiecerot pl-index@ nzboard +  dup piece-valid? rot or tmpflag or to tmpflag
    piece-there? tmpflag or to tmpflag
    pattern-list d x npiecerot pl-index@ nxboard +  dup piece-valid?
    pattern-list d y npiecerot pl-index@ nyboard +  dup piece-valid? rot or 
    pattern-list d z npiecerot pl-index@ nzboard +  dup piece-valid? rot or tmpflag or to tmpflag
    piece-there? tmpflag or to tmpflag
    pattern-list e x npiecerot pl-index@ nxboard +  dup piece-valid?
    pattern-list e y npiecerot pl-index@ nyboard +  dup piece-valid? rot or
    pattern-list e z npiecerot pl-index@ nzboard +  dup piece-valid? rot or tmpflag or to tmpflag
    piece-there? tmpflag or ;

: ponboard { npiece npiecerot nxboard nyboard nzboard }
    npiecerot nxboard nyboard nzboard place-piece?
    false = if
	npiece thepuzzle piece# 
	nxboard pattern-list a x npiecerot pl-index@ +
	nyboard pattern-list a y npiecerot pl-index@ +
	nzboard pattern-list a z npiecerot pl-index@ +
	xyz-puzzle-index!
	npiece thepuzzle piece# 
	nxboard pattern-list b x npiecerot pl-index@ +
	nyboard pattern-list b y npiecerot pl-index@ + 
	nzboard pattern-list b z npiecerot pl-index@ +
	xyz-puzzle-index!
	npiece thepuzzle piece# 
	nxboard pattern-list c x npiecerot pl-index@ +
	nyboard pattern-list c y npiecerot pl-index@ + 
	nzboard pattern-list c z npiecerot pl-index@ +
	xyz-puzzle-index!
	npiece thepuzzle piece# 
	nxboard pattern-list d x npiecerot pl-index@ +
	nyboard pattern-list d y npiecerot pl-index@ + 
	nzboard pattern-list d z npiecerot pl-index@ +
	xyz-puzzle-index!
	npiece thepuzzle piece# 
	nxboard pattern-list e x npiecerot pl-index@ +
	nyboard pattern-list e y npiecerot pl-index@ + 
	nzboard pattern-list e z npiecerot pl-index@ +
	xyz-puzzle-index!
    then
;

: clear-board ( -- ) \ clear the puzzle board 
    thepuzzle board% %size 5 * 5 * 5 * erase ;




