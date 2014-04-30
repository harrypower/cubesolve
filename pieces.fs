24 constant rotations

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
pattern-list blc% %size rotations * dup allot erase  \ this makes an array of 24 of the blc% starting at pattern-list address
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
\ *************************
0 0 0 pattern-list a 4 xyz!
0 0 1 pattern-list b 4 xyz!
0 0 2 pattern-list c 4 xyz!
0 1 2 pattern-list d 4 xyz!
0 1 3 pattern-list e 4 xyz!

0 0 0 pattern-list a 5 xyz!
0 0 1 pattern-list b 5 xyz!
0 0 2 pattern-list c 5 xyz!
-1 0 2 pattern-list d 5 xyz!
-1 0 3  pattern-list e 5 xyz!

0 0 0 pattern-list a 6 xyz!
0 0 1 pattern-list b 6 xyz!
0 0 2 pattern-list c 6 xyz!
0 -1 2 pattern-list d 6 xyz!
0 -1 3 pattern-list e 6 xyz!

0 0 0 pattern-list a 7 xyz!
0 0 1 pattern-list b 7 xyz!
0 0 2 pattern-list c 7 xyz!
1 0 2 pattern-list d 7 xyz!
1 0 3 pattern-list e 7 xyz!
\ *************************
0 0 0 pattern-list a 8 xyz!
1 0 0  pattern-list b 8 xyz!
2 0 0 pattern-list c 8 xyz!
2 -1 0 pattern-list d 8 xyz!
3 -1 0 pattern-list e 8 xyz!

0 0 0 pattern-list a 9 xyz!
1 0 0 pattern-list b 9 xyz!
2 0 0 pattern-list c 9 xyz!
2 0 1 pattern-list d 9 xyz!
3 0 1  pattern-list e 9 xyz!

0 0 0 pattern-list a 10 xyz!
1 0 0 pattern-list b 10 xyz!
2 0 0 pattern-list c 10 xyz!
2 1 0 pattern-list d 10 xyz!
3 1 0 pattern-list e 10 xyz!

0 0 0 pattern-list a 11 xyz!
1 0 0 pattern-list b 11 xyz!
2 0 0 pattern-list c 11 xyz!
2 0 -1 pattern-list d 11 xyz!
3 0 -1 pattern-list e 11 xyz!
\ *************************
0 0 0 pattern-list a 12 xyz!
0 -1 0 pattern-list b 12 xyz!
0 -2 0 pattern-list c 12 xyz!
1 -2 0 pattern-list d 12 xyz!
1 -3 0 pattern-list e 12 xyz!

0 0 0 pattern-list a 13 xyz!
0 -1 0 pattern-list b 13 xyz!
0 -2 0 pattern-list c 13 xyz!
0 -2 1 pattern-list d 13 xyz!
0 -3 1 pattern-list e 13 xyz!

0 0 0 pattern-list a 14 xyz!
0 -1 0 pattern-list b 14 xyz!
0 -2 0 pattern-list c 14 xyz!
-1 -2 0 pattern-list d 14 xyz!
-1 -3 0 pattern-list e 14 xyz!

0 0 0 pattern-list a 15 xyz!
0 -1 0 pattern-list b 15 xyz!
0 -2 0 pattern-list c 15 xyz!
0 -2 -1 pattern-list d 15 xyz!
0 -3 -1 pattern-list e 15 xyz!
\ *************************
0 0 0 pattern-list a 16 xyz!
0 0 -1 pattern-list b 16 xyz!
0 0 -2 pattern-list c 16 xyz!
0 1 -2 pattern-list d 16 xyz!
0 1 -3 pattern-list e 16 xyz!

0 0 0 pattern-list a 17 xyz!
0 0 -1 pattern-list b 17 xyz!
0 0 -2 pattern-list c 17 xyz!
-1 0 -2 pattern-list d 17 xyz!
-1 0 -3  pattern-list e 17 xyz!

0 0 0 pattern-list a 18 xyz!
0 0 -1 pattern-list b 18 xyz!
0 0 -2 pattern-list c 18 xyz!
0 -1 -2 pattern-list d 18 xyz!
0 -1 -3 pattern-list e 18 xyz!

0 0 0 pattern-list a 19 xyz!
0 0 -1 pattern-list b 19 xyz!
0 0 -2 pattern-list c 19 xyz!
1 0 -2 pattern-list d 19 xyz!
1 0 -3 pattern-list e 19 xyz!
\ *************************
0 0 0 pattern-list a 20 xyz!
-1 0 0  pattern-list b 20 xyz!
-2 0 0 pattern-list c 20 xyz!
-2 -1 0 pattern-list d 20 xyz!
-3 -1 0 pattern-list e 20 xyz!

0 0 0 pattern-list a 21 xyz!
-1 0 0 pattern-list b 21 xyz!
-2 0 0 pattern-list c 21 xyz!
-2 0 1 pattern-list d 21 xyz!
-3 0 1  pattern-list e 21 xyz!

0 0 0 pattern-list a 22 xyz!
-1 0 0 pattern-list b 22 xyz!
-2 0 0 pattern-list c 22 xyz!
-2 1 0 pattern-list d 22 xyz!
-3 1 0 pattern-list e 22 xyz!

0 0 0 pattern-list a 23 xyz!
-1 0 0 pattern-list b 23 xyz!
-2 0 0 pattern-list c 23 xyz!
-2 0 -1 pattern-list d 23 xyz!
-3 0 -1 pattern-list e 23 xyz!
\ *************************

5 constant x-count
5 constant y-count
5 constant z-count

struct
    cell% field piece#
    cell% field piece-rot#
end-struct board%

create thepuzzle
thepuzzle board% %size  x-count * y-count * z-count * dup allot erase \ this makes the working board to solve puzzle
\ remember thepuzzle is only a 5 x 5 x 5 dimentioned board% structure

: xyz-puzzle-index@ ( nthepuzzle-addr nx ny nz -- nvalue )
    \ retreieve thepuzzle structure with nx ny nz coordinates
    z-count y-count * * swap
    y-count * + + 
    board% %size * + @ ;

: xyz-puzzle-index! ( nvalue nthepuzzle-addr nx ny nz -- ) 
    \ store thepuzzle nvalue at nthepuzzle-addr nx ny nz location
    z-count y-count * * swap
    y-count * + +
    board% %size * + ! ;

: displayboard ( -- )
    cr s" X Y Z0 1 2 3 4" type cr s" **************" type
    x-count 0 ?DO
	y-count 0 ?DO
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

: place-piece? ( npiecerot nxboard nyboard nzboard -- nflag ) \ nflag is false if piece can be placed
    0
    { npiecerot nxboard nyboard nzboard tmpflag }    
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

: ponboard { npiece npiecerot nxboard nyboard nzboard -- }
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
    xyz-puzzle-index! ;

: place-pieceonboard { npiece npiecerot nxboard nyboard nzboard -- }
    npiecerot nxboard nyboard nzboard place-piece?
    false = if
	npiece npiecerot nxboard nyboard nzboard ponboard
    then ;

: clear-board ( -- ) \ clear the puzzle board 
    thepuzzle board% %size x-count * y-count * z-count * erase ;


x-count y-count * z-count * rotations * constant total-possibilitys
variable total-solutions 0 total-solutions !

loc%
    cell% field rot#
end-struct solution-list%

create thesolutions
thesolutions solution-list% %size total-possibilitys * dup allot erase


: solutions! ( nx ny nz nrot nindex -- )
    solution-list% %size * thesolutions + dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x ! ;

: solutions@ ( nindex -- nx ny nz nrot )
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
    LOOP total-solutions ! ;
