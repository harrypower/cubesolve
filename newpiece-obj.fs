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
    { nx ny nz naddr nindex }
    nx naddr x loc% %size nindex * + !
    ny naddr y loc% %size nindex * + !
    nz naddr z loc% %size nindex * + !
  ;m method bulk!
  m: ( naddr nindex piece -- nx ny nz )
    loc% %size * + dup dup
    x @ -rot y @ swap z @
  ;m method bulk@
  m: ( nx ny nz nbase-shapes-addr nindex piece -- ) \ to store basic-shape data array
    { nx ny nz nbsa nindex }
    nx nbsa x nindex blc% %size * + !
    ny nbsa y nindex blc% %size * + !
    nz nbsa z nindex blc% %size * + !
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

  m: ( piece -- )
     base-shapes e 3 this bshape@ . . . cr
     base-shapes d 2 this bshape@ . . . cr
     ." XXXXXXXX" cr
     bshape-max @  0 do base-shapes i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
     ." ********" cr
     sx-max @ 0 do shapes-x i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
     ." yyyyyyyyy" cr
     sxy-max @ 0 do shapes-xy i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
     ." zzzzzzzzz" cr
     sxyz-max @ 0 do shapes-xyz i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
     ." all------" cr
     allorient-max @ 0 do all-orient i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
    pindex-max @ 0 do
      all-orient a i this bshape@ rot ." a:" . swap . .
      all-orient b i this bshape@ rot ." b:" . swap . .
      all-orient c i this bshape@ rot ." c:" . swap . .
      all-orient d i this bshape@ rot ." d:" . swap . .
      all-orient e i this bshape@ rot ." e:" . swap . . ." #" i . cr
    loop
  ;m method testing
end-class piece

piece class
  struct
    cell% field pieces
  end-struct thepieces%
  25 variable parray-max parray-max !
    create theboard
  theboard thepieces% %size parray-max @ * dup allot erase \ array of 25 board pieces
  protected
  m: ( npiece nindex board -- )
    thepieces% %size * theboard + !
  ;m method piece!
  m: ( nindex board -- npiece )
    thepieces% %size * theboard + @
  ;m method piece@
  public
  m: ( piece -- )
    this [parent] construct
  ;m overrides construct
  m: ( piece -- )
    499 3 this piece!
    cr 3 this piece@ . ." this should be 499!" cr
    parray-max @ 0 do i this piece@ . ."  #" i . cr loop
  ;m method testing2
end-class board

board heap-new constant btest
btest testing
btest testing2
