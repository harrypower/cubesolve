require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

object class
  protected
  char% inst-var x \ voxels have a limit of 0 to 255 in dimentions
  char% inst-var y
  char% inst-var z
  public
  m: ( ux uy uz voxel -- ) \ store the voxel coordinates
    z c! y c! x c! ;m method voxel!
  m: ( -- ) \ constructed
    0 0 0 this voxel! ;m overrides construct
  m: ( voxel -- ux uy uz ) \ retrieve voxel coordinates
    x c@ y c@ z c@ ;m method voxel@
  m: ( uvoxel voxel -- nflag ) \ nflag is true if uvoxel is intersecting with voxel
    { uvoxel }
    uvoxel voxel@ z c@ = swap y c@ = and swap x c@ = and ;m method equivalent?
end-class voxel

object class
  destruction implementation
  selector voxel-quantity@
  protected
  cell% inst-var a-voxel
  cell% inst-var a-voxel-list
  m: ( uindex piece -- ) \ seek to uindex voxel in list
    a-voxel-list @ [bind] double-linked-list ll-set-start
    0 ?do a-voxel-list @ [bind] double-linked-list ll> drop loop
  ;m method seek-voxel
  public
  m: ( piece -- ) \ construct
    voxel heap-new a-voxel !
    double-linked-list heap-new a-voxel-list !
  ;m overrides construct
  m: ( piece -- ) \ destruct
    a-voxel @ free throw
    0 this seek-voxel
    this voxel-quantity@ 0 ?do
     a-voxel-list @ [bind] double-linked-list ll@ drop free throw
     \ note voxel object has nothing allocated so destruct method not present
    loop
    a-voxel-list @ [bind] double-linked-list destruct
    a-voxel-list @ free throw
  ;m overrides destruct
  m: ( ux uy uz piece -- )
    a-voxel @ [bind] voxel voxel!
    a-voxel cell a-voxel-list @ [bind] double-linked-list ll!
    voxel heap-new a-voxel !
  ;m method add-voxel
  m: ( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel in this piece
    this seek-voxel
    a-voxel-list @ [bind] double-linked-list ll@ drop @ [bind] voxel voxel@
  ;m method get-voxel
  m: ( piece -- usize ) \ return voxel quantity
    a-voxel-list @ [bind] double-linked-list ll-size@
  ;m overrides voxel-quantity@
end-class piece

object class
  destruction implementation
  selector pieces-quantity@
  protected
  cell% inst-var a-piece
  cell% inst-var a-pieces-list
  m: ( uindex piece -- ) \ seek to uindex piece in a-pieces-list
    a-pieces-list @ [bind] double-linked-list ll-set-start
    0 ?do a-pieces-list @ [bind] double-linked-list ll> drop loop
  ;m method seek-piece
  public
  m: ( pieces -- ) \ construct
    piece heap-new a-piece !
    double-linked-list heap-new a-pieces-list !
  ;m overrides construct
  m: ( pieces -- ) \ destruct
    a-piece @ [bind] piece destruct
    a-piece @ free throw
    this pieces-quantity@ 0 ?do
      a-pieces-list @ [bind] double-linked-list ll@ drop @ [bind] piece destruct
      a-pieces-list @ [bind] double-linked-list ll@ drop free throw
    loop
    a-pieces-list @ [bind] double-linked-list destruct
    a-pieces-list @ free throw
  ;m overrides destruct
  m: ( upiece pieces -- ) \ copies contents of upiece object and puts the copied piece object in a-pieces-list
    { upiece }
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel a-piece @ [bind] piece add-voxel
    loop
    a-piece cell a-pieces-list @ [bind] double-linked-list ll!
    piece heap-new a-piece !
  ;m method add-a-piece
  m: ( uindex pieces -- upiece ) \ retrieve uindex piece from a-pieces-list
    this seek-piece
    a-pieces-list @ [bind] double-linked-list ll@ drop @
  ;m method get-a-piece
  m: ( pieces -- usize ) \ return the quantity of pieces in this piece list
    a-pieces-list @ [bind] double-linked-list ll-size@
  ;m overrides pieces-quantity@
end-class pieces

pieces heap-new constant puzzle-pieces
piece heap-new constant working-piece

: define-a-piece ( -- ) \ make a piece in working-piece  and place it into puzzle-pieces
  working-piece puzzle-pieces add-a-piece
  working-piece destruct
  working-piece construct ;
: define-a-voxel ( ux uy uz -- ) \ make a voxel and put it into working-piece
  working-piece add-voxel ;

object class
  destruction implementation
  selector set-board-dims
  protected
  struct
    cell% field board-cell
  end-struct board-cell%
  cell% inst-var board-array        \ pointer to cell sized array that contains index #'s refering to pieces in the board-pieces-list
  cell% inst-var board-pieces-list  \ pointer to a pieces objects that contain piece objects that are currently in the board-array
\  cell% inst-var a-voxel            \ pointer to a voxel object
  cell% inst-var a-piece            \ pointer to a piece object
  inst-value x-max                  \ values containing the x y and z max board dimensions
  inst-value y-max
  inst-value z-max
  inst-value z-mult                 \ calculated value for the z multiplier used to find board-array memory location
  m: ( ux uy uz board -- uaddr ) \ calculated board-array address
    z-mult * swap x-max * + + board-cell% %size * board-cell board-array @ + ;m method calc-board-array
    \ z-mult * swap y-max * + + cell * board-array @ + ;m method calc-board-array
  m: ( uvalue ux uy uz board -- ) \ place uvalue into the board-array at location ux uy uz
    this calc-board-array ! ;m method board-array!
  m: ( ux uy uz board -- uvalue ) \ retrieve uvalue from board-array at location ux uy uz
    this calc-board-array @ ;m method board-array@
  public
  m: ( board -- ) \ constructor
    pieces heap-new board-pieces-list !
    0 0 0 this set-board-dims
  ;m overrides construct
  m: ( board -- ) \ destructor
    board-array @ free throw
    board-pieces-list @ [bind] pieces destruct
    board-pieces-list @ free throw
  ;m overrides destruct
  m: ( ux uy uz board -- ) \ set max board size and allocate the board-array memory
    [to-inst] z-max [to-inst] y-max [to-inst] x-max
    x-max y-max * z-max * board-cell% %size * allocate throw board-array !
    x-max y-max * [to-inst] z-mult
    x-max 0 ?do
      y-max 0 ?do
        z-max 0 ?do
          true k j i this board-array! \ place true into array to show no pieces
        loop
      loop
    loop ;m overrides set-board-dims
  m: ( board -- ux-max uy-max uz-max ) \ get dimensions of this board
    x-max y-max z-max ;m method get-board-dims
  m: ( ux uy uz board -- nflag ) \ ux uy uz is a voxel to test if it can be placed on an empty board
    \ nflag is true if ux uy uz can be on the board
    \ nflag is false if ux uy uz can not be on the board
    dup z-max < swap 0 >= and swap
    dup y-max < swap 0 >= and and swap
    dup x-max < swap 0 >= and and ;m method voxel-on-board?
  m: ( upiece board -- nflag ) \ test if upiece can be placed on an empty board
    { upiece } true
    upiece [bind] piece voxel-quantity@ 0 ?do
      i upiece [bind] piece get-voxel this voxel-on-board? and
    loop ;m method piece-on-board?
  m: ( upiece board -- nflag ) \ test if upiece could be placed on the current populated board

  ;m method piece-on-this-board?
\  m: ( ux uy uz board -- uvalue ) \ test word to see board numbers at ux uy uz
\    this board-array@ ;m method see-board-x
\  m: ( uvalue ux uy uz board -- ) \ test word to uvalue board number at ux uy uz
\    this board-array! ;m method set-board-x
end-class board

board heap-new constant puzzle-board

include ./newpuzzle.def

voxel heap-new value testvoxela
voxel heap-new value testvoxelb
0 5 7 testvoxela voxel!
0 5 7 testvoxelb voxel!
testvoxelb testvoxela equivalent? . cr
testvoxela construct
testvoxelb testvoxela equivalent? . cr
7 5 0 testvoxela voxel!
testvoxelb testvoxela equivalent? . cr

\\\
puzzle-pieces pieces-quantity@ . ." pieces" cr .s cr
0 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 0" cr
1 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 1" cr
2 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 2" cr
3 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 3" cr
4 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 4" cr
5 puzzle-pieces get-a-piece puzzle-board piece-on-board? . ."  piece 5" cr

\\\
0 0 0 puzzle-board voxel-on-board? . .s cr
4 4 4 puzzle-board voxel-on-board? . .s cr
5 5 5 puzzle-board voxel-on-board? . .s cr
-1 -1 0 puzzle-board voxel-on-board? . .s cr

\\\
board heap-new constant testboard
cr
: seetheboard
  5 0 ?do
    5 0 ?do
      5 0 ?do
        i j k testboard see-board-x . ." value " i . ." x " j . ." y " k . ." z " .s cr
      loop
    loop
  loop ;

5 5 5 testboard set-board-dims
seetheboard
testboard get-board-dims . . . cr

256 0 0 0 testboard set-board-x
0 0 0 testboard see-board-x . cr
1 0 0 testboard see-board-x . cr .s cr
92392323 3 1 4 testboard set-board-x
234 0 2 4 testboard set-board-x
1932 2 0 1 testboard set-board-x
1984 3 3 3 testboard set-board-x
2016 4 4 4 testboard set-board-x
seetheboard
.s cr

\\\
cr
puzzle-pieces pieces-quantity@ . ." pieces!" cr
: seepieces
  dup puzzle-pieces get-a-piece voxel-quantity@ 0 ?do
     dup puzzle-pieces get-a-piece i swap get-voxel . . . cr
  loop drop ;
cr
0 seepieces ." piece 0 " cr
1 seepieces ." piece 1 " cr
2 seepieces ." piece 2 " cr

\\\
pieces heap-new constant testpieces
piece heap-new constant testpiece

0 0 0 testpiece add-voxel
1 0 0 testpiece add-voxel
1 1 0 testpiece add-voxel
2 1 0 testpiece add-voxel
3 1 0 testpiece add-voxel
testpiece testpieces add-a-piece

0 0 0 testpiece add-voxel
1 0 0 testpiece add-voxel
1 1 0 testpiece add-voxel
2 1 0 testpiece add-voxel
3 1 0 testpiece add-voxel
testpiece testpieces add-a-piece

testpieces pieces-quantity@ . cr
1 testpieces get-a-piece voxel-quantity@ dup . cr

: seepieces
  dup testpieces get-a-piece voxel-quantity@ 0 ?do
     dup testpieces get-a-piece i swap get-voxel . . . cr
  loop drop ;
cr
1 seepieces cr
0 seepieces

\\\
voxel heap-new constant testvoxel
cr
10 20 30 testvoxel voxel! .s cr
testvoxel voxel@ . . . cr
15 25 35 testvoxel voxel! .s cr
testvoxel voxel@ . . . cr
.s cr

piece heap-new constant testpiece
cr
3 2 1 testpiece add-voxel .s cr
5 8 2 testpiece add-voxel .s cr


0 testpiece get-voxel . . . cr
1 testpiece get-voxel . . . cr

testpiece voxel-quantity@ . cr .s cr

testpiece destruct .s cr
testpiece construct .s cr
testpiece voxel-quantity@ . cr
