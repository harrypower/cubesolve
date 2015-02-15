
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
    m: ( piece -- x y z ) \ piece-location in xyz values and put on stack
	piece-ax @
	piece-ay @
	piece-az @ ;m method loc>xyz@
    m: ( nvalue pattern-list-addr nindex piece -- )
	blc% %size * + ! ;m method pl-index!
    m: ( pattern-list-addr nindex -- nvalue )
	blc% %size * + @ ;m method pl-index@
    m: { nx ny nz npattern-list-addr nindex -- } \ store xyz into pattern-list at index nindex
	nx npattern-list-addr x nindex this pl-index!
	ny npattern-list-addr y nindex this pl-index!
	nz npattern-list-addr z nindex this pl-index! ;m method xyz!
    m: { npattern-list-addr nindex -- x y z } \ retreave xyz from pattern-list at index nindex
	npattern-list-addr x nindex this pl-index@
	npattern-list-addr y nindex this pl-index@
	npattern-list-addr z nindex this pl-index@ ;m method xyz@
  public
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
	pattern-list a piece-orientation @ this xyz@ rot . space swap . space . ."  a" cr
	pattern-list b piece-orientation @ this xyz@ rot . space swap . space . ."  b" cr
	pattern-list c piece-orientation @ this xyz@ rot . space swap . space . ."  c" cr
	pattern-list d piece-orientation @ this xyz@ rot . space swap . space . ."  d" cr
	pattern-list e piece-orientation @ this xyz@ rot . space swap . space . ."  e" cr ;m overrides print
    m: ( norient nloc piece -- ) \ set the piece orientation and location 
	piece-location !
	piece-orientation !
	this loc>xyz! ;m method set-piece
    m: ( piece-test piece -- ) \ compare piece-test to this piece for any unions
	0 0 0 0 0 0 
	{ piece-test test-x test-y test-z this-x this-y this-z }
	pattern-list b piece-orientation @ this xyz@
	piece-az @ + to this-z
	piece-ay @ + to this-y
	piece-ax @ + to this-x
	pattern-list b piece-orientation @ piece-test xyz@
	to test-z to test-y to test-x
	piece-test loc>xyz@
	test-z + to test-z
	test-y + to test-y
	test-x + to test-x
	this-x . space this-y . space this-z . cr
	test-x . space test-y . space test-y . cr
    ;m method compair-pieces

end-class piece
