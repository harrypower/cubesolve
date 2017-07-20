require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./serialize-obj.fs

[ifundef] intersect
  interface
    selector intersect? ( n object-name -- nflag )
  end-interface intersect
[endif]
[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  intersect implementation
  protected
  char% inst-var x \ voxels have a limit of 0 to 255 in dimentions
  char% inst-var y
  char% inst-var z
  m: ( uvalue voxel -- uvalue1 ) \ convert uvalue to correct 2's complement
    dup %10000000 >= if -256 or then ;m method adjust
  public
  m: ( ux uy uz voxel -- ) \ store the voxel coordinates
    z c! y c! x c! ;m method voxel!
  m: ( voxel -- ) \ constructed
    0 0 0 this voxel! ;m overrides construct
  m: ( voxel -- ux uy uz ) \ retrieve voxel coordinates
    x c@ this adjust y c@ this adjust z c@ this adjust ;m method voxel@
  m: ( uvoxel voxel -- nflag ) \ nflag is true if uvoxel is intersecting with voxel nflag is false if not intersecting
    voxel@ z c@ this adjust = swap y c@ this adjust = and swap x c@ this adjust = and ;m overrides intersect?
end-class voxel

defer -piece
save-instance-data class
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
    this [parent] construct
    voxel heap-new a-voxel !
    double-linked-list heap-new a-voxel-list !
  ;m overrides construct
  m: ( piece -- ) \ destruct
    this [parent] destruct
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
    { upiece }
    try
      upiece voxel-quantity@ 0 ?do
        this voxel-quantity@ 0 ?do
          i this get-voxel-object
          j upiece get-voxel-object
          [bind] voxel intersect? throw
        loop
      loop
      false
    restore
    endtry
  ;m overrides intersect?
  m: ( upiece piece -- nflag ) \ test upiece agains this piece for exact voxels match forward or backward
    \ nflag is true if all voxels match either forward or backward
    \ nflag is false if there is no complete match forward or backward
    0 0 { upiece forwardtest backwardtest }
    upiece voxel-quantity@ this voxel-quantity@ = if
      upiece voxel-quantity@ 0 ?do
        i this get-voxel-object
        i upiece get-voxel-object
        [bind] voxel intersect? true = if forwardtest 1 + to forwardtest then
      loop
      upiece voxel-quantity@ 0 ?do
        upiece voxel-quantity@ 1 - i - upiece get-voxel-object
        i this get-voxel-object
        [bind] voxel intersect? true = if backwardtest 1 + to backwardtest then
      loop
      upiece voxel-quantity@ dup forwardtest = swap backwardtest = or \ true if either backward or forward finds all voxels match
    else
      false
    then
  ;m method same?
  m: ( upiece piece -- ) \ exact copy upiece to this piece
    { upiece }
    this destruct this construct
    upiece voxel-quantity@ 0 ?do
      i upiece get-voxel this add-voxel
    loop
  ;m method copy
  m: ( piece -- nstrings ) \ to save this data
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
  ;m overrides serialize-data@
  m: ( nstrings piece -- ) \ to restore previously saved data
    this destruct
  ;m overrides serialize-data@
end-class piece
' piece is -piece

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
