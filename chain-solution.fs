require ./chain-ref.fs
require ./newpuzzle.def \ definition of the puzzle to be solved
require ./allpieces.fs  \ to make all the pieces an place them in a chain reference

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list chain-ref heap-new constant chain-ref-array    \ this object takes reference list from above and makes chain ref array of that data
chain-ref-array fast-puzzle-board heap-new constant the-board \ the main board object for viewing the puzzle ... note it needs the reference created above
