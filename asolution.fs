require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs
require ./ref-puzzle-pieces.fs

: pause-for-key ( -- ) \ halt until a key is pressed then continue
  begin key? until
  key drop 10 ms ;

: key-test-wait ( -- ) \ if keyboard is pressed pause until it is pressed again
  key?
  if
    key drop 10 ms
    pause-for-key
  then ;

\ puzzle-board - a board object for the basic puzzle
\ this board object can be used with puzzle-board to hold pieces
\ ref-piece-array - a make-all-pieces object containing all the reference pieces for basic puzzle
\ this reference array can be used to test if pieces intersect in a fast way!
\ hapl - a hole-array-piece-list object organized as x y z addressed holes with lists of reference pieces that are in that hole

ref-piece-array puzzle-board hole-array-piece-list heap-new constant hapl

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation  \ ( hole-solution -- )
  protected
  selector next-hole          \ ( hole-solution -- )
  selector solution-size@     \ ( hole-solution -- usize )
  cell% inst-var board-array        \ multi-cell-array object containing current puzzle board in this hole-solution
  cell% inst-var a-ref-piece-array  \ piece-array object containing the reference piece array passed to this hole-solution
  cell% inst-var a-hapl             \ hole-array-piece-list object containing the reference hapl passed to this hole-solution
  cell% inst-var solution-piece-list  \ double-linked-list object containing reference pieces of current puzzle solution
  inst-value x-display-size         \ contains constants that are used to display the board in a terminal
  inst-value y-display-size
  inst-value z-display-size
  inst-value x-max
  inst-value y-max
  inst-value z-max
  inst-value x-now
  inst-value y-now
  inst-value z-now
  cell% inst-var anumberbuffer      \ used to store and retrieve a number
  inst-value final-solution         \ holds the number of pieces needed that constitutes a solution
  inst-value solveloops             \ for display purposes the amount of loops between showing current solution
  inst-value solvelow
  inst-value solvelow-flag
  inst-value solvehigh

  m: ( hole-solution -- nflag ) \ test if puzzle can be solved and if so place piece count needed for soulution in final-solution
  \ nflag is true if puzzle can be solved and false if the pieces do not add up to a solution
    x-max y-max * z-max *
    0 a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@
    2dup / [to-inst] final-solution
    mod 0= if true else false then
  ;m method test-solvable?
  m: ( uref-piece hole-solution -- nflag ) \ test if uref-piece can be placed into current board
  \ this test will look through the current pieces in solution-piece-list using a-ref-piece-array object fast-intersect? test!
  \ nflag is true if uref-piece can be placed into current board and false if uref-piece can not be placed into current board
    { uref-piece }
    solution-piece-list @ [bind] double-linked-list ll-set-start
    solution-piece-list @ [bind] double-linked-list ll-size@
    \ .s ." size of list in test" cr
    0 =
    if
      true \ list empty so no intersection
    else
      begin
        solution-piece-list @ [bind] double-linked-list ll@> rot rot anumberbuffer swap move true =
        if \ at end of list
          anumberbuffer @ uref-piece \ .s ." data into fast test at end" cr
          a-ref-piece-array @ [bind] piece-array fast-intersect?
          if false true else true true then \ if intersection found leave loop with false on stack otherwise leave loop with true on stack
        else \ in middle of list
          anumberbuffer @ uref-piece \ .s ." data into fast test in middle" cr
          a-ref-piece-array @ [bind] piece-array fast-intersect?
          if false true else false then \ if an intersection is found leave loop with false on stack otherwise continue loop
        then
      until
    then
  ;m method intersect-test?

  m: ( unumber ux uy uz hole-solution -- ) \ place unumber in board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array! ;m method board-array!
  m: ( ux uy uz hole-solution -- unumber ) \ retrieve unumber from board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array@ ;m method board-array@
  m: ( uvoxel uref-piece hole-solution -- ux uy uz ) \ get voxel address from uref-pieces
    a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel ;m method voxel-address@
  m: ( uref-piece hole-solution -- ) \ store uref-piece in board-array as defined by the voxels in the piece that uref-piece defines
    { uref-piece } uref-piece a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@ 0 ?do
      uref-piece \ store the reference piece in board-array
      i uref-piece this voxel-address@
      this board-array!
    loop ;m method add-board-piece
  m: ( uref-piece hole-solution -- ) \ remove uref-piece from board-array as defined by the voxels in the piece that uref-piece defines
    { uref-piece } uref-piece a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@ 0 ?do
      true \ true is the place holder for no piece in board-array
      i uref-piece this voxel-address@
      this board-array!
    loop ;m method del-board-piece

  m: ( uref-piece hole-solution -- ) \ add uref-piece to solution-piece-list at last in list possition
  \ also add uref-piece to board-array multi-cell-array object to allow fast display
    dup
    anumberbuffer !
    anumberbuffer cell solution-piece-list @ [bind] double-linked-list ll!
    this add-board-piece ;m method add-solution-piece
  m: ( hole-solution -- ) \ delete the last reference added to solution-piece-list
  \ remove the last reference added from the board-array to ensure it is updated
    solution-piece-list @ [bind] double-linked-list ll-set-end
    solution-piece-list @ [bind] double-linked-list ll@ anumberbuffer swap move
    anumberbuffer @ this del-board-piece
    solution-piece-list @ [bind] double-linked-list delete-last ;m method del-solution-piece
  m: ( hole-solution -- usize ) \ return the current size of solution-piece-list
    solution-piece-list @ [bind] double-linked-list ll-size@ ;m overrides solution-size@

  m: ( uref-piece hole-solution -- nflag ) \ test intersecting for uref-piece and place piece in solution-piece-list
  \ nflag is true if uref-piece can be placed in solution-piece-list and is placed in list and board array
  \ nflag is false if uref-piece can not be placed in solution-piece-list and is not placed in list and board array
  \ note uref-piece is placed into solution-piece-list and put on the board-array for display purposes if it can be placed at all
    dup this intersect-test? if this add-solution-piece true else drop false then ;m method place-piece?
  m: ( ux uy hole-solution -- ) \ display current solution reference numbers
    { ux uy }
    solution-piece-list @ [bind] double-linked-list ll-set-start
    begin
      ux uy 1 + dup to uy at-xy
      solution-piece-list @ [bind] double-linked-list ll@>
      rot rot anumberbuffer swap move anumberbuffer @ .
      ux 5 + uy at-xy
      0 anumberbuffer @ a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel
      rot . swap . .
      0 anumberbuffer @ a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel
      ux 15 + uy at-xy
      a-hapl @ [bind] hole-array-piece-list hole-list-quantity@ .
    until ;m method display-current-solution-list

  m: ( hole-solution -- ux uy uz ) \ return current hole address to fill with out changing it
    x-now y-now z-now ;m method current-hole
  m: ( hole-solution -- ) \ increment current hole address
    x-now 1 + dup [to-inst] x-now
    x-max = if
      0 [to-inst] x-now
      y-now 1 + dup [to-inst] y-now
      y-max = if
        0 [to-inst] y-now
        z-now 1 + dup [to-inst] z-now
        z-max = if
          0 [to-inst] z-now
        then
      then
    then ;m method hole+
\  m: ( hole-solution -- ) \ decrement current hole address
\    x-now 1 - dup [to-inst] x-now
\    0< if x-max 1 - [to-inst] x-now
\      y-now 1 - dup [to-inst] y-now
\      0< if y-max 1 - [to-inst] y-now
\        z-now 1 - dup [to-inst] z-now
\        0< if z-max 1 - [to-inst] z-now
\        then
\      then
\    then ;m method hole-
  m: ( hole-solution -- ) \ increment to next hole address to fill
    begin
      this current-hole \ 40 40 at-xy .s ."    "
      this board-array@ \ 40 41 at-xy .s ."    "
      \ pause-for-key
      true = if true else this hole+ false then
    until
  ;m overrides next-hole
  public
  m: ( uref-piece-array uhapl hole-solution -- ) \ constructor
    6 [to-inst] x-display-size
    1 [to-inst] y-display-size
    x-display-size y-display-size * [to-inst] z-display-size
    a-hapl !
    a-ref-piece-array !
    a-hapl @ [bind] hole-array-piece-list hole-max-address@ [to-inst] z-max [to-inst] y-max [to-inst] x-max
    x-max y-max z-max 3 multi-cell-array heap-new board-array !
    z-max 0 ?do
      y-max 0 ?do
        y-max 0 ?do
          true i j k this board-array!
        loop
      loop
    loop
    double-linked-list heap-new solution-piece-list !
    0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
    0 [to-inst] solveloops
    this test-solvable? drop
    final-solution [to-inst] solvelow
    false [to-inst] solvelow-flag
    0 [to-inst] solvehigh
  ;m overrides construct
  m: ( hole-solution -- ) \ destructor
  ;m overrides destruct

  m: ( hole-solution -- ) \ basic terminal view of the board-array reference pieces solution
  \  page
    z-max 0 ?do
      y-max 0 ?do
        x-max 0 ?do
          i x-display-size * j y-display-size * k z-display-size * + at-xy
          i j k this board-array@ dup true = if drop ." *****" else 5 u.r then
        loop
      loop
    loop
  ;m method see-solution

  m: ( hole-solution -- ) \ solve puzzle and display partial solutions and steps working on along the way
    this test-solvable? if
      page
      this current-hole
      a-hapl @ [bind] hole-array-piece-list next-ref-piece-in-hole@ drop
      this place-piece? drop
      this see-solution
      begin
        this next-hole
        begin
          this current-hole
          a-hapl @ [bind] hole-array-piece-list next-ref-piece-in-hole@
          if \ at end of hole references
            this place-piece?
            if \ next hole because hole was filled with last one .. now exit this begin until
              true
            else \ last hole becasue hole was not filled with last one  .. now exit  this begin until
              \ solution-piece-list @ [bind] double-linked-list ll-set-end
              \ solution-piece-list @ [bind] double-linked-list ll@ anumberbuffer swap move
              \ 0 anumberbuffer @ a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel
\              [to-inst] z-now [to-inst] y-now [to-inst] x-now
\              this current-hole 40 35 at-xy rot . swap . .
              this del-solution-piece
              0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
              true
            then
          else \ not at end of hole references
            this place-piece?
            if \ next hole because hole was filled with last one .. now exit this begin until
              true
            else \ next reference at same hole becasue hole was not filled ... do not exit this begin until
              false
            then
          then
        until
        this solution-size@ solvehigh > if this solution-size@ [to-inst] solvehigh then
        solveloops 3000 > if true [to-inst] solvelow-flag then
        this solution-size@ solvelow < solvelow-flag and if this solution-size@ [to-inst] solvelow then
        solveloops 3000 > if
          0 [to-inst] solveloops this see-solution
          40 0 at-xy this solution-size@ . ." solution-size"
          40 1 at-xy solvehigh . ." highest"
          40 2 at-xy solvelow . ." lowest"
          this current-hole 40 3 at-xy rot . swap . . ." current-hole"
          40 4 this display-current-solution-list
        else
          solveloops 1 + [to-inst] solveloops
        then
\        0 [to-inst] solveloops this see-solution
\        40 0 at-xy this solution-size@ . ." solution-size"
\        40 1 at-xy solvehigh . ." highest"
\        40 2 at-xy solvelow . ." lowest"
\        this current-hole 40 3 at-xy rot . swap . . ." current-hole"
\        40 4 this display-current-solution-list
\        pause-for-key
        key-test-wait
        this solution-size@ final-solution =
      until
      page this see-solution
      40 0 at-xy ." solution found!"
    else
      ." The puzzle board can not hold the puzzle pieces in an even quantity!  Puzzle is not solvable!" cr
    then
  ;m method solveit

  m: ( uref-piece hole-solution -- nflag ) \ test intersect-test? method
    this place-piece? ;m method test-intersect

  m: ( hole-solution -- )  \ test del-solution-piece
    this del-solution-piece ;m method removeit

  m: ( hole-solution -- usize ) \ return current size of solution
    this solution-size@ ;m method  currentsize@

  m: ( hole-solution -- ux uy uz ) \ test next hole solution
    this next-hole
    this current-hole ;m method nexthole@
end-class hole-solution

\ ***************************************************************************************************************************************

\ 0 0 0 hapl next-ref-piece-in-hole@ .s ." < 0 0 0 " cr
\ 1 0 0 hapl next-ref-piece-in-hole@ .s ." < 1 0 0 " cr
\ \\\
ref-piece-array hapl hole-solution heap-new constant testsolution
\ 0 testsolution test-intersect . ." answer for ref 0" cr
\ 376 testsolution test-intersect . ." answer for ref 376" cr
\ 60 testsolution test-intersect . ." answer for ref 60" cr
\ \\\
page
testsolution solveit

\\\
testsolution currentsize@ ." should be 1" cr
testsolution see-solution

\\\
cr 0 testsolution test-intersect . cr
testsolution currentsize@ . cr
0 59 ref-piece-array fast-intersect? . cr
testsolution removeit
0 testsolution test-intersect . cr
testsolution currentsize@ . cr
5 testsolution test-intersect . cr
testsolution currentsize@ . cr
59 testsolution test-intersect . cr
testsolution currentsize@ . cr

testsolution see-solution
testsolution nexthole@ . . .
