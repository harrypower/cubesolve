require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

\ mmm must rethink this class through... i may be doing to many things here that should not be grouped!!
object class \ like board object but uses index from reference array to populate a board
  destruction implementation
  protected
  inst-value x-max              \ values containing the x y and z max board dimensions
  inst-value y-max
  inst-value z-max
  inst-value x-display-size
  inst-value y-display-size
  inst-value z-display-size
  inst-value index-quantity     \ current size of the indexs on this board
  cell% inst-var board-ref      \ contains address to array-object that contains the board references as reference indexes
  cell% inst-var board-index    \ contains address to array-object that contains the board indexs
  public
  m: ( uref-piece-array board-fast -- ) \ constructor
  \ note the board dimensions are retrieved from puzzle-board object of type board
  ;m overrides construct

  m: ( board-fast -- ) \ destructor
  ;m overrides destruct

  m: ( board-fast --  ux-max uy-max uz-max )
  ;m method board-dims@

  m: ( board-fast -- uindex-quantity )
  ;m method board-index-quantity@

  m: ( uref board-fast -- nflag ) \ place uref on board
  \ nflag is true if uref was added
  \ nflag is false if uref was not added due to a piece intersection issue
  ;m method add-ref-to-board

  m: ( board-fast -- )
  ;m method remove-last-ref-from-board

  m: ( uref board-fast -- nflag ) \ test if uref could be placed on the current populated board
    \ nflag is true if uref can be placed on the current board
    \ nflag is false if uref could not be placed on current board due to a piece intersection
  ;m method ref-on-this-board?

  m: ( board-fast -- ) \ display current board as sequential pieces placed and index
  ;m method see-board-as-indexs

  m: ( board-fast -- ) \ display current board as reference indexs of pieces
  ;m method see-board-as-ref
end-class board-fast
