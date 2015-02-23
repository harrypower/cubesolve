require objects.fs

object class
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
    pattern-list blc% %size rotations * dup allot erase
    cell% inst-var piece-orientation
    cell% inst-var piece-location
    cell% inst-var piece-ax
    cell% inst-var piece-ay
    cell% inst-var piece-az
  protected
    m: ( piece -- ) \ convert piece-location to xyz values and store them
	piece-location @ 25 / piece-az !
	piece-location @ 25 mod 5 / piece-ay !
	piece-location @ 25 mod 5 mod piece-ax ! ;m method loc>xyz!
    m: ( nvalue pattern-list-addr nindex piece -- )
	blc% %size * + ! ;m method pl-index!
    m: ( pattern-list-addr nindex piece -- nvalue )
	blc% %size * + @ ;m method pl-index@
    m: ( nx ny nz npattern-list-addr nindex piece -- )
	{ nx ny nz npattern-list-addr nindex } \ store xyz into pattern-list at index nindex
	nx npattern-list-addr x nindex this pl-index!
	ny npattern-list-addr y nindex this pl-index!
	nz npattern-list-addr z nindex this pl-index! ;m method xyz!
    m: ( npattern-list-addr nindex piece -- x y z )
	{ npattern-list-addr nindex } \ retreave xyz from pattern-list at index nindex
	npattern-list-addr x nindex this pl-index@
	npattern-list-addr y nindex this pl-index@
	npattern-list-addr z nindex this pl-index@ ;m method xyz@
    m: ( nxa nya nza nxb nyb nzb piece -- nflag ) \ compair two sets of xyz coordinates
	{ nxa nya nza nxb nyb nzb }              \ nflag is true if match found only 
	nxa nxb =                                \ nflag is false if no match found
	nya nyb =
	nza nzb =
	and and ;m method testxyz
    m: ( nvalue piece -- nflag ) \ nvalue is tested for > 4 or < 0
	dup 4 > swap 0 < or ;m method boundry-test
  public
    m: ( nblock piece -- nx ny nz ) \ produces the block xyz values given the piece orientation and location
	case
	    0 of pattern-list a endof
	    1 of pattern-list b endof
	    2 of pattern-list c endof
	    3 of pattern-list d endof
	    4 of pattern-list e endof
	    s" nblock can only be 0 to 4" exception throw
	endcase
	piece-orientation @ this xyz@
	piece-az @ + rot
	piece-ax @ + rot
	piece-ay @ + rot ;m method blockxyz@
    m: ( piece -- nflag ) \ test bound for 5x5x5 board
	0
	5 0 do
	    i this blockxyz@
	    this boundry-test rot
	    this boundry-test rot
	    this boundry-test or or or
	loop ;m method inboard-test
    m: ( piece -- ) \ constructor populates the pattern-list
	0 0 0 pattern-list a 0 this xyz!
	0 1 0 pattern-list b 0 this xyz!
	0 2 0 pattern-list c 0 this xyz!
	1 2 0 pattern-list d 0 this xyz!
	1 3 0 pattern-list e 0 this xyz!
	
	0 0 0 pattern-list a 1 this xyz!
	0 1 0 pattern-list b 1 this xyz!
	0 2 0 pattern-list c 1 this xyz!
	0 2 1 pattern-list d 1 this xyz!
	0 3 1 pattern-list e 1 this xyz!
	
	0 0 0 pattern-list a 2 this xyz!
	0 1 0 pattern-list b 2 this xyz!
	0 2 0 pattern-list c 2 this xyz!
	-1 2 0 pattern-list d 2 this xyz!
	-1 3 0 pattern-list e 2 this xyz!
	
	0 0 0 pattern-list a 3 this xyz!
	0 1 0 pattern-list b 3 this xyz!
	0 2 0 pattern-list c 3 this xyz!
	0 2 -1 pattern-list d 3 this xyz!
	0 3 -1 pattern-list e 3 this xyz!
	\ *************************
	0 0 0 pattern-list a 4 this xyz!
	0 0 1 pattern-list b 4 this xyz!
	0 0 2 pattern-list c 4 this xyz!
	0 1 2 pattern-list d 4 this xyz!
	0 1 3 pattern-list e 4 this xyz!
	
	0 0 0 pattern-list a 5 this xyz!
	0 0 1 pattern-list b 5 this xyz!
	0 0 2 pattern-list c 5 this xyz!
	-1 0 2 pattern-list d 5 this xyz!
	-1 0 3  pattern-list e 5 this xyz!
	
	0 0 0 pattern-list a 6 this xyz!
	0 0 1 pattern-list b 6 this xyz!
	0 0 2 pattern-list c 6 this xyz!
	0 -1 2 pattern-list d 6 this xyz!
	0 -1 3 pattern-list e 6 this xyz!
	
	0 0 0 pattern-list a 7 this xyz!
	0 0 1 pattern-list b 7 this xyz!
	0 0 2 pattern-list c 7 this xyz!
	1 0 2 pattern-list d 7 this xyz!
	1 0 3 pattern-list e 7 this xyz!
	\ *************************
	0 0 0 pattern-list a 8 this xyz!
	1 0 0  pattern-list b 8 this xyz!
	2 0 0 pattern-list c 8 this xyz!
	2 -1 0 pattern-list d 8 this xyz!
	3 -1 0 pattern-list e 8 this xyz!
	
	0 0 0 pattern-list a 9 this xyz!
	1 0 0 pattern-list b 9 this xyz!
	2 0 0 pattern-list c 9 this xyz!
	2 0 1 pattern-list d 9 this xyz!
	3 0 1  pattern-list e 9 this xyz!
	
	0 0 0 pattern-list a 10 this xyz!
	1 0 0 pattern-list b 10 this xyz!
	2 0 0 pattern-list c 10 this xyz!
	2 1 0 pattern-list d 10 this xyz!
	3 1 0 pattern-list e 10 this xyz!
	
	0 0 0 pattern-list a 11 this xyz!
	1 0 0 pattern-list b 11 this xyz!
	2 0 0 pattern-list c 11 this xyz!
	2 0 -1 pattern-list d 11 this xyz!
	3 0 -1 pattern-list e 11 this xyz!
	\ *************************
	0 0 0 pattern-list a 12 this xyz!
	0 -1 0 pattern-list b 12 this xyz!
	0 -2 0 pattern-list c 12 this xyz!
	1 -2 0 pattern-list d 12 this xyz!
	1 -3 0 pattern-list e 12 this xyz!
	
	0 0 0 pattern-list a 13 this xyz!
	0 -1 0 pattern-list b 13 this xyz!
	0 -2 0 pattern-list c 13 this xyz!
	0 -2 1 pattern-list d 13 this xyz!
	0 -3 1 pattern-list e 13 this xyz!
	
	0 0 0 pattern-list a 14 this xyz!
	0 -1 0 pattern-list b 14 this xyz!
	0 -2 0 pattern-list c 14 this xyz!
	-1 -2 0 pattern-list d 14 this xyz!
	-1 -3 0 pattern-list e 14 this xyz!
	
	0 0 0 pattern-list a 15 this xyz!
	0 -1 0 pattern-list b 15 this xyz!
	0 -2 0 pattern-list c 15 this xyz!
	0 -2 -1 pattern-list d 15 this xyz!
	0 -3 -1 pattern-list e 15 this xyz!
	\ *************************
	0 0 0 pattern-list a 16 this xyz!
	0 0 -1 pattern-list b 16 this xyz!
	0 0 -2 pattern-list c 16 this xyz!
	0 1 -2 pattern-list d 16 this xyz!
	0 1 -3 pattern-list e 16 this xyz!
	
	0 0 0 pattern-list a 17 this xyz!
	0 0 -1 pattern-list b 17 this xyz!
	0 0 -2 pattern-list c 17 this xyz!
	-1 0 -2 pattern-list d 17 this xyz!
	-1 0 -3  pattern-list e 17 this xyz!
	
	0 0 0 pattern-list a 18 this xyz!
	0 0 -1 pattern-list b 18 this xyz!
	0 0 -2 pattern-list c 18 this xyz!
	0 -1 -2 pattern-list d 18 this xyz!
	0 -1 -3 pattern-list e 18 this xyz!
	
	0 0 0 pattern-list a 19 this xyz!
	0 0 -1 pattern-list b 19 this xyz!
	0 0 -2 pattern-list c 19 this xyz!
	1 0 -2 pattern-list d 19 this xyz!
	1 0 -3 pattern-list e 19 this xyz!
	\ *************************
	0 0 0 pattern-list a 20 this xyz!
	-1 0 0  pattern-list b 20 this xyz!
	-2 0 0 pattern-list c 20 this xyz!
	-2 -1 0 pattern-list d 20 this xyz!
	-3 -1 0 pattern-list e 20 this xyz!
	
	0 0 0 pattern-list a 21 this xyz!
	-1 0 0 pattern-list b 21 this xyz!
	-2 0 0 pattern-list c 21 this xyz!
	-2 0 1 pattern-list d 21 this xyz!
	-3 0 1  pattern-list e 21 this xyz!
	
	0 0 0 pattern-list a 22 this xyz!
	-1 0 0 pattern-list b 22 this xyz!
	-2 0 0 pattern-list c 22 this xyz!
	-2 1 0 pattern-list d 22 this xyz!
	-3 1 0 pattern-list e 22 this xyz!
	
	0 0 0 pattern-list a 23 this xyz!
	-1 0 0 pattern-list b 23 this xyz!
	-2 0 0 pattern-list c 23 this xyz!
	-2 0 -1 pattern-list d 23 this xyz!
	-3 0 -1 pattern-list e 23 this xyz!
	\ *************************
	0 piece-orientation !  \ start this piece at orientation 0
	0 piece-location !     \ init to location 0
	this loc>xyz!          \ init a xyz values from location 
    ;m overrides construct
    m: ( piece -- ) \ print piece info
	cr piece-orientation @ . ."  orientation" cr
	piece-location @ . ."  loc" cr
	piece-ax @ . ."  ax " piece-ay @ . ."  ay " piece-az @ . ."  az" cr 
	0 this blockxyz@ rot . swap . . ."  a abs" cr
	1 this blockxyz@ rot . swap . . ."  b abs" cr
	2 this blockxyz@ rot . swap . . ."  c abs" cr
	3 this blockxyz@ rot . swap . . ."  d abs" cr
	4 this blockxyz@ rot . swap . . ."  e abs" cr
	pattern-list a piece-orientation @ this xyz@ rot . swap . . ."  a rel" cr
	pattern-list b piece-orientation @ this xyz@ rot . swap . . ."  b rel" cr
	pattern-list c piece-orientation @ this xyz@ rot . swap . . ."  c rel" cr
	pattern-list d piece-orientation @ this xyz@ rot . swap . . ."  d rel" cr
	pattern-list e piece-orientation @ this xyz@ rot . swap . . ."  e rel" cr ;m overrides print
    m: ( norient nloc piece -- nflag ) \ set the piece orientation and location 
	piece-location !               \ nflag is false if the piece fits in the board boundarys
	piece-orientation !            \ nflag is true if the piece has a block that exceedes the board boundarys
	this loc>xyz!
	this inboard-test ;m method set-piece 
    m: ( piece -- norientation nlocation )
	piece-orientation @
	piece-location @ ;m method get-piece
    m: ( piece-test piece -- nflag ) \ compare piece-test to this piece for any unions
	{ piece-test }               \ nflag is false only if no blocks overlap other blocks in test pieces
	0                            \ nflag is a negative number that indicates total blocks that overlapped if it is not false
	5 0 do
	    5 0 do 
		i this blockxyz@ 
		j piece-test blockxyz@ 
		this testxyz + 
	    loop
	loop ;m method compair-pieces
end-class piece
