require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs
require ./ref-puzzle-pieces.fs

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
  selector solution-size@     \ ( hole-solution -- usize )
  protected
  cell% inst-var board-array        \ multi-cell-array object containing current puzzle board in this hole-solution
  cell% inst-var a-ref-piece-array  \ piece-array object containing the reference piece array passed to this hole-solution
  cell% inst-var a-hapl             \ hole-array-piece-list object containing the reference hapl passed to this hole-solution
  cell% inst-var solution-piece-list  \ double-linked-list object containing reference pieces of current puzzle solution
  inst-value x-display-size         \ contains constants that are used to display the board in a terminal
  inst-value y-display-size
  inst-value z-display-size
  cell% inst-var anumberbuffer      \ used to store and retrieve a number

  m: ( uref-piece hole-solution -- nflag ) \ test if uref-piece can be placed into current board
  \ this test will look through the current pieces in solution-piece-list using a-ref-piece-array object fast-intersect? test!
  \ nflag is true if uref-piece can be placed into current board and false if uref-piece can not be placed into current board
    { uref-piece }
    solution-piece-list @ [bind] double-linked-list ll-set-start
    solution-piece-list @ [bind] double-linked-list ll-size@
    case
      0 of true endof \ list empty so can be added to
      1 of  \ test if first item in list intersects or not
        solution-piece-list @ [bind] double-linked-list ll@> drop anumberbuffer swap move
        anumberbuffer @ uref-piece a-ref-piece-array @ [bind] piece-array fast-intersect?
        if false else true then \ if an intersection is found place false on stack if no intersection place true on stack
      endof
      begin
        solution-piece-list @ [bind] double-linked-list ll@> rot rot anumberbuffer swap move true =
        if true true \ at end of list and no intersections found so return true
        else
          anumberbuffer @ uref-piece a-ref-piece-array @ [bind] piece-array fast-intersect?
          if false true else false then \ if an intersection is found leave loop with false on stack otherwise continue loop
        then
      until
    endcase
  ;m method intersect-test?

  m: ( unumber ux uy uz hole-solution -- ) \ place unumber in board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array! ;m method board-array!
  m: ( ux uy uz hole-solution -- unumber ) \ retrieve unumber from board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array@ ;m method board-array@
  m: ( uref-piece hole-solution -- ) \ store uref-piece in board-array as defined by the voxels in the piece that uref-piece defines

  ;m method add-board-piece
  m: ( uref-piece hole-solution -- ) \ remove uref-piece from board-array as defined by the voxels in the piece that uref-piece defines

  ;m method del-board-piece

  m: ( uref-piece hole-solution -- ) \ add uref-piece to solution-piece-list at last in list possition
  \ also add uref-piece to board-array multi-cell-array object to allow fast display
    anumberbuffer !
    anumberbuffer cell solution-piece-list @ [bind] double-linked-list ll!
  ;m method add-solution-piece
  m: ( hole-solution -- ) \ delete the last reference added to solution-piece-list
  \ remove the last reference added from the board-array to ensure it is updated
    solution-piece-list @ [bind] double-linked-list delete-last
  ;m method del-solution-piece
  m: ( hole-solution -- usize ) \ return the current size of solution-piece-list
    solution-piece-list @ [bind] double-linked-list ll-size@
  ;m overrides solution-size@

  public
  m: ( uref-piece-array uhapl hole-solution -- ) \ constructor
    6 [to-inst] x-display-size
    1 [to-inst] y-display-size
    x-display-size y-display-size * [to-inst] z-display-size
    a-hapl !
    a-ref-piece-array !
    a-hapl @ [bind] hole-array-piece-list hole-max-address@ { ux uy uz }
    ux uy uz 3 multi-cell-array heap-new board-array !
    uz 0 ?do
      uy 0 ?do
        ux 0 ?do
          true i j k this board-array!
        loop
      loop
    loop
    double-linked-list heap-new solution-piece-list !
  ;m overrides construct
  m: ( hole-solution -- ) \ destructor
  ;m overrides destruct

  m: ( hole-solution -- ) \ basic terminal view of the board-array reference pieces solution
  ;m method see-solution

  m: ( hole-solution -- ) \ solve puzzle and display partial solutions and steps working on along the way
  \ is solution-piece-list full for this puzzle for a solution ... if so done if not continue below
  \ next hole address ...
  \ get reference from a-hapl for above given hole address
  \ test above given reference for intersection
  \ if no intersection add reference to solution-piece-list and board-array for display purposes
  \   go to top to test if puzzle solved and continue
  \ if interesection then get next reference from a-hapl for given hole address above
  \   if at end of reference list for a given hole address then step back one hole address and continue above

  ;m method solveit

  m: ( uref-piece hole-solution -- nflag ) \ test intersect-test? method
    dup this intersect-test?
    true = if this add-solution-piece true else drop false then
  ;m method test-intersect

  m: ( hole-solution -- )  \ test del-solution-piece
    this del-solution-piece ;m method removeit

  m: ( hole-solution -- usize ) \ return current size of solution
    this solution-size@ ;m method  currentsize@
end-class hole-solution

\ ***************************************************************************************************************************************

ref-piece-array hapl hole-solution heap-new constant testsolution
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
