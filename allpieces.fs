require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs

object class
  destruction implementation
  protected
  cell% inst-var working-board      \ board object to do board placement testing
  cell% inst-var start-pieces       \ pieces object will contain all the valid starting pieces
  cell% inst-var all-pieces         \ pieces object will contain all the valid rotated translated pieces created from start-pieces
  cell% inst-var working-piece      \ piece object used to process or temporary holder of a piece
  cell% inst-var rotated-pieces     \ pieces object containing all the rotated pieces working on before adding to all-pieces list
  cell% inst-var translated-pieces  \ pieces object conatining all the translated pieces working on before adding to all-pieces list
  m: ( make-all-pieces -- ) \ reset working-board that points to a piece object
    working-board @ [bind] board destruct
    working-board @ [bind] board construct
    puzzle-board [bind] board get-board-dims working-board @ [bind] board set-board-dims
  ;m method reset-working-board
  m: ( upieces make-all-pieces -- ) \ upieces is a pieces object containing the all the start puzzle pieces
    \ this method takes those start pieces and puts the pieces that fit on board into start-pieces pieces list
    { uindex upieces }
    \ upieces [bind] pieces pieces-quantity@ 0 ?do
      uindex upieces [bind] pieces get-a-piece working-board @ [bind] board piece-on-board? true =
      if uindex upieces [bind] pieces get-a-piece start-pieces @ [bind] pieces add-a-piece then
    \ loop
    this reset-working-board
  ;m method the-start-pieces
  m: ( udim1 udim2 uaxis -- udim3 udim4 ) \ uaxis is 0 to 7 which axis rotation to perform
    CASE
      0 of ENDOF
      1 of swap negate ENDOF
      2 of negate swap ENDOF
      3 of negate swap negate ENDOF
      4 of swap negate swap ENDOF
      5 of negate endof
      6 of swap ENDOF
      7 of negate swap negate swap ENDOF
    ENDCASE ;m method do-axis-rotation
  m: ( x y z uaxis utype -- x1 y1 z1 ) \ uaxis is 0 to 7 for rotation axis .. utype is 0 to 2 for x y or z type
    { x y z uaxis utype }
    utype CASE
      0 of x y uaxis this [current] do-axis-rotation z ENDOF
      1 of x z uaxis this [current] do-axis-rotation y swap ENDOF
      2 of y z uaxis this [current] do-axis-rotation x rot rot ENDOF
    ENDCASE ;m method do-rotation

  m: ( upiece make-all-pieces -- ) \ upiece is a piece object that is used to create translated pieces and place them in translated-pieces
    puzzle-board [bind] board get-board-dims 0 0 0 { upiece x-max y-max z-max x y z }
    x-max 0 ?do i to x
      y-max 0 ?do i to y
        z-max 0 ?do i to z
          upiece [bind] piece voxel-quantity@ 0 ?do
            i upiece [bind] piece get-voxel z + swap y + swap rot x + rot rot
            working-piece @ [bind] piece add-voxel
          loop
          working-piece @ translated-pieces @ [bind] pieces add-a-piece
          working-piece @ [bind] piece destruct
          working-piece @ [bind] piece construct
        loop
      loop
    loop ;m method translate
  m: ( upiece make-all-pieces -- ) \ upiece is a piece object that is used to create rotated pieces and place them in rotated-pieces
    { upiece }
    3 0 do
      8 0 do
        upiece [bind] piece voxel-quantity@ 0 ?do
          i upiece [bind] piece get-voxel j k this [current] do-rotation
          working-piece @ [bind] piece add-voxel
        loop
        working-piece @ rotated-pieces @ [bind] pieces add-a-piece
        working-piece @ [bind] piece destruct
        working-piece @ [bind] piece construct
      loop
    loop ;m method rotate
  m: ( upieces make-all-pieces -- ) \ will add all the uniqu upieces to all-pieces list
    0 0 { upieces testpiece result }
    upieces [bind] pieces pieces-quantity@ 0 ?do
      i upieces [bind] pieces get-a-piece to testpiece 0 to result
      testpiece working-board @ [bind] board piece-on-board? true = if
        all-pieces @ [bind] pieces pieces-quantity@ 0 ?do
          testpiece i all-pieces @ [bind] pieces get-a-piece
          [bind] piece same? result or to result
        loop
        result false = if testpiece all-pieces @ [bind] pieces add-a-piece then
      then
    loop
  ;m method add-to-all-pieces
  m: ( make-all-pieces -- ) \ will take start-pieces and create all rotations and translated pieces then add to all-pieces if not there already
    start-pieces @ [bind] pieces pieces-quantity@ 0 ?do
      i start-pieces @ [bind] pieces get-a-piece
      this [current] rotate
      rotated-pieces @ [bind] pieces pieces-quantity@ 0 ?do
        i rotated-pieces @ [bind] pieces get-a-piece
        this [current] translate
        translated-pieces @ this [current] add-to-all-pieces
        translated-pieces @ [bind] pieces destruct
        translated-pieces @ [bind] pieces construct
      loop
      rotated-pieces @ [bind] pieces destruct
      rotated-pieces @ [bind] pieces construct
    loop
  ;m method all-rotations-translations
  public
  m: ( uindex upieces make-all-pieces -- upieces2 ) \ constructor
    \ uindex is the reference to the pieces object piece defined in newpuzzle.def file
    \ upieces2 is the returned pieces object that contains the total list of pieces that can be in board as defined by upieces and puzzle-board
    \ note puzzle-board contains the dimentions of the board used here
    board heap-new working-board !
    puzzle-board [bind] board get-board-dims working-board @ [bind] board set-board-dims
    pieces heap-new start-pieces !
    pieces heap-new all-pieces !
    piece heap-new working-piece !
    pieces heap-new rotated-pieces !
    pieces heap-new translated-pieces !
    this the-start-pieces   \ create the start pieces then go from there
    this all-rotations-translations
    all-pieces @ \ return the total list of pieces
  ;m overrides construct
  m: ( make-all-pieces -- ) \ destructor
    working-board @ dup [bind] board destruct free throw
    start-pieces @ dup [bind] pieces destruct free throw
    all-pieces @ dup [bind] pieces destruct free throw
    \ note all-pieces is made in constructor but given out by this object so its handle might be out in the wild and invalid after destruct called
    working-piece @ dup [bind] piece destruct free throw
    rotated-pieces @ dup [bind] pieces destruct free throw
    translated-pieces @ dup [bind] pieces destruct free throw
  ;m overrides destruct
end-class make-all-pieces


\ ********************************************************************************************************************************

\ \\\
0 puzzle-pieces make-all-pieces heap-new constant testmap

bind pieces pieces-quantity@ . ." the size of all!" cr
