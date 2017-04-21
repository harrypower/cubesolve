require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./piece-array.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above
                                                              \ and makes a reference array of list for indexing faster

\ the follwoing class needs to be rebuilt to use ref-piece-array above and that means index values rather then piece object references
board class
  m: ( upiece board-fast -- nflag ) \ test if upiece could be placed on the current populated board
    \ nflag is true if upiece can be placed on the current board
    \ nflag is false if upiece could not be placed on current board due to a piece intersection or a board boundry issue.
  ;m overrides piece-on-this-board?

end-class board-fast
