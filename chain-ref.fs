require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./piece-array.fs
require ./serialize-obj.fs
require ./fast-puzzle-board.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -chain-ref
piece-array class
  destruction implementation
  protected
\  inst-value value-name

  m: ( nquantity chain-ref -- ) \ used to restore serialized instance values
  ;m method ser-chain-inst-values!

  m: ( nquantity chain-ref -- ) \ used to restore serialized double-linked-list
  ;m method ser-chain-dll!

  public
  m: ( upieces chain-ref -- ) \ constructor
    this [parent] construct
  ;m overrides construct

  m: ( chain-ref -- ) \ destructor
    this [parent] destruct
  ;m overrides destruct

  m: ( chain-ref -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] serialize-data@
\ these commented out lines are examples of serializing instance values and a double-linked-list object
\    ['] ser-chain-inst-values! this do-save-name
\    1 this do-save-nnumber  \ there is 1 inst-var saved here to serialize and retrieve later
\    ['] value-name this do-save-inst-value

\    ['] ser-chain-dll! this do-save-name
\    dll-value [bind] double-linked-list ll-size@ dup this do-save-nnumber
\    dll-value [bind] double-linked-list ll-set-start
\    0 ?do
\      dll-value [bind] double-linked-list ll-cell@ this do-save-nnumber
\    loop
    save$
  ;m overrides serialize-data@

  m: ( nstrings chain-ref -- ) \ nstrings contains serialized data to restore this object
    \ this [current] destruct
    \ this [parent] construct
    this [parent] serialize-data!
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    \ this [current] do-retrieve-data true = if d>s rot rot -chain-ref rot rot this [current] $->method else 2drop 2drop true abort" chain-ref inst-value data incorrect!" then
    \ this [current] do-retrieve-data true = if d>s rot rot -chain-ref rot rot this [current] $->method else 2drop 2drop true abort" chain-ref double-linked-list data incorrect!" then
  ;m overrides serialize-data!

end-class chain-ref
' chain-ref is -chain-ref

\ ********************************************************************************************************************************
\ \\\
require ./newpuzzle.def \ this is the definition of the puzzle to be solved

require ./allpieces.fs

0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces it is not used directly but produces ref.
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list chain-ref heap-new constant chain-ref-array    \ this object takes reference list from above and makes chain ref array of that data

chain-ref-array bind chain-ref quantity@ . ." < should be 480" cr
