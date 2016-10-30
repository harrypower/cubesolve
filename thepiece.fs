require objects.fs

: #to$ ( n -- c-addr u1 ) \ convert n to string
    s>d
    swap over dabs
    <<# #s rot sign #> #>> ;

interface
    selector destruct ( -- ) \ to free allocated memory in objects that use this
end-interface destruction

\ piece object *******************************************************************************************************
object class
  destruction implementation
  protected \ ********************************************************************************************************
  struct
    cell% field x
    cell% field y
    cell% field z
  end-struct loc%
  struct
    loc% field a
    loc% field b
    loc% field c
    loc% field d
    loc% field e
  end-struct blc%
  create base-shapes
  base-shapes blc% %size 4 * dup allot erase \ 4 sets of piece data
  create shapes-x
  shapes-x blc% %size 4 * 2 * dup allot erase \ data with x translation
  create shapes-xy
  shapes-xy blc% %size 4 * 2 * 5 * dup allot erase \ data with x and y translation
  create shapes-xyz
  shapes-xyz blc% %size 2 * 5 * 4 * 4 *  dup allot erase \ basic piece one orientation
  \ 2 x translation placements 5 y translation placements 4 z translation placements of 4 sets of pieces
  create all-orient
  all-orient blc% %size 2 * 5 * 4 * 4 * 6 * dup allot erase \ all pieces list
  \ 6 transformations from basic piece orientation

  20 constant bshape-max
  40 constant sx-max
  200 constant sxy-max
  800 constant sxyz-max
  4800 constant allorient-max
  960 constant pindex-max
  1000 constant nopiece
  false variable piece-table-created piece-table-created ! \ used at construct time to create shape data only once

  inst-value thispiece# \ used to hold this piece current number
  inst-value collisionlist-addr  \ the address to start of collision list
  inst-value collisionlist-flag  \ a flag if true then collision list is valid false means list not calculated yet

  struct
    char% field piece-flag
  end-struct collisionlist%

  struct
    char% field adjacent-flag
  end-struct adjacentlist%
  inst-value adjacent-addr \ the address to start of adjacent possible list
  inst-value adjacentlist-flag \ flag if true then adjacent list is valid false means list not calculated yet

  protected
  m: ( nx ny nz naddr nindex piece -- )
    loc% %size * + { naddr }
    naddr z ! naddr y ! naddr x !
  ;m method bulk!
  m: ( naddr nindex piece -- nx ny nz )
    loc% %size * + dup dup
    x @ -rot y @ swap z @
  ;m method bulk@
  m: ( nx ny nz nbase-shapes-addr nindex piece -- ) \ to store basic-shape data array
    blc% %size * + { nbsa }
    nbsa z ! nbsa y ! nbsa x !
  ;m method basicshape!
  m: ( nbase-shapes-addr nindex piece -- nx ny nz ) \ get basic-shape x y z data
    blc% %size * + dup dup
    x @ -rot y @ swap z @
  ;m method basicshape@
  m: ( piece -- )
    bshape-max 0 do base-shapes i this [current] bulk@ shapes-x i this [current] bulk! loop
    bshape-max 0 do base-shapes i this [current] bulk@ rot 1 + -rot shapes-x i bshape-max + this [current] bulk! loop
  ;m method creatextrans
  m: ( piece -- )
    sx-max 0 do shapes-x i this [current] bulk@ shapes-xy i this [current] bulk! loop
    sx-max 0 do shapes-x i this [current] bulk@ swap 1 + swap shapes-xy i sx-max + this [current] bulk! loop
    sx-max 0 do shapes-x i this [current] bulk@ swap 2 + swap shapes-xy i sx-max 2 * + this [current] bulk! loop
    sx-max 0 do shapes-x i this [current] bulk@ swap 3 + swap shapes-xy i sx-max 3 * + this [current] bulk! loop
    sx-max 0 do shapes-x i this [current] bulk@ swap 4 + swap shapes-xy i sx-max 4 * + this [current] bulk! loop
  ;m method createxytrans
  m: ( piece -- )
    sxy-max 0 do shapes-xy i this [current] bulk@ shapes-xyz i this [current] bulk! loop
    sxy-max 0 do shapes-xy i this [current] bulk@ 1 + shapes-xyz i sxy-max + this [current] bulk! loop
    sxy-max 0 do shapes-xy i this [current] bulk@ 2 + shapes-xyz i sxy-max 2 * + this [current] bulk! loop
    sxy-max 0 do shapes-xy i this [current] bulk@ 3 + shapes-xyz i sxy-max 3 * + this [current] bulk! loop
  ;m method createxyztrans
  m: ( piece -- )
    sxyz-max 0 do shapes-xyz i this [current] bulk@ all-orient i this [current] bulk! loop
    sxyz-max 0 do shapes-xyz i this [current] bulk@ rot swap all-orient i sxyz-max + this [current] bulk! loop
    sxyz-max 0 do shapes-xyz i this [current] bulk@ rot swap -rot all-orient i sxyz-max 2 * + this [current] bulk! loop
    sxyz-max 0 do shapes-xyz i this [current] bulk@ swap all-orient i sxyz-max 3 * + this [current] bulk! loop
    sxyz-max 0 do shapes-xyz i this [current] bulk@ rot all-orient i sxyz-max 4 * + this [current] bulk! loop
    sxyz-max 0 do shapes-xyz i this [current] bulk@ -rot all-orient i sxyz-max 5 * + this [current] bulk! loop
  ;m method all6rotations
  m: ( nx1 ny1 nz1 nx2 ny2 nz2 piece -- nflag ) \ compare nx1 ny1 nz1 to nx2 ny2 nz2
    >r >r >r rot r> = rot r> = rot r> = and and
  ;m method test-voxel?
  m: ( nx ny nz nindex piece -- nflag ) \ compare nx ny nz voxel to nindex piece all voxels
  \ nflag is false if no collisions
  \ nflag is true for any collision
    { nx ny nz nindex }
    try
      nx ny nz all-orient a nindex this [current] basicshape@ this [current] test-voxel? throw
      nx ny nz all-orient b nindex this [current] basicshape@ this [current] test-voxel? throw
      nx ny nz all-orient c nindex this [current] basicshape@ this [current] test-voxel? throw
      nx ny nz all-orient d nindex this [current] basicshape@ this [current] test-voxel? throw
      nx ny nz all-orient e nindex this [current] basicshape@ this [current] test-voxel? throw
      false
    restore
    endtry
  ;m method test-voxeltovoxels?
  m: ( nindex1 nindex2 piece -- nflag ) \ compare one piece for collision with another piece
  \ nflag is false if no collisions
  \ nflag is true for any collision
    { nindex1 nindex2 }
    try
      all-orient a nindex1 this [current] basicshape@ nindex2 this [current] test-voxeltovoxels? throw
      all-orient b nindex1 this [current] basicshape@ nindex2 this [current] test-voxeltovoxels? throw
      all-orient c nindex1 this [current] basicshape@ nindex2 this [current] test-voxeltovoxels? throw
      all-orient d nindex1 this [current] basicshape@ nindex2 this [current] test-voxeltovoxels? throw
      all-orient e nindex1 this [current] basicshape@ nindex2 this [current] test-voxeltovoxels? throw
      false
    restore
    endtry
  ;m method test-collision?
  m: ( n1 n2 piece -- nflag ) \ test n1 against n2 nflag true if n1 is -1 or +1 from n2
    \ -2 if n1 n2 is > or < 2 from each other
    \ false if non of these conditions are meet
    { n1 n2 }
    n1 n2 1 + = n1 n2 1 - = or
    n1 n2 1 + > n1 n2 1 - < or if -2 else false then +
  ;m method test-distance?
  m: ( nx1 ny1 nz1 nx2 ny2 nz2 piece -- nflag ) \ compare nx1 ny1 nz1 to nx2 ny2 nz2 for adjacent voxel
    \ true only if 2 shared dimentions and the other dimention is +- 1 value away
    \ false if the aboce condition is not meet
    { nx2 ny2 nz2 }
    nz2 this [current] test-distance? to nz2
    ny2 this [current] test-distance? to ny2
    nx2 this [current] test-distance? to nx2
    nz2 ny2 nx2 + + -1 = if true else false then
  ;m method test-adj-voxel?
  m: ( nx1 ny1 nz1 nindex piece -- nflag ) \ compair nx ny nz voxel to nindex piece all voxels for adjacent piece
  \ aka 2 shared dimentions and other dimention is -+ 1 value away
    { nx ny nz nindex }
    try
      nx ny nz all-orient a nindex this [current] basicshape@ this [current] test-adj-voxel? throw
      nx ny nz all-orient b nindex this [current] basicshape@ this [current] test-adj-voxel? throw
      nx ny nz all-orient c nindex this [current] basicshape@ this [current] test-adj-voxel? throw
      nx ny nz all-orient d nindex this [current] basicshape@ this [current] test-adj-voxel? throw
      nx ny nz all-orient e nindex this [current] basicshape@ this [current] test-adj-voxel? throw
      false
    restore
    endtry
  ;m method test-adj-voxeltovoxel?
  m: ( nindex1 nindex2 piece -- nflag )  \ will test for adjacent pieces that share 2 dimentions have third dimention is one away from any one piece
    { nindex1 nindex2 }
    try
      all-orient a nindex1 this [current] basicshape@ nindex2 this [current] test-adj-voxeltovoxel? throw
      all-orient b nindex1 this [current] basicshape@ nindex2 this [current] test-adj-voxeltovoxel? throw
      all-orient c nindex1 this [current] basicshape@ nindex2 this [current] test-adj-voxeltovoxel? throw
      all-orient d nindex1 this [current] basicshape@ nindex2 this [current] test-adj-voxeltovoxel? throw
      all-orient e nindex1 this [current] basicshape@ nindex2 this [current] test-adj-voxeltovoxel? throw
      false
    restore
    endtry
  ;m method test-adjacent?
  m: ( piece -- ) \ populate the possible adjacent list for thispiece#
    0 adjacent-addr <> thispiece# nopiece <> adjacentlist-flag false = and and
    if
      pindex-max 0 do
        thispiece# i this [current] test-adjacent?
        adjacent-addr adjacent-flag i adjacentlist% %size * + c!
      loop
      true [to-inst] adjacentlist-flag
    then
  ;m method populateadjacentlist
  m: ( piece -- ) \ populate the collision  list for thispiece#
    0 collisionlist-addr <> thispiece# nopiece <> collisionlist-flag false = and and
    if
      pindex-max 0 do
        thispiece# i this [current] test-collision?
        collisionlist-addr piece-flag i collisionlist% %size * + c!
      loop
      true [to-inst] collisionlist-flag
    then
  ;m method populatecollisionlist
  m: ( piece -- ) \ allocate room for the collision list or clear list if allocated already
    0 collisionlist-addr <>
    if
      collisionlist-addr collisionlist% %size pindex-max * erase
    else
      collisionlist% %size pindex-max * allocate throw [to-inst] collisionlist-addr
      collisionlist-addr collisionlist% %size pindex-max * erase
    then
    false [to-inst] collisionlist-flag
  ;m method create-collisionlist
  m: ( piece -- ) \ allocate room for the adjacent list or clear list if allocated already
    0 adjacent-addr <>
    if
      adjacent-addr adjacentlist% %size pindex-max * erase
    else
      adjacentlist% %size pindex-max * allocate throw [to-inst] adjacent-addr
      adjacent-addr adjacentlist% %size pindex-max * erase
    then
    false [to-inst] adjacentlist-flag
  ;m method create-adjacentlist
  m: ( nx ny nz piece -- ) \ just displays x y z from stack
    rot ." x:" . swap ."  y:" . ."  z:" .
  ;m method xyz.
  public \ ***********************************************************************************************************
  m: ( piece -- )
    piece-table-created @ false = if \ to create piece table only once for all piece objects
      0 0 0 base-shapes a 0 this [current] basicshape! \ first shape
      1 0 0 base-shapes b 0 this [current] basicshape!
      2 0 0 base-shapes c 0 this [current] basicshape!
      2 0 1 base-shapes d 0 this [current] basicshape!
      3 0 1 base-shapes e 0 this [current] basicshape!
      0 0 1 base-shapes a 1 this [current] basicshape! \ second shape
      1 0 1 base-shapes b 1 this [current] basicshape!
      2 0 1 base-shapes c 1 this [current] basicshape!
      2 0 0 base-shapes d 1 this [current] basicshape!
      3 0 0 base-shapes e 1 this [current] basicshape!
      0 0 0 base-shapes a 2 this [current] basicshape! \ third shape
      1 0 0 base-shapes b 2 this [current] basicshape!
      1 0 1 base-shapes c 2 this [current] basicshape!
      2 0 1 base-shapes d 2 this [current] basicshape!
      3 0 1 base-shapes e 2 this [current] basicshape!
      0 0 1 base-shapes a 3 this [current] basicshape! \ fourth shape
      1 0 1 base-shapes b 3 this [current] basicshape!
      1 0 0 base-shapes c 3 this [current] basicshape!
      2 0 0 base-shapes d 3 this [current] basicshape!
      3 0 0 base-shapes e 3 this [current] basicshape!
      this [current] creatextrans
      this [current] createxytrans
      this [current] createxyztrans
      this [current] all6rotations
      \ at this moment the piece data base is populated
      true piece-table-created !
    then
    0 [to-inst] collisionlist-addr \ at construct time the collsion list is not allocated yet
    0 [to-inst] adjacent-addr \ at construct time the adjacent list is not allocated yet
    this [current] create-collisionlist
    this [current] create-adjacentlist
    nopiece [to-inst] thispiece#  \ start with no piece
  ;m overrides construct
  m: ( piece -- ) \ free allocated memory for this piece
    0 collisionlist-addr <> if
      collisionlist-addr free throw
    then
    0 [to-inst] adjacent-addr
    false [to-inst] adjacentlist-flag
    0 [to-inst] collisionlist-addr
    false [to-inst] collisionlist-flag
    nopiece [to-inst] thispiece#
  ;m overrides destruct
  m: ( npiece# piece -- ) \ set the thispiece#
    dup dup 0 >= -rot pindex-max < rot and if [to-inst] thispiece# else drop nopiece [to-inst] thispiece# then
  ;m method piece!
  m: ( piece -- npiece# ) \ get the current thispiece#
    thispiece#
  ;m method piece@
  m: ( piece -- ) \ generate the collision list for thispiece#
   \ note this does clean out the collision list if it already was populated
    this [current] populatecollisionlist
  ;m method collisionlist!
  m: ( piece -- ) \ generate the adjacent list for thispiece#
    this [current] populateadjacentlist
  ;m method adjacentlist!
  m: ( npiece# piece -- ) \ set thispiece# and removed old collision list and generate a new collision list
    this [current] destruct
    this [current] construct
    this [current] piece!
    this [current] collisionlist!
    this [current] adjacentlist!
  ;m method newpiece!
  m: ( npiece# piece -- nflag ) \ test the npiece# collistion value from collision list
  \ nflag is true if npiece# has collided with thispiece# from the collision list
  \ nflag is false if npiece# has not collided with thispiece# in the collision list or the collision list does not exist
    collisionlist-flag true =
    if
      collisionlist-addr piece-flag swap collisionlist% %size * + c@
      if true else false then
    else
      drop false
    then
  ;m method collisionlist?
  m: ( npiece# piece -- nflag ) \ test the npiece# adjacent value from adjacent list
    \ nflag is true if npiece# is adjacent to thispiece# from this object
    \ nflag is false if npiece# is not adjacent to thispiece# from this object or the adjacent list does not exist
    adjacentlist-flag true =
    if
      adjacent-addr adjacent-flag swap adjacentlist% %size * + c@
      if true else false then
    else
      drop false
    then
  ;m method adjacent?
  m: ( nsubpiece# npiece# piece -- nx ny nz ) \ will return sub block xyz values for a given npiece# and a given nsubpiece#
    \ nsubpiece# is 0 to 4
    \ npiece# is 0 to pindex-max -1
    \ note this method returns the correct xyz values all the time from any object instance given the correct input information
    { nsubpiece# npiece# }
    nsubpiece#
    CASE
      0 OF all-orient a npiece# this [current] basicshape@ ENDOF
      1 OF all-orient b npiece# this [current] basicshape@ ENDOF
      2 OF all-orient c npiece# this [current] basicshape@ ENDOF
      3 OF all-orient d npiece# this [current] basicshape@ ENDOF
      4 OF all-orient e npiece# this [current] basicshape@ ENDOF
      \ default simply return a data
      all-orient a npiece# this [current] basicshape@ 3 roll
    ENDCASE
  ;m method subpiece@
  \ testing words to ensure this piece generation is correct ******************************************************************
  m: ( piece -- ) \ testing basic data set creation
    base-shapes e 3 this [current] basicshape@ . . . cr
    base-shapes d 2 this [current] basicshape@ . . . cr
    ." XXXXXXXX" cr
    bshape-max  0 do base-shapes i this [current] bulk@ this [current] xyz. ."  #" i . cr loop
    ." ********" cr
    sx-max 0 do shapes-x i this [current] bulk@ this [current] xyz. ."  #" i . cr loop
    ." yyyyyyyyy" cr
    sxy-max 0 do shapes-xy i this [current] bulk@ this [current] xyz. ."  #" i . cr loop
    ." zzzzzzzzz" cr
    sxyz-max 0 do shapes-xyz i this [current] bulk@ this [current] xyz. ."  #" i . cr loop
    ." all------" cr
    allorient-max 0 do all-orient i this [current] bulk@ this [current] xyz. ."  #" i . cr loop
    pindex-max 0 do
      all-orient a i this [current] basicshape@ rot ." a:" . swap . .
      all-orient b i this [current] basicshape@ rot ." b:" . swap . .
      all-orient c i this [current] basicshape@ rot ." c:" . swap . .
      all-orient d i this [current] basicshape@ rot ." d:" . swap . .
      all-orient e i this [current] basicshape@ rot ." e:" . swap . . ." #" i . cr
    loop
  ;m method testDataSet
  m: ( piece -- ) \ testing collision detection words
    cr
    1 2 3 1 2 3 this [current] test-voxel? . ."  <- should be true!" cr
    1 2 3 1 2 5 this [current] test-voxel? . ."  <- should be false!" cr
    4 0 0 4 0 0 this [current] test-voxel? . ."  <- should be true!" cr
    0 0 0 0 0 0 this [current] test-voxel? . ."  <- should be true!" cr
    4 4 4 4 4 4 this [current] test-voxel? . ."  <- should be true!" cr
    4 0 4 4 4 0 this [current] test-voxel? . ."  <- should be false!" cr
    0 0 this [current] test-collision?  . ."  <- collision should be true!" cr
    0 1 this [current] test-collision?  . ."  <- collision should be true!" cr
    0 25 this [current] test-collision?  . ."  <- collision should be false!" cr
    all-orient a 0 this [current] basicshape@ this [current] xyz. ."  :a 0" cr
    all-orient b 0 this [current] basicshape@ this [current] xyz. ."  :b 0" cr
    all-orient c 0 this [current] basicshape@ this [current] xyz. ."  :c 0" cr
    all-orient d 0 this [current] basicshape@ this [current] xyz. ."  :d 0" cr
    all-orient e 0 this [current] basicshape@ this [current] xyz. ."  :e 0" cr
    all-orient a 25 this [current] basicshape@ this [current] xyz. ."  :a 25" cr
    all-orient b 25 this [current] basicshape@ this [current] xyz. ."  :b 25" cr
    all-orient c 25 this [current] basicshape@ this [current] xyz. ."  :c 25" cr
    all-orient d 25 this [current] basicshape@ this [current] xyz. ."  :d 25" cr
    all-orient e 25 this [current] basicshape@ this [current] xyz. ."  :e 25" cr
    0 0 0 0 this [current] test-voxeltovoxels? . ."  <- collision should be true!" cr
    3 0 1 0 this [current] test-voxeltovoxels? . ."  <- collision should be true!" cr
    3 0 2 0 this [current] test-voxeltovoxels? . ."  <- collision should be false!" cr
  ;m method testcompare
  m: ( piece -- ) \ testing collision detection list processes of this object
    this [current] destruct
    this [current] construct
    500 this [current] piece!
    this [current] collisionlist!
    cr
    500 this [current] collisionlist? . ." < this should be true!" cr
    501 this [current] collisionlist? . ." < this should be true!" cr
    510 this [current] collisionlist? . ." < this should be false!" cr
    520 this [current] collisionlist? . ." < this should be false!" cr
    540 this [current] collisionlist? . ." < this should be true!" cr

    this [current] destruct
    this [current] construct
    0 this [current] piece!
    this [current] collisionlist!
    cr
    0 this [current] collisionlist? . ." < this should be true!" cr
    3 this [current] collisionlist? . ." < this should be true!" cr
    10 this [current] collisionlist? . ." < this should be false!" cr
  ;m method testcollistionlist
  m: ( piece -- ) \ test collision list full
    cr
    this [current] destruct
    this [current] construct
    959 this [current] piece!
    this [current] collisionlist!
    pindex-max 0 do i this [current] collisionlist? i . . cr loop
  ;m method testcollsionlistfull
end-class piece

\ displaypieces object *****************************************************************************************************************
object class
  destruction implementation
  protected
  5 constant displaycellsize
	1 constant topoffset
	1 constant zplane-spacing
  5 constant xyz-size
  struct
    cell% field piecedisplay#
  end-struct thepiece%
  inst-value displaydata-addr
  cell% inst-var displaypiecesetup
  public
  m: ( displaypieces -- )
    displaypiecesetup displaypiecesetup @ =
    if
      displaydata-addr free throw
      0 [to-inst] displaydata-addr
      0 displaypiecesetup !
    then
  ;m overrides destruct
  m: ( displaypieces -- )
    displaypiecesetup displaypiecesetup @ <>
    if \ not setup yet
      thepiece% %size xyz-size * xyz-size * xyz-size * allocate throw [to-inst] displaydata-addr
      displaypiecesetup displaypiecesetup ! \ now this instance has been setup once
    then
    displaydata-addr thepiece% %size xyz-size * xyz-size * xyz-size * true fill \ place no pieces in display data
  ;m overrides construct
  m: ( nx ny nz npiece# displaypieces -- )
    displaypiecesetup displaypiecesetup @ =
    if
      >r
      xyz-size dup * * swap \ z offset
      xyz-size * +          \ y offset
      +                     \ x offset
      thepiece% %size * displaydata-addr piecedisplay# + \ final address
      r> swap !
    else
      2drop 2drop \ no display setup just drop input
    then
  ;m method displaypiece!
  m: ( nx ny nz displaypieces -- npiece# )
  displaypiecesetup displaypiecesetup @ =
  if
    xyz-size dup * * swap \ z offset
    xyz-size * +          \ y offset
    +                     \ x offset
    thepiece% %size * displaydata-addr piecedisplay# + \ final address
    @
  else
    2drop drop true  \ no display setup just drop input and output no piece
  then
  ;m method displaypiece@
  m: ( displaypices -- )
    page
    xyz-size 0 ?do    	\ x
      xyz-size 0 ?do		\ y
        xyz-size 0 ?do	\ z
          k j i this [current] displaypiece@ \ retrieve piece value to display
          dup true = if drop 999 then  \ if no piece then show 99
          k displaycellsize * \ x for at-xy
          xyz-size zplane-spacing + i * j + topoffset + \ y for at-xy
          at-xy
          ." :" #to$ type
        loop
      loop
    loop
  ;m method showdisplay
end-class displaypieces
