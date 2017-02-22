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
  protected
  cell% inst-var a-voxel
  cell% inst-var a-voxel-list
  public
  m: ( -- ) \ construct
    voxel heap-new a-voxel !
    double-linked-list heap-new a-voxel-list !
  ;m overrides construct
  m: ( -- ) \ destruct
    a-voxel @ free throw
    \ note must free all the voxel objects here before the destruct of the double-linked-list object next
    a-voxel-list @ [bind] double-linked-list destruct
    a-voxel-list @ free throw
  ;m overrides destruct
  m: ( ux uy uz piece -- )
    a-voxel @ [bind] voxel voxel!
    a-voxel @ sp@ cell a-voxel-list @ [bind] double-linked-list ll! drop  
    voxel heap-new a-voxel !
  ;m method add-voxel
  m: ( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel
    a-voxel-list @ [bind] double-linked-list ll-set-start
    0 ?do a-voxel-list @ [bind] double-linked-list ll> drop loop
    a-voxel-list @ [bind] double-linked-list ll@ drop @ [bind] voxel voxel@
  ;m method get-voxel
end-class piece

voxel heap-new constant testvoxel
cr
10 20 30 testvoxel voxel! .s cr
testvoxel voxel@ . . . cr
15 25 35 testvoxel voxel! .s cr
testvoxel voxel@ . . . cr

piece heap-new constant testpiece
cr
3 2 1 testpiece add-voxel .s cr
5 8 2 testpiece add-voxel .s cr

0 testpiece get-voxel . . . cr
1 testpiece get-voxel . . . cr
