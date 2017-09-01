require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./Gforth-Objects/mdca-obj.fs
require ./piece-array.fs
require ./serialize-obj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -fast-puzzle-board
save-instance-data class
  destruction implementation
\  selector index>xyz  ( uindex hole-array-piece-list -- uholex uholey uholez ) \ return the xyz hole address for the given uindex value
  protected
  inst-value board-array      \ double-linked-list of current board locations index numbers inside them
  inst-value ref-piece-array

  public
  m: ( uref-piece-array upuzzle-board fast-puzzle-board -- ) \ constructor
    \ uref-piece-array is a piece-array object that contains all the pieces this puzzle board can place on it. The array is copied into this object not stored
    \ upuzzle-board is a board object that simply has the size of the puzzle to be worked on.  The size is taken and upzzle-board not stored.

  ;m overrides construct
  m: ( fast-puzzle-board -- ) \ destructor
  ;m overrides destruct
  m: ( fast-puzzle-board -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
    \ put code her to stringafy the data
    save$
  ;m overrides serialize-data@

  m: ( nstrings fast-puzzle-board -- ) \ nstrings contains serialized data to restore this object
    this destruct
    this [parent] construct
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this $->method else 2drop 2drop true abort" FPB inst-value data incorrect!" then
    this create-hole-array
    this do-retrieve-data true = if d>s rot rot -fast-puzzle-board rot rot this $->method else 2drop 2drop true abort" FPB indexed reference data incorrect!" then
  ;m overrides serialize-data!
end-class fast-puzzle-board
' fast-puzzle-board is -fast-puzzle-board

\ **********************************************************************************************************************************************************************
