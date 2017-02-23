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

include ./newpuzzle.def
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
