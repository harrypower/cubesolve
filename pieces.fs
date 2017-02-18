require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

object class
  destruction implementation
  selector clear-board
  selector pieces-overlaping?
  protected
  inst-value x-max
  inst-value y-max
  inst-value z-max
  cell% inst-var board-a
  cell% inst-var board-bytes
  cell% inst-var board-bits
  public
  m: ( voxel-board-mapping -- ) \ destructor
    board-a @ free throw
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
  m: ( voxel-board-mapping -- uboard-> ubytes )
    board-a @ board-bytes @
  ;m method get-board
  m: ( voxel-board-mapping -- )
    board-a @ board-bytes @ erase
  ;m overrides clear-board
  m: ( uboard uboard1 voxel-board-mapping -- nflag )
    { uboard uboard1 -- nflag } \ uboard and uboard1 are addresses of boards with pieces to be tested for overlapping pieces
  \ board sizes tested are equal to board-bytes @ amount
  \ nflag is false if there are no overlaps
  \ nflag is true for an overlap of any piece
    0 board-bytes @ 0 ?do uboard i + c@ uboard1 i + c@ and or loop 0 = if false else true then
  ;m overrides pieces-overlaping?
  m: ( uboard uboard1 voxel-board-mapping -- nflag )
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
      piece-list @ [bind] double-linked-list ll-set-start
      piece-list @ [bind] double-linked-list ll-size@ 0 ?do
        piece-list @ [bind] double-linked-list ll@ drop @ [bind] double-linked-list destruct
        piece-list @ [bind] double-linked-list ll@> 2drop @ free throw
      loop
      piece-list @ [bind] double-linked-list destruct
      piece-list @ free throw
      voxel-list @ [bind] double-linked-list destruct
      voxel-list @ free throw
      0 piece-list !
      0 voxel-list !
      voxels @ free throw
    then ;m overrides destruct
  m: ( pieces -- ) \ add voxel list to the current piece in the piece-list
    voxel-list cell
    piece-list @ [bind] double-linked-list ll!
    double-linked-list heap-new voxel-list !
  ;m method define
  m: ( pieces -- uquantity ) \ return quantity of pieces
    piece-list @ [bind] double-linked-list ll-size@ ;m method piece-quantity
  m: ( ux uy uz pieces -- ) \ add voxel to voxel-list
    voxels @ voxel-z c!
    voxels @ voxel-y c!
    voxels @ voxel-x c!
    voxels @ voxels-size @ voxel-list @ [bind] double-linked-list ll!
  ;m method add-voxel
  m: ( uindex pieces -- ux uy uz ) \ retrieve voxel from piece uindex
    piece-list @ [bind] double-linked-list ll-set-start
    0 ?do piece-list @ [bind] double-linked-list ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ [bind] double-linked-list ll@ drop @ [bind] double-linked-list ll@ drop
    dup >r voxel-x c@
    r@ voxel-y c@
    r> voxel-z c@ ;m method getvoxel
  m: ( unindex pieces -- ) \ move voxel list pointer to next voxel from piece uindex
    piece-list @ [bind] double-linked-list ll-set-start
    0 ?do piece-list @ [bind] double-linked-list ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ [bind] double-linked-list ll@ drop @ [bind] double-linked-list ll>
    if piece-list @ [bind] double-linked-list ll@ drop @ [bind] double-linked-list ll-set-start then ;m method nextvoxel
  m: ( uindex pieces -- usize ) \ return voxel quantity from piece uindex
    piece-list @ [bind] double-linked-list ll-set-start
    0 ?do piece-list @ [bind] double-linked-list ll> abort" Trying to get a piece that is not there!" loop
    piece-list @ [bind] double-linked-list ll@ drop @ [bind] double-linked-list ll-size@ ;m method voxel-quantity
  m: ( pieces -- ) \ print
    cr this [parent] print
    this piece-quantity . ." pieces in this pieces object!" cr
    this piece-quantity 0 ?do
      i . ."  piece" cr
      i this voxel-quantity 0 ?do
        j this getvoxel rot . space swap . space . ." voxel!" cr
        j this nextvoxel
      loop
    loop
  ;m overrides print
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
    translated-pieces-> [bind] double-linked-list ll-set-start
    false translated-pieces-> [bind] double-linked-list ll-size@ 0 ?do drop
      dup translated-pieces-> [bind] double-linked-list ll@> 2drop this pieces-identical? if true leave else false then
    loop swap drop
  ;m method in-piece-list?
  m: ( uindex upieces translate-pieces -- ) \ make piece board image and add it to translated-pieces list if it is not there already
    { uindex upieces }
    this clear-board
    uindex upieces voxel-quantity 0 ?do
      uindex upieces [bind] pieces getvoxel
      uindex upieces [bind] pieces nextvoxel
      this set-board-voxel
    loop
    this get-board
    over this in-piece-list? false = if translated-pieces-> [bind] double-linked-list ll! else 2drop then
  ;m method make-add-piece
  m: ( translate-pieces -- ) \ add input pieces to translated-pieces list
\    pieces-> [bind] pieces piece-quantity 0 ?do
\      i pieces-> [bind] pieces voxel-quantity 0 ?do
\        j pieces-> [bind] pieces getvoxel working-pieces-> add-voxel
\        j pieces-> [bind] pieces nextvoxel
\      loop
\      working-pieces-> [bind] pieces define
\    loop
    pieces-> [bind] pieces piece-quantity 0 ?do
      i pieces-> this make-add-piece
    loop
  ;m method add-start-pieces
  public
  m: ( translate-pieces -- )
    pieces heap-new [to-inst] working-pieces->
    puzzle-pieces [to-inst] pieces->
    double-linked-list heap-new [to-inst] translated-pieces->
    puzzle-board-dimensions [bind] voxel-board-mapping get-board-dims this set-board-dims
    this add-start-pieces
  ;m overrides construct
  m: ( translate-pieces -- ) \ destructor
    this destruct
    translated-pieces-> [bind] double-linked-list destruct
    working-pieces-> [bind] pieces destruct
  ;m overrides destruct
  m: ( uindex translate-pieces -- uboard-> ubytes ) \ get uindex board from translated pieces list
    translated-pieces-> [bind] double-linked-list ll-set-start
    0 ?do translated-pieces-> [bind] double-linked-list ll> drop loop
    translated-pieces-> [bind] double-linked-list ll@
  ;m method get-board-piece
  m: ( uboard voxel-board-mapping -- upieces ) \ will extract all the voxels from the binary map in uboard and put them in upieces object
  \ note upieces is the internal working-pieces-> object instance of pieces and it will be cleared before use
  \ note board-a is clobered in this algorithom
    { uboard }
    working-pieces-> [bind] pieces destruct
    working-pieces-> [bind] pieces construct
    x-max 0 ?do
      y-max 0 ?do
        z-max 0 ?do
          this clear-board
          k j i this set-board-voxel
          uboard board-a @ this pieces-overlaping?
          if k j i working-pieces-> [bind] pieces add-voxel then
        loop
      loop
    loop
    working-pieces-> [bind] pieces define
    working-pieces->
  ;m method get-voxels-from-board
  m: ( translate-pieces -- ) \ print
    cr this [parent] print cr
    this get-board-dims rot . swap . . ."  x y z dimensions of this puzzle board!" cr
    working-pieces-> [bind] pieces piece-quantity . ." pieces in working list!" cr
    working-pieces-> [bind] pieces piece-quantity 0 ?do
      i . ."  piece" cr
      i working-pieces-> [bind] pieces voxel-quantity 0 ?do
        j working-pieces-> [bind] pieces getvoxel rot . space swap . space . ." voxel!" cr
        j working-pieces-> [bind] pieces nextvoxel
      loop
    loop
    translated-pieces-> [bind] double-linked-list ll-size@ . ."  pieces in translated-pieces!" cr
    translated-pieces-> [bind] double-linked-list ll-set-start
    translated-pieces-> [bind] double-linked-list ll-size@ 0 ?do
      i . ."  piece" cr
      translated-pieces-> [bind] double-linked-list ll@> drop dump
    loop
  ;m overrides print
end-class translate-pieces

create translated-pieces translate-pieces dict-new drop

translated-pieces print cr
puzzle-pieces print cr
0 translated-pieces get-board-piece pad swap move
pad translated-pieces get-voxels-from-board
bind pieces print cr
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
