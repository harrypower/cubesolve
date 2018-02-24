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
  inst-value chain-array  \ mdca containing double-linked-list objects per array cell

  m: ( uref-test uref chain-ref -- nflag ) \ for a uref piece find if uref-test piece can chain on either end
  \ nflag returns true if uref-test can chain with uref piece and false if uref-test piece can not chain with uref
  \ note there are 17 X 2 voxel locations to test for a piece to be considered a chain piece.  This method test them all and returns true at first chain find!
  \ The false will be returned if uref-test has no end piece in one of these 17 X 2 voxel locations.
  ;m method chain?

  m: ( uref chain-ref -- ) \ populate the reference chain list for uref piece only in chain-array mdca
  \ note this needs to be done for each reference piece in this piece-array
    drop 
  ;m method generate-chain

  m: ( nquantity chain-ref -- ) \ used to restore serialized instance values
  ;m method ser-chain-inst-values!

  m: ( nquantity chain-ref -- ) \ used to restore serialized double-linked-list
  ;m method ser-chain-dll!
  public
  m: ( upieces chain-ref -- ) \ constructor
    this [parent] construct
    this [current] quantity@ 1 multi-cell-array heap-new [to-inst] chain-array
    this [current] quantity@ 0 ?do
      double-linked-list heap-new i chain-array [bind] multi-cell-array cell-array!
      i this [current] generate-chain
    loop
  ;m overrides construct

  m: ( chain-ref -- ) \ destructor
    this [current] quantity@ 0 ?do
      i chain-array [bind] multi-cell-array cell-array@ dup [bind] double-linked-list destruct free throw
    loop
    chain-array [bind] multi-cell-array destruct
    chain-array free throw
    this [parent] destruct
  ;m overrides destruct

  m: ( uref chain-ref -- unext-chain nflag ) \ retrieve next chain reference for given uref piece
  \ unext-chain is a piece that can chain to uref piece
  \ nflag is true when the list of possible chains for uref is at the end and will reset for next retriveal to be at beginning of list
  ;m method next-chain@

  m: ( uref chain-ref -- uchain-quantity ) \ return chain quantity for given uref piece
  ;m method chain-quantity@

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

chain-ref-array bind chain-ref destruct
