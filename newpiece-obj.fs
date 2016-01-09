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

  20 variable bshape-max bshape-max !
  40 variable sx-max sx-max !
  200 variable sxy-max sxy-max !
  800 variable sxyz-max sxyz-max !
  4800 variable  allorient-max allorient-max !
  960 variable pindex-max pindex-max !

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
  ;m method bshape!
  m: ( nbase-shapes-addr nindex piece -- nx ny nz ) \ get basic-shape x y z data
    blc% %size * + dup dup
    x @ -rot y @ swap z @
  ;m method bshape@
  m: ( piece -- )
    bshape-max @ 0 do base-shapes i this bulk@ shapes-x i this bulk! loop
    bshape-max @ 0 do base-shapes i this bulk@ rot 1 + -rot shapes-x i bshape-max @ + this bulk! loop
  ;m method creatextrans
  m: ( piece -- )
    sx-max @ 0 do shapes-x i this bulk@ shapes-xy i this bulk! loop
    sx-max @ 0 do shapes-x i this bulk@ swap 1 + swap shapes-xy i sx-max @ + this bulk! loop
    sx-max @ 0 do shapes-x i this bulk@ swap 2 + swap shapes-xy i sx-max @ 2 * + this bulk! loop
    sx-max @ 0 do shapes-x i this bulk@ swap 3 + swap shapes-xy i sx-max @ 3 * + this bulk! loop
    sx-max @ 0 do shapes-x i this bulk@ swap 4 + swap shapes-xy i sx-max @ 4 * + this bulk! loop
  ;m method createxytrans
  m: ( piece -- )
    sxy-max @ 0 do shapes-xy i this bulk@ shapes-xyz i this bulk! loop
    sxy-max @ 0 do shapes-xy i this bulk@ 1 + shapes-xyz i sxy-max @ + this bulk! loop
    sxy-max @ 0 do shapes-xy i this bulk@ 2 + shapes-xyz i sxy-max @ 2 * + this bulk! loop
    sxy-max @ 0 do shapes-xy i this bulk@ 3 + shapes-xyz i sxy-max @ 3 * + this bulk! loop
  ;m method createxyztrans
  m: ( piece -- )
    sxyz-max @ 0 do shapes-xyz i this bulk@ all-orient i this bulk! loop
    sxyz-max @ 0 do shapes-xyz i this bulk@ rot swap all-orient i sxyz-max @ + this bulk! loop
    sxyz-max @ 0 do shapes-xyz i this bulk@ rot swap -rot all-orient i sxyz-max @ 2 * + this bulk! loop
    sxyz-max @ 0 do shapes-xyz i this bulk@ swap all-orient i sxyz-max @ 3 * + this bulk! loop
    sxyz-max @ 0 do shapes-xyz i this bulk@ rot all-orient i sxyz-max @ 4 * + this bulk! loop
    sxyz-max @ 0 do shapes-xyz i this bulk@ -rot all-orient i sxyz-max @ 5 * + this bulk! loop
  ;m method all6rotations
  m: ( nx1 ny1 nz1 nx2 ny2 nz2 piece -- nflag ) \ compare nx1 ny1 nz1 to nx2 ny2 nz2
    >r >r >r rot r> = rot r> = rot r> = and and
  ;m method test-voxel
  m: ( nx ny nz nindex piece -- nflag ) \ compare nx ny nz voxel to nindex piece all voxels
    { nx ny nz nindex }
    nx ny nz all-orient a nindex this bshape@ this test-voxel
    nx ny nz all-orient b nindex this bshape@ this test-voxel
    nx ny nz all-orient c nindex this bshape@ this test-voxel
    nx ny nz all-orient d nindex this bshape@ this test-voxel
    nx ny nz all-orient e nindex this bshape@ this test-voxel
    or or or or
  ;m method test-voxeltovoxels
  m: ( nindex1 nindex2 piece -- nflag ) \ compare one piece for collision with another piece
    { nindex1 nindex2 }
    all-orient a nindex1 this bshape@ nindex2 this test-voxeltovoxels
    all-orient b nindex1 this bshape@ nindex2 this test-voxeltovoxels
    all-orient c nindex1 this bshape@ nindex2 this test-voxeltovoxels
    all-orient d nindex1 this bshape@ nindex2 this test-voxeltovoxels
    all-orient e nindex1 this bshape@ nindex2 this test-voxeltovoxels
    or or or or
  ;m method test-collision
  public
  m: ( piece -- )
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
  ;m overrides construct

  m: ( nx ny nz piece -- )
    rot ." x:" . swap ."  y:" . ."  z:" .
  ;m method xyz.
  m: ( piece -- )
    base-shapes e 3 this bshape@ . . . cr
    base-shapes d 2 this bshape@ . . . cr
    ." XXXXXXXX" cr
    bshape-max @  0 do base-shapes i this bulk@ this xyz. ."  #" i . cr loop
    ." ********" cr
    sx-max @ 0 do shapes-x i this bulk@ this xyz. ."  #" i . cr loop
    ." yyyyyyyyy" cr
    sxy-max @ 0 do shapes-xy i this bulk@ this xyz. ."  #" i . cr loop
    ." zzzzzzzzz" cr
    sxyz-max @ 0 do shapes-xyz i this bulk@ this xyz. ."  #" i . cr loop
    ." all------" cr
    allorient-max @ 0 do all-orient i this bulk@ this xyz. ."  #" i . cr loop
    pindex-max @ 0 do
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
end-class piece

piece class
  struct
    cell% field pieces
  end-struct thepieces%
  0 variable current-solution-piece current-solution-piece !
  25 variable parray-max parray-max ! \ total pieces in static array
  1000 variable nopiece nopiece ! \ the value that means no piece is present
  create theboard \ note this is a static board used in all board objects
  theboard thepieces% %size parray-max @ * dup allot erase \ array of 25 board pieces
  protected
  m: ( npiece nindex board -- )
    thepieces% %size * theboard + !
  ;m method piece!
  m: ( nindex board -- npiece )
    thepieces% %size * theboard + @
  ;m method piece@
  public
  m: ( board -- )
    parray-max @ 0 do nopiece @ i this piece! loop \ put no piece in array at start
  ;m method emptyboard
  m: ( board -- )
    this [parent] construct
    this emptyboard
  ;m overrides construct
  m: ( board -- )
    499 3 this piece!
    cr 3 this piece@ . ." this should be 499!" cr
    parray-max @ 0 do i this piece@ . ."  #" i . cr loop
  ;m method testing2
  m: ( ntestpiece board -- nflag ) \ test ntestpiece with all pieces currently in solution for collision
    \ nflag is false for no collision
    \ nflag is true for a collision
    false { ntestpiece ntc }
    current-solution-piece @ 0 ?do ntestpiece i this piece@ this test-collision ntc or to ntc loop ntc
  ;m method testpiece
  m: ( nstart board -- nsolution nflag ) \ test all piece combination placements with current pieces in solution for collision
    \ nflag is false if solution is found
    \ nflag is true if no solutions is found
    \ nsolution is the last pindex solution value tested
    true true rot pindex-max @ swap do 2drop i this testpiece if i true else i false leave then loop
  ;m method findpiece
  m: ( board -- )
    this emptyboard
    0 current-solution-piece !
    0 \ start the search from the begining of total pieces
    begin
      this findpiece
      if
        .s ." no more solutions! " cr parray-max @ current-solution-piece !
      else
        \ store this found solution
        current-solution-piece @ this piece! current-solution-piece @ 1 + current-solution-piece !
        0 \ start a new search from the start of total pieces
      then
      current-solution-piece @ . cr
      current-solution-piece @ parray-max @ >= \ if true then solution reached if false continue
    until
    ." yay found the solution"
  ;m method solvepuzzle
  m: ( board -- )
    this emptyboard
    0 current-solution-piece !
    0 0 this piece!
    cr ." testpiece " 0 this testpiece . cr
    ." findpiece " 0 this findpiece . . .s cr
    1 current-solution-piece !
    cr ." testpiece " 0 this testpiece . cr
    ." findpiece " 0 this findpiece . . .s cr
    cr ." testpiece " 8 this testpiece . cr
    ." findpiece " 8 this findpiece . . .s cr
    8 1 this piece!
    2 current-solution-piece !
    cr ." testpiece " 8 this testpiece . cr
    ." findpiece " 8 this findpiece . . .s cr
  ;m method test4
end-class board
\ note piece object is a parent to board object
\ board object and piece object have no instance variables
\ this means they are to be instantated once and used only this way so they are not normal objects because of this.

board heap-new constant btest
\ btest testing
\ btest testing2
\ btest testcompare
\ btest solvepuzzle
\ btest findpiece . .
\ btest test4
