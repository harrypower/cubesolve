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

object class


end-class hole-solution
