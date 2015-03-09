require c:\Users\Philip\Documents\GitHub\cubesolve\piece_object.fs
require string.fs
require objects.fs

25 constant total-pieces
24 constant total-orientations
125 constant total-locations
struct
    cell% field apiece
end-struct pieces%

variable solutionoutput$ s" " solutionoutput$ $!
variable puzzlefile$ s" c:\Users\Philip\Documents\GitHub\cubesolve\puzzlesolution.txt" puzzlefile$ $!
variable fid
variable junk$ s" " junk$ $!
create pieces-array
pieces-array pieces% %size total-pieces * dup allot erase
piece heap-new constant testpiece
variable solution false solution !
variable working-pieces 0 working-pieces ! \ note working-pieces starts at 0 because there are 0 working pieces 
\ now i have an array of cell size variables to hold the piece objects
\ This array will constitute the 25 pieces to solve the puzzle.

: dto$ ( d -- caddr u )  \ convert double signed to a string
    swap over dabs <<# #s rot sign #> #>> ;

: #to$ ( n -- c-addr u1 ) \ convert n to string 
    s>d
    swap over dabs
    <<# #s rot sign #> #>> ;

: piece@ ( nindex -- piece ) \ return piece object address stored in pieces-array 
    pieces-array apiece pieces% %size rot * + @ ;
: piece! ( piece nindex -- ) \ store piece object address in pieces-array
    pieces-array apiece pieces% %size rot * + ! ;
: make-pieces ( -- ) \ instantate total-pieces worth of piece objects and put into pieces-array
    total-pieces 0 do
	piece heap-new i piece!
    loop ;
make-pieces
: place-piece ( norient nloc  -- nflag ) \ test npiece at norient and nloc for fit on board and with other pieces
    testpiece set-piece                  \ return true if successfull false if fit not possible
    true =
    if
	false
    else
	false
	working-pieces @ 0 ?do
	    testpiece i piece@ compair-pieces +
	loop
	false = if true else false then
    then ;

: displayboard ( -- )
    working-pieces @ 0 ?do
	i . ."  *************" 
	i piece@ print 
    loop ;

: solveit { nstart -- }  \ does the main solving iterations. solve_top starts this code
    total-locations total-orientations * nstart ?do
	i total-orientations mod i total-orientations /
	2dup place-piece true =
	if
	    working-pieces @ piece@ set-piece drop
	    working-pieces @ 1 + working-pieces !
	else
	    2drop
	then
	working-pieces @ total-pieces >= if true solution ! leave then 
    loop ;

: solve_top ( nstart -- ) \ solve puzzle from nstart as beginning point 
    begin
	solveit
	solution @ false = key? false = and 
    while
	    working-pieces @ 1 - dup working-pieces !
	    dup piece@ get-piece total-orientations * + 1 +
		dup . ."  sv " swap . ."  wp"cr 
    repeat
    ;
	
: make-solutionoutput$ ( -- ) \ populates the string solutionoutput$ to place in an output file
		working-pieces @ #to$ solutionoutput$ $! s" ;" solutionoutput$ $+!
		total-pieces 0 do
			i piece@ get-piece swap 
			#to$ solutionoutput$ $+!
		    s" ," solutionoutput$ $+!
			#to$ solutionoutput$ $+! s" ;" solutionoutput$ $+!
		loop
		s"  " solutionoutput$ $+!
		;
		
: savepuzzle ( -- )
		puzzlefile$ $@ w/o open-file swap fid !	0 <> 
		if
			puzzlefile$ $@ w/o create-file throw fid !
		then
		make-solutionoutput$ solutionoutput$ $@ fid @ write-file throw
		fid @ flush-file throw
		fid @ close-file throw ;
		
: parsepieces ( -- ) \ take data in solutionoutput$ and place in piece objects
	solutionoutput$ $@  ';' $split junk$ $!
	s>unumber? true <> throw d>s working-pieces !
	total-pieces 0 ?do
		junk$ $@ ',' $split solutionoutput$ $! 
		s>unumber? true <> throw d>s 
		solutionoutput$ $@ ';' $split junk$ $!
		s>unumber? true <> throw d>s 
		i piece@ set-piece
	loop
;  
: loadpuzzle ( -- )
	puzzlefile$ $@ r/o open-file throw fid ! 
	fid @ slurp-fid
	solutionoutput$ $!
	fid @ close-file throw
	;
	
: solvepuzzle ( nstart npiece-start -- ) \ solve puzzle from a point 
\ nstart is 0 to 2999 ( orientation and location together )
\ npiece-start is the working-pieces setting to start with 
\ normaly start the puzzle at 0 0 
	working-pieces !
	solve_top
	key drop 
	." Saving puzzle current solution!" cr
	make-solutionoutput$ savepuzzle
	." Current solution now saved!" cr
	working-pieces @ . ." working-pieces current value!" cr
	;
	
: continuepuzzle ( -- ) \ continue to solve puzzle from file 
	loadpuzzle parsepieces
	0 working-pieces @ solvepuzzle
	;
	
object class
	4 constant displaycellsize
	5 constant topoffset
	7 constant xyz-size
	struct
		cell% field cube#
	end-struct acell%
	create mydisplay
	mydisplay acell% %size xyz-size xyz-size * xyz-size * * dup allot erase
	
	m: ( npiece# nx ny nz display -- ) \ store the piece # at location into a display array for viewing later
		xyz-size xyz-size * * swap \ z calculation offset
		xyz-size * +               \ y calculation offset
		+                          \ x calculation offset
		acell% %size * mydisplay + \ final address calculation
		! ;m method display!
	m: ( nx ny nz display --- npiece# ) \ retreave piece # at location from display array for use
		xyz-size xyz-size * * swap \ z calculation offset
		xyz-size * +               \ y calculation offset
		+                          \ x calculation offset
		acell% %size * mydisplay + \ final address calculation
		@ ;m method display@
	m: ( display -- ) \ populate display array 
		mydisplay acell% %size xyz-size xyz-size * xyz-size * * erase \ clear array
		working-pieces @ 0 ?do
			5 0 ?do
				i j i piece@ blockxyz@ this display!
			loop
		loop ;m method popdisplay
	m: ( display -- ) \ populate and display current solution
		this popdisplay
		page
		xyz-size 0 ?do    		\ x
			xyz-size 0 ?do		\ y
				xyz-size 0 ?do	\ z
					i j k this display@ \ retrieve piece value to display  
					i displaycellsize * \ x 
					xyz-size k * j + topoffset + \ y
					at-xy
					." :" #to$ type 
				loop
			loop
		loop ;m method dodisplay
end-class display

display heap-new constant thedisplay

: seesolution ( -- )
	thedisplay dodisplay ;