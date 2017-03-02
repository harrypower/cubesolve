require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./newpieces.fs
require ./puzzleboard.fs

object class
  destruction implementation
  protected
  cell% inst-var start-pieces       \ pieces object will contain all the valid starting pieces
  cell% inst-var all-pieces         \ pieces object will contain all the valid rotated translated pieces created from start-pieces
  cell% inst-var working-piece      \ piece object used to process or temporary holder of a piece
  cell% inst-var rotated-pieces     \ pieces object containing all the rotated pieces working on before adding to all-pieces list
  cell% inst-var translated-pieces  \ pieces object conatining all the translated pieces working on before adding to all-pieces list
  public
  m: ( upiece make-all-pieces -- ) \ upiece is a piece object that is used to create translated pieces and place them in translated-pieces
  ;m method translate
  m: ( upiece make-all-pieces -- ) \ upiece is a piece object that is used to create rotated pieces and place them in rotated-pieces
  ;m method rotate
  m: ( upiece make-all-pieces -- ) \ upiece is a piece object containing the all the start puzzle pieces
    \ this method takes those start pieces and puts the non intersecting pieces that fit on board into start-pieces pieces list
  ;m method the-start-pieces
  m: ( make-all-pieces -- ) \ constructor
    pieces heap-new start-pieces !
    pieces heap-new all-pieces !
    piece heap-new working-piece !
    pieces heap-new rotated-pieces !
    pieces heap-new translated-pieces !
  ;m overrides constructor
  m: ( make-all-pieces -- ) \ destructor
    start-pieces @ dup [bind] pieces destruct free throw
    \ note all-pieces is make in constructor but given out by this object so not destroyed here!
    working-piece @ dup [bind] piece destruct free throw
    rotated-pieces @ dup [bind] pieces destruct free throw
    translated-pieces @ dup [bind] pieces destruct free throw 
  ;m overrides destruct

end-class make-all-pieces
