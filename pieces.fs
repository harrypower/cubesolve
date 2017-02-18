require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

object class
  destruction implementation
  selector clear-board
  protected
  inst-value x-max
  inst-value y-max
  inst-value z-max
  cell% inst-var board-a
\  cell% inst-var board-b
  cell% inst-var board-bytes
  cell% inst-var board-bits
  public
  m: ( voxel-board-mapping -- ) \ destructor
    board-a @ free throw
\    board-b @ free throw
  ;m overrides destruct
  m: ( uxmax uymax uzmax voxel-board-mapping -- )
    [to-inst] z-max
    [to-inst] y-max
    [to-inst] x-max
    x-max y-max z-max
    * * board-bits !
    board-bits @ 8 / aligned dup
    8 * board-bits @ > if drop board-bits @ 8 / 1+ aligned then
    board-bytes !
    board-bytes @ allocate throw board-a !
\    board-bytes @ allocate throw board-b !
    this clear-board
  ;m method set-board-dims
  m: ( voxel-board-mapping -- uxmax uymax uxmax ) \ return the board dimentions
    x-max y-max z-max ;m method get-board-dims
  m: ( ux uy uz voxel-board-mapping -- )
    z-max y-max * *
    swap x-max * + + dup 8 / { ubit ubyte }
    board-a @ ubyte + c@
    1 ubit ubyte 8 * - lshift or
    board-a @ ubyte + c!
  ;m method set-board-voxel
  m: ( uboard upieces voxel-board-mapping -- ) \ will extract all the voxels from the binary map in uboard and put them in upieces object
  \ note upieces object will be cleared and added to as if one big pieces was defined
    { uboard upieces }
  ;m method get-voxels-from-board
  m: ( voxel-board-mapping -- uboard-> ubytes )
    board-a @ board-bytes @
  ;m method get-board
  m: ( voxel-board-mapping -- )
    board-a @ board-bytes @ erase
\    board-b @ board-bytes @ erase
  ;m overrides clear-board
  m: ( uboard uboard1 voxel-board-mapping -- )
    { uboard uboard1 -- nflag } \ uboard and uboard1 are addresses of boards with pieces to be tested for overlapping pieces
  \ board sizes tested are equal to board-bytes @ amount
  \ nflag is false if there are no overlaps
  \ nflag is true for an overlap of any piece
    0 board-bytes @ 0 ?do uboard i + c@ uboard1 i + c@ and or loop 0 = if false else true then
  ;m method pieces-overlaping?
  m: ( uboard uboard1 voxel-board-mapping -- )
    { uboard uboard1 -- nflag } \ uboard and uboard1 are addresses of boards containing pieces to compair for exact copies
  \ board sizes tested are equal to board-bytes @ amount
  \ nflag is false if no exact copy of pieces are found in boards
  \ nflag is true if an exact copy of pieces are found in boards
    true board-bytes @ 0 ?do uboard i + c@ uboard1 i + c@ <> if drop false leave then loop
  ;m method pieces-identical?
end-class voxel-board-mapping

create puzzle-board-dimensions voxel-board-mapping dict-new drop

object class
  destruction implementation
  protected
  struct
    char% field voxel-x
    char% field voxel-y
    char% field voxel-z
  end-struct temp-voxel%
  cell% inst-var voxels
  cell% inst-var voxels-size
  cell% inst-var piece-list
  cell% inst-var voxel-list
  public
  m: ( pieces -- ) \ constructor Note memory allocated here so call destruct before calling this construct or memory leaks will happen
    double-linked-list heap-new piece-list !
    double-linked-list heap-new voxel-list !
    temp-voxel% %size dup voxels-size ! allocate throw voxels !
    voxels @ voxels-size @ erase
  ;m overrides construct
  m: ( pieces -- ) \ destructor
    piece-list @ 0 <> voxel-list @ 0 <> and if
      piece-list @ ll-set-start
      piece-list @ ll-size@ 0 ?do
        piece-list @ ll@ drop @ destruct
        piece-list @ ll@> 2drop @ free throw
      loop
      piece-list @ destruct
      piece-list @ free throw
      voxel-list @ destruct
      voxel-list @ free throw
      0 piece-list !
      0 voxel-list !
      voxels @ free throw
    then ;m overrides destruct
  m: ( pieces -- ) \ add voxel list to the current piece in the piece-list
    voxel-list cell
    piece-list @ ll!
    double-linked-list heap-new voxel-list !
  ;m method define
  m: ( pieces -- uquantity ) \ return quantity of pieces
    piece-list @ ll-size@ ;m method piece-quantity
  m: ( ux uy uz pieces -- ) \ add voxel to voxel-list
    voxels @ voxel-z c!
    voxels @ voxel-y c!
    voxels @ voxel-x c!
    voxels @ voxels-size @ voxel-list @ ll!
  ;m method add-voxel
  m: ( uindex pieces -- ux uy uz ) \ retrieve voxel from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll@ drop
    dup >r voxel-x c@
    r@ voxel-y c@
    r> voxel-z c@ ;m method getvoxel
  m: ( unindex pieces -- ) \ move voxel list pointer to next voxel from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll> if piece-list @ ll@ drop @ ll-set-start then ;m method nextvoxel
  m: ( uindex pieces -- usize ) \ return voxel quantity from piece uindex
    piece-list @ ll-set-start
    0 ?do piece-list @ ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ ll@ drop @ ll-size@ ;m method voxel-quantity
end-class pieces

create puzzle-pieces pieces dict-new drop

include ./puzzle.def

voxel-board-mapping class
\ this class has methods to take puzzle-pieces and create all the translated pieces in a link list of board images
\ the data for this object is collected from puzzle-pieces and puzzle-board-dimensions objects
  destruction implementation
  protected
  inst-value pieces->             \ a pointer to the current defined pieces to puzzle
  inst-value working-pieces->     \ a pointer to pieces object for working on translation pieces
  inst-value translated-pieces->  \ a pointer to a double-linked-list object containing translated pieces in a board binary format
  m: ( uboard-> -- nflag ) \ look for uboard in translated-pieces list if not found nflag is false if found nflag is true
    translated-pieces-> ll-set-start
    false translated-pieces-> ll-size@ 0 ?do drop
      dup translated-pieces-> ll@> 2drop this pieces-identical? if true leave else false then
    loop swap drop
  ;m method in-piece-list?
  m: ( uindex upieces translate-pieces -- ) \ make piece board image and add it to translated-pieces list if it is not there already
    { uindex upieces }
    this clear-board
    uindex upieces voxel-quantity 0 ?do
      uindex upieces getvoxel
      uindex upieces nextvoxel
      this set-board-voxel
    loop
    this get-board
    over this in-piece-list? false = if translated-pieces-> ll! else 2drop then
  ;m method make-add-piece
  m: ( translate-pieces -- ) \ add input pieces to translated-pieces list
\    pieces-> piece-quantity 0 ?do
\      i pieces-> voxel-quantity 0 ?do
\        j pieces-> getvoxel working-pieces-> add-voxel
\        j pieces-> nextvoxel
\      loop
\      working-pieces-> define
\    loop
    pieces-> piece-quantity 0 ?do
      i pieces-> this make-add-piece
    loop
  ;m method add-start-pieces
  public
  m: ( translate-pieces -- )
    pieces heap-new [to-inst] working-pieces->
    puzzle-pieces [to-inst] pieces->
    double-linked-list heap-new [to-inst] translated-pieces->
    puzzle-board-dimensions get-board-dims this set-board-dims
    this add-start-pieces
  ;m overrides construct
  m: ( translate-pieces -- ) \ destructor
    this destruct
    translated-pieces-> destruct
    working-pieces-> destruct
  ;m overrides destruct
  m: ( -- ) \ print
    cr this [parent] print cr
    this get-board-dims rot . swap . . ."  x y z dimensions of this puzzle board!" cr
    working-pieces-> piece-quantity . ." pieces in working list!" cr
    working-pieces-> piece-quantity 0 ?do
      i . ."  piece" cr
      i working-pieces-> voxel-quantity 0 ?do
        j working-pieces-> getvoxel rot . space swap . space . ." voxel!" cr
        j working-pieces-> nextvoxel
      loop
    loop
    translated-pieces-> ll-size@ dup . ."  pieces in translated-pieces!" cr
    translated-pieces-> ll-set-start
    translated-pieces-> ll-size@ 0 ?do
      i . ."  piece" cr
      translated-pieces-> ll@> drop dump
    loop
  ;m overrides print
end-class translate-pieces

create translated-pieces translate-pieces dict-new drop
translated-pieces print cr

\\\
loop total pieces
loop total voxels for current piece
 piece store voxels if this pieces voxel combination is not currently in linked list
 loop all rotations for current piece
  loop all translations for current piece
    piece translations rotations store voxels if this pieces translation rotations voxels combination are not currently in linked list
  endloop all translations for current pieces
 endloop all rotations for current piece
endloop total voxels for current piece
endloop total pieces
