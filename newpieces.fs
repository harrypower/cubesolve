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
  m: ( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel
    this seek-voxel
    a-voxel-list @ [bind] double-linked-list ll@ drop @ [bind] voxel voxel@
  ;m method get-voxel
  m: ( piece -- usize ) \ return voxel quantity
    a-voxel-list @ [bind] double-linked-list ll-size@
  ;m overrides voxel-quantity@
end-class piece

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
