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
    begin
      solution-piece-list @ [bind] double-linked-list ll@> rot rot anumberbuffer swap move if
      true true \ leaves loop with true on stack
      else
        anumberbuffer @ uref-piece a-ref-piece-array @ [bind] piece-array fast-intersect?
        if false true else false then \ if an intersection is found leave loop with false on stack otherwise continue loop
      then
    until
  ;m method intersect-test?
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
    a-hapl @ [bind] hole-array-piece-list hole-max-address@ 3 multi-cell-array heap-new board-array !
    double-linked-list heap-new solution-piece-list !
  ;m overrides construct
  m: ( hole-solution -- ) \ destructor
  ;m overrides destruct

  m: ( hole-solution -- ) \ basic terminal view of the board-array reference pieces solution
  ;m method see-solution

  m: ( hole-solution -- ) \ solve puzzle and display partial solutions and steps working on along the way
  ;m method solveit
end-class hole-solution

\ ***************************************************************************************************************************************

ref-piece-array hapl hole-solution heap-new constant testsolution
