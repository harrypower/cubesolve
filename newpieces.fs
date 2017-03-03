require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

interface
  selector intersect? ( n n -- nflag )
end-interface intersect

object class
  intersect implementation
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
  m: ( uvoxel voxel -- nflag ) \ nflag is true if uvoxel is intersecting with voxel nflag is false if not intersecting
    { uvoxel }
    uvoxel voxel@ z c@ = swap y c@ = and swap x c@ = and ;m overrides intersect?
end-class voxel

object class
  destruction implementation
  intersect implementation
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
  m: ( uindex piece -- uvoxel ) \ return a voxel object at uindex
    this seek-voxel
    a-voxel-list @ [bind] double-linked-list ll@ drop @ ;m method get-voxel-object
  m: ( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel in this piece
    this get-voxel-object [bind] voxel voxel@
  ;m method get-voxel
  m: ( piece -- usize ) \ return voxel quantity
    a-voxel-list @ [bind] double-linked-list ll-size@
  ;m overrides voxel-quantity@
  m: ( upiece piece -- nflag ) \ test for intersection of upiece with this piece on any voxel
    \ nflag is true if intersection happens
    \ nflag is false if no intersection happens
    { upiece } 0
    upiece voxel-quantity@ 0 ?do
      this voxel-quantity@ 0 ?do
        i this get-voxel-object
        j upiece get-voxel-object
        [bind] voxel intersect? or
      loop
    loop
  ;m overrides intersect?
end-class piece

object class
  destruction implementation
  selector pieces-quantity@
  protected
  cell% inst-var a-piece
  cell% inst-var a-pieces-list
  public
  m: ( pieces -- ) \ construct
    piece heap-new a-piece !
    double-linked-list heap-new a-pieces-list !
  ;m overrides construct
  m: ( pieces -- ) \ destruct
    a-piece @ [bind] piece destruct
    a-piece @ free throw
    a-pieces-list @ [bind] double-linked-list ll-set-start
    this pieces-quantity@ 0 ?do
      a-pieces-list @ [bind] double-linked-list ll@ drop @ [bind] piece destruct
      a-pieces-list @ [bind] double-linked-list ll@ drop @ free throw
      a-pieces-list @ [bind] double-linked-list ll> drop
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
    a-pieces-list @ [bind] double-linked-list nll@ drop @
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


\ **********************************************************************************************************************
\\\
0 0 0 define-a-voxel
1 0 0 define-a-voxel
1 1 0 define-a-voxel
2 1 0 define-a-voxel
3 1 0 define-a-voxel
define-a-piece

0 0 0 define-a-voxel
1 0 0 define-a-voxel
2 1 0 define-a-voxel
3 1 0 define-a-voxel
define-a-piece
puzzle-pieces bind pieces destruct

\\\
piece heap-new constant working2
working-piece voxel-quantity@ . cr
1 2 3 working-piece add-voxel
0 4 2 working-piece add-voxel
1 3 3 working2 add-voxel
0 7 2 working2 add-voxel
0 4 2 working2 add-voxel

working2 working-piece intersect? . cr

\\\
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
