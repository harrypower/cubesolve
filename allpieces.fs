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
    { upieces }
    upieces [bind] pieces pieces-quantity@ 0 ?do
      i upieces [bind] pieces get-a-piece working-board @ [bind] board piece-on-board? true =
      if i upieces [bind] pieces get-a-piece start-pieces @ [bind] pieces add-a-piece then
    loop
    this reset-working-board
  ;m method the-start-pieces
  public
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
    loop
  ;m method translate
  m: ( upiece make-all-pieces -- ) \ upiece is a piece object that is used to create rotated pieces and place them in rotated-pieces
  ;m method rotate
  m: ( make-all-pieces -- ) \ constructor
    board heap-new working-board !
    puzzle-board [bind] board get-board-dims working-board @ [bind] board set-board-dims
    pieces heap-new start-pieces !
    pieces heap-new all-pieces !
    piece heap-new working-piece !
    pieces heap-new rotated-pieces !
    pieces heap-new translated-pieces !
    puzzle-pieces this the-start-pieces   \ create the start pieces then go from there
  ;m overrides construct
  m: ( make-all-pieces -- ) \ destructor
    working-board @ dup [bind] board destruct free throw
    start-pieces @ dup [bind] pieces destruct free throw
    \ note all-pieces is make in constructor but given out by this object so not destroyed here!
    working-piece @ dup [bind] piece destruct free throw
    rotated-pieces @ dup [bind] pieces destruct free throw
    translated-pieces @ dup [bind] pieces destruct free throw
  ;m overrides destruct
  m: ( make-all-pieces -- ) \ test start
    start-pieces @ [bind] pieces pieces-quantity@ 0 ?do
      i start-pieces @ [bind] pieces get-a-piece working-board @ [bind] board place-piece-on-board drop
    loop
    working-board @ [bind] board see-board
    cr start-pieces @ [bind] pieces pieces-quantity@ . ." qnt" cr
    this reset-working-board
  ;m method test-start
  m: ( uindexT uindexS make-all-pieces -- ) \ test translate
    start-pieces @ [bind] pieces get-a-piece this [current] translate
    translated-pieces @ [bind] pieces get-a-piece
    working-board @ [bind] board place-piece-on-board drop
    working-board @ [bind] board see-board
    translated-pieces @ [bind] pieces pieces-quantity@ . ." translated pieces qnt" cr
    this [current] reset-working-board
    translated-pieces @ [bind] pieces destruct
    translated-pieces @ [bind] pieces construct
  ;m method test-translate
end-class make-all-pieces


\ ********************************************************************************************************************************
\ \\\
make-all-pieces heap-new constant testmap

testmap test-start page
1 0 testmap test-translate
