require objects.fs

object class
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
  4800 constant  allorient-max
  960 constant pindex-max
  1000 constant nopiece
  false variable piece-table-created piece-table-created ! \ used at construct time to create shape data only once

  inst-value thispiece# \ used to hold this piece current number
  inst-value lastcollisionlist-a \ pointer to the piece collision link list of piece# above
  inst-value nextlistretrieve-a \ index value used by collist! for retrieveing the linked list values
  struct
    cell% field pcollision#
    cell% field nextcollisionlist
  end-struct pcll%

  protected
  m: ( npcollision# piece -- ) \ store piece collision # in lastcollisionlist-a
    pcll% %size allocate throw \ make room
    dup lastcollisionlist-a swap nextcollisionlist ! \ store last list address in this list
    dup -rot pcollision# ! \ store this npcollision# in this list
    [to-inst] lastcollisionlist-a \ update the last pointer to this list address
  ;m method collist!
  m: ( piece -- npcollision# nflag ) \ retrieve piece collisions from next list item
  \ npcollision# is the piece collision value returned
  \ if nflag is false then the list has reached the end and will start returning values from the begining of list
  \ if nflag is true then the linked list has more stuff to retrieve
    nextlistretrieve-a 0 = lastcollisionlist-a 0 = and
    if 0 false
    else
      nextlistretrieve-a 0 =
      if lastcollisionlist-a else nextlistretrieve-a  then
        dup pcollision# @ swap nextcollisionlist @ \ get npcollision# and next address
        dup [to-inst] nextlistretrieve-a \ record next address of list to retrieve
        false = if false else true then \ test if at end of list or not and return nflag
    then ;m method collist@
  m: ( piece -- ) \ populate the collision link list for thispiece#

  ;m method makecollisionlinklist
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
  ;m method bshape!
  m: ( nbase-shapes-addr nindex piece -- nx ny nz ) \ get basic-shape x y z data
    blc% %size * + dup dup
    x @ -rot y @ swap z @
  ;m method bshape@
  m: ( piece -- )
    bshape-max 0 do base-shapes i this bulk@ shapes-x i this bulk! loop
    bshape-max 0 do base-shapes i this bulk@ rot 1 + -rot shapes-x i bshape-max + this bulk! loop
  ;m method creatextrans
  m: ( piece -- )
    sx-max 0 do shapes-x i this bulk@ shapes-xy i this bulk! loop
    sx-max 0 do shapes-x i this bulk@ swap 1 + swap shapes-xy i sx-max + this bulk! loop
    sx-max 0 do shapes-x i this bulk@ swap 2 + swap shapes-xy i sx-max 2 * + this bulk! loop
    sx-max 0 do shapes-x i this bulk@ swap 3 + swap shapes-xy i sx-max 3 * + this bulk! loop
    sx-max 0 do shapes-x i this bulk@ swap 4 + swap shapes-xy i sx-max 4 * + this bulk! loop
  ;m method createxytrans
  m: ( piece -- )
    sxy-max 0 do shapes-xy i this bulk@ shapes-xyz i this bulk! loop
    sxy-max 0 do shapes-xy i this bulk@ 1 + shapes-xyz i sxy-max + this bulk! loop
    sxy-max 0 do shapes-xy i this bulk@ 2 + shapes-xyz i sxy-max 2 * + this bulk! loop
    sxy-max 0 do shapes-xy i this bulk@ 3 + shapes-xyz i sxy-max 3 * + this bulk! loop
  ;m method createxyztrans
  m: ( piece -- )
    sxyz-max 0 do shapes-xyz i this bulk@ all-orient i this bulk! loop
    sxyz-max 0 do shapes-xyz i this bulk@ rot swap all-orient i sxyz-max + this bulk! loop
    sxyz-max 0 do shapes-xyz i this bulk@ rot swap -rot all-orient i sxyz-max 2 * + this bulk! loop
    sxyz-max 0 do shapes-xyz i this bulk@ swap all-orient i sxyz-max 3 * + this bulk! loop
    sxyz-max 0 do shapes-xyz i this bulk@ rot all-orient i sxyz-max 4 * + this bulk! loop
    sxyz-max 0 do shapes-xyz i this bulk@ -rot all-orient i sxyz-max 5 * + this bulk! loop
  ;m method all6rotations
  m: ( nx1 ny1 nz1 nx2 ny2 nz2 piece -- nflag ) \ compare nx1 ny1 nz1 to nx2 ny2 nz2
    >r >r >r rot r> = rot r> = rot r> = and and
  ;m method test-voxel
  m: ( nx ny nz nindex piece -- nflag ) \ compare nx ny nz voxel to nindex piece all voxels
  \ nflag is false if no collisions
  \ nflag is true for any collision
    { nx ny nz nindex }
    try
      nx ny nz all-orient a nindex this bshape@ this test-voxel throw
      nx ny nz all-orient b nindex this bshape@ this test-voxel throw
      nx ny nz all-orient c nindex this bshape@ this test-voxel throw
      nx ny nz all-orient d nindex this bshape@ this test-voxel throw
      nx ny nz all-orient e nindex this bshape@ this test-voxel throw
      false
    restore
    endtry
  ;m method test-voxeltovoxels
  m: ( nindex1 nindex2 piece -- nflag ) \ compare one piece for collision with another piece
  \ nflag is false if no collisions
  \ nflag is true for any collision
    { nindex1 nindex2 }
    try
      all-orient a nindex1 this bshape@ nindex2 this test-voxeltovoxels throw
      all-orient b nindex1 this bshape@ nindex2 this test-voxeltovoxels throw
      all-orient c nindex1 this bshape@ nindex2 this test-voxeltovoxels throw
      all-orient d nindex1 this bshape@ nindex2 this test-voxeltovoxels throw
      all-orient e nindex1 this bshape@ nindex2 this test-voxeltovoxels throw
      false
    restore
    endtry
  ;m method test-collision
  m: ( nx ny nz piece -- ) \ just displays x y z from stack
    rot ." x:" . swap ."  y:" . ."  z:" .
  ;m method xyz.
  public
  m: ( piece -- )
    piece-table-created @ false = if \ to create piece table only once for all piece objects
      0 0 0 base-shapes a 0 this bshape! \ first shape
      1 0 0 base-shapes b 0 this bshape!
      2 0 0 base-shapes c 0 this bshape!
      2 0 1 base-shapes d 0 this bshape!
      3 0 1 base-shapes e 0 this bshape!
      0 0 1 base-shapes a 1 this bshape! \ second shape
      1 0 1 base-shapes b 1 this bshape!
      2 0 1 base-shapes c 1 this bshape!
      2 0 0 base-shapes d 1 this bshape!
      3 0 0 base-shapes e 1 this bshape!
      0 0 0 base-shapes a 2 this bshape! \ third shape
      1 0 0 base-shapes b 2 this bshape!
      1 0 1 base-shapes c 2 this bshape!
      2 0 1 base-shapes d 2 this bshape!
      3 0 1 base-shapes e 2 this bshape!
      0 0 1 base-shapes a 3 this bshape! \ fourth shape
      1 0 1 base-shapes b 3 this bshape!
      1 0 0 base-shapes c 3 this bshape!
      2 0 0 base-shapes d 3 this bshape!
      3 0 0 base-shapes e 3 this bshape!
      this creatextrans
      this createxytrans
      this createxyztrans
      this all6rotations
      \ at this moment the piece data base is populated
      true piece-table-created !
      nopiece [to-inst] thispiece#  \ start with no piece
      0 [to-inst] lastcollisionlist-a \ start with no collision list
      0 [to-inst] nextlistretrieve \ start at zero in the current linked list
    then
  ;m overrides construct

  m: ( piece -- )
    base-shapes e 3 this bshape@ . . . cr
    base-shapes d 2 this bshape@ . . . cr
    ." XXXXXXXX" cr
    bshape-max  0 do base-shapes i this bulk@ this xyz. ."  #" i . cr loop
    ." ********" cr
    sx-max 0 do shapes-x i this bulk@ this xyz. ."  #" i . cr loop
    ." yyyyyyyyy" cr
    sxy-max 0 do shapes-xy i this bulk@ this xyz. ."  #" i . cr loop
    ." zzzzzzzzz" cr
    sxyz-max 0 do shapes-xyz i this bulk@ this xyz. ."  #" i . cr loop
    ." all------" cr
    allorient-max 0 do all-orient i this bulk@ this xyz. ."  #" i . cr loop
    pindex-max 0 do
      all-orient a i this bshape@ rot ." a:" . swap . .
      all-orient b i this bshape@ rot ." b:" . swap . .
      all-orient c i this bshape@ rot ." c:" . swap . .
      all-orient d i this bshape@ rot ." d:" . swap . .
      all-orient e i this bshape@ rot ." e:" . swap . . ." #" i . cr
    loop
  ;m method testing
  m: ( piece -- )
    1 2 3 1 2 3 this test-voxel . ."  <- should be true!" cr
    1 2 3 1 2 5 this test-voxel . ."  <- should be false!" cr
    4 0 0 4 0 0 this test-voxel . ."  <- should be true!" cr
    0 0 0 0 0 0 this test-voxel . ."  <- should be true!" cr
    4 4 4 4 4 4 this test-voxel . ."  <- should be true!" cr
    4 0 4 4 4 0 this test-voxel . ."  <- should be false!" cr
    0 0 this test-collision . ."  <- collision should be true!" cr
    0 1 this test-collision . ."  <- collision should be true!" cr
    0 25 this test-collision . ."  <- collision should be false!" cr
    all-orient a 0 this bshape@ this xyz. ."  :a 0" cr
    all-orient b 0 this bshape@ this xyz. ."  :b 0" cr
    all-orient c 0 this bshape@ this xyz. ."  :c 0" cr
    all-orient d 0 this bshape@ this xyz. ."  :d 0" cr
    all-orient e 0 this bshape@ this xyz. ."  :e 0" cr
    all-orient a 25 this bshape@ this xyz. ."  :a 25" cr
    all-orient b 25 this bshape@ this xyz. ."  :b 25" cr
    all-orient c 25 this bshape@ this xyz. ."  :c 25" cr
    all-orient d 25 this bshape@ this xyz. ."  :d 25" cr
    all-orient e 25 this bshape@ this xyz. ."  :e 25" cr
    0 0 0 0 this test-voxeltovoxels . ."  <- collision should be true!" cr
    3 0 1 0 this test-voxeltovoxels . ."  <- collision should be true!" cr
    3 0 2 0 this test-voxeltovoxels . ."  <- collision should be false!" cr
  ;m method testcompare
  m: ( piece --)
    5 this collist!
    0 this collist!
    100 this collist!
    759 this collist!
    .s cr
    begin this collist@ swap . cr true <> until
    begin this collist@ swap . cr true <> until
  ;m method testcollist
  m: (  piece -- ) \ store piece collision # in lastcollisionlist-a
     5 pcll% %size allocate throw .s cr \ make room
    dup lastcollisionlist-a swap nextcollisionlist ! .s cr \ store last list address in this list
    dup -rot pcollision# ! .s cr \ store this npcollision# in this list
    [to-inst] lastcollisionlist-a .s cr \ update the last pointer to this list address
  ;m method testcol!
end-class piece

piece heap-new constant ptest
\ ptest testcol!
 ptest testcollist
\ ptest testing
\ ptest testcompare
