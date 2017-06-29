require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/stringobj.fs
require ./newpieces.fs
require ./puzzleboard.fs

board heap-new constant puzzle-board
\ puzzle-board - a board object for the basic puzzle
\ this board object can be used with puzzle-board to hold pieces
require ./newpuzzle.def \ this is the definition of the puzzle to be solved

require ./allpieces.fs
require ./piece-array.fs
require ./ref-puzzle-pieces.fs
require ./save-file-obj.fs
require ./serialize-obj.fs
require ./holesolution.fs


0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

ref-piece-array puzzle-board hole-array-piece-list heap-new constant hapl
\ hapl - a hole-array-piece-list object organized as x y z addressed holes with lists of reference pieces that are in that hole

ref-piece-array hapl hole-solution heap-new constant asolution

page
asolution start-solving
