include pieces.fs

\ *************
\ these structures and words are an attempt to understand total rotational combinations
\ *************
x-count y-count * z-count * rotations * constant total-possibilitys
variable total-solutions 0 total-solutions !

loc%
    cell% field rot#
end-struct solution-list%

create thesolutions
thesolutions solution-list% %size total-possibilitys * dup allot erase


: solutions! ( nx ny nz nrot nindex -- ) \ store rotational possible placement in solution-list%
    solution-list% %size * thesolutions + dup
    rot# rot swap ! dup
    z rot swap ! dup
    y rot swap !
    x ! ;

: solutions@ ( nindex -- nx ny nz nrot ) \ retreave rotational possible placemet in the solution-list% at nindex location
    \ note the list will retreave empty entry in the list as 0 rotation value
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
    LOOP total-solutions ! ;  \ note only here is the variable total-solutions correctly populated

\ ************
\ These next words are about solving one path of the puzzle to see how it could be done
\ ************
