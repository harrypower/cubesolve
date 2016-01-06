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
  cell% inst-var bshape-max
  cell% inst-var sx-max
  cell% inst-var sxy-max
  cell% inst-var sxyz-max
  cell% inst-var allorient-max

  protected
  m: ( nx ny nz naddr nindex piece -- )
    { nx ny nz naddr nindex }
    nx naddr x loc% %size nindex * + !
    ny naddr y loc% %size nindex * + !
    nz naddr z loc% %size nindex * + !
  ;m method bulk!
  m: ( naddr nindex piece -- nx ny nz )
    { naddr nindex }
    naddr x loc% %size nindex * + @
    naddr y loc% %size nindex * + @
    naddr z loc% %size nindex * + @
  ;m method bulk@
  m: ( nx ny nz nbase-shapes-addr nindex piece -- ) \ to store basic-shape data array
    { nx ny nz nbsa nindex }
    nx nbsa x nindex blc% %size * + !
    ny nbsa y nindex blc% %size * + !
    nz nbsa z nindex blc% %size * + !
  ;m method bshape!
  m: ( nbase-shapes-addr nindex piece -- nx ny nz ) \ get basic-shape x y z data
    { nbsa nindex }
    nbsa x nindex blc% %size * + @
    nbsa y nindex blc% %size * + @
    nbsa z nindex blc% %size * + @
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
    20 bshape-max !
    40 sx-max !
    200 sxy-max !
    800 sxyz-max !
    4800 allorient-max !
    this creatextrans
    this createxytrans
  ;m overrides construct
  m: ( piece -- )
    base-shapes e 3 this bshape@ . . . cr
    base-shapes d 2 this bshape@ . . . cr
    ." XXXXXXXX" cr
    bshape-max @ 0 ?do base-shapes i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
    ." ********" cr
    sx-max @ 0 ?do shapes-x i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
    ." yyyyyyyyy" cr
    sxy-max @ 0 ?do shapes-xy i this bulk@ rot ." x:" . swap ."  y:" . ."  z:" . ."  #" i . cr loop
  ;m method testing
end-class piece

piece class

  m: ( piece -- )
    this [parent] construct
  ;m overrides construct
end-class board

piece heap-new constant ptest
ptest testing

board heap-new constant btest
btest testing
