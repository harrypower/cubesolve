require objects.fs

: #to$ ( n -- c-addr u1 ) \ convert n to string
    s>d
    swap over dabs
    <<# #s rot sign #> #>> ;

interface
    selector destruct ( -- ) \ to free allocated memory in objects that use this
end-interface destruction

object class
  destruction implementation
  protected
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
  m: ( nx ny nz piece -- ) \ just displays x y z from stack
    rot ." x:" . swap ."  y:" . ."  z:" .
  ;m method xyz.
  public
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
    this [current] create-collisionlist
    nopiece [to-inst] thispiece#  \ start with no piece
  ;m overrides construct
  m: ( piece -- ) \ free allocated memory for this piece
    0 collisionlist-addr <> if
      collisionlist-addr free throw
    then
    0 [to-inst] collisionlist-addr
    false [to-inst] collisionlist-flag
    nopiece [to-inst] thispiece#
  ;m overrides destruct
  m: ( npiece# piece -- ) \ set the piece# and create the collision list
    dup dup 0 >= -rot pindex-max < rot and if [to-inst] thispiece# else drop nopiece [to-inst] thispiece# then
  ;m method piece!
  m: ( piece -- npiece# ) \ get the current piece#
    thispiece#
  ;m method piece@
  m: ( piece -- ) \ create the collision list for this piece
    this [current] populatecollisionlist
  ;m method collisionlist!
  m: ( npiece# piece -- nflag ) \ test the npiece# collistion value from collision list
  \ nflag is true if npiece# has collided with thispiece# from the collision list
  \ nflag is false if npiece# has not collided with thispiece# in the collision list or the collision list does not exist
    collisionlist-flag true =
    if
      collisionlist-addr swap collisionlist% %size * + c@
      if true else false then
    else
      drop false
    then
  ;m method collisionlist?
  m: ( nsubpiece# npiece# piece -- nx ny nz ) \ will return sub block xyz values for a given npiece# and a given nsubpiece#
    \ nsubpiece# is 0 to 4
    \ npiece# is 0 to pindex-max -1
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

object class
  destruction implementation
  protected
  4 constant displaycellsize
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
    displaydata-addr thepiece% %size xyz-size * xyz-size * xyz-size * true fill \ place no pices in display data
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
          dup true = if drop 99 then  \ if no piece then show 99
          k displaycellsize * \ x for at-xy
          xyz-size zplane-spacing + i * j + topoffset + \ y for at-xy
          at-xy
          ." :" #to$ type
        loop
      loop
    loop
  ;m method showdisplay
end-class displaypieces

object class
  destruction implementation
  protected
  struct
   cell% field pieceaddr
  end-struct apiece%
  create allpiecesarray
  allpiecesarray apiece% %size 960 * dup allot erase
  struct
   cell% field pieceindex
  end-struct aboardpiece%
  inst-value boardpiecearray
  cell% inst-var boardtest \ test if construct has been run for this instance
  inst-value current-solution-index \ will be used in solution to index through the board pieces
  25 constant boardpieces
  960 constant piece-max
  false variable boardconstruct boardconstruct ! \ test if the board has been constructed once
  inst-value view#
  inst-value nowlow#
  inst-value nowhigh#
  inst-value oneshot
  inst-value thedisplay

  protected
  m: ( npieceaddr nindex board -- ) \ store collision list piece
    apiece% %size * allpiecesarray pieceaddr + !
  ;m method collisionpiece!
  m: ( nindex board -- npieceaddr ) \ retrieve collision list piece
    apiece% %size * allpiecesarray pieceaddr + @
  ;m method collisionpiece@
  m: ( npieceaddr nindex board -- ) \ store a piece on the board
    aboardpiece% %size * boardpiecearray pieceindex + !
  ;m method pieceonboard!
  m: ( nindex board -- npieceaddr ) \ retreave a piece on the board
    aboardpiece% %size * boardpiecearray pieceindex + @
  ;m method pieceonboard@
  m: ( ntestpiece board -- nflag ) \ test ntestpiece with all pieces currently in solution for collision
    \ nflag is false for no collision
    \ nflag is true for a collision
    { ntestpiece }
    try
      current-solution-index 0
      ?do
        ntestpiece i this [current] pieceonboard@  this [current] collisionpiece@ [bind] piece collisionlist?
        throw \ if a collision then return true
      loop
      false \ if no collision then return false
    restore
    endtry
  ;m method testallpieces
  m: ( nstart -- nsolution nflag ) \ nstart is index to start testing for collisions with current solution
    \ nflag is false if solution is found
    \ nflag is true if no solutions is found
    \ nsolution is the last pindex solution value tested
    true true rot
    piece-max swap
    do
      2drop i this [current] testallpieces if i true else i false leave then
    loop
  ;m method findpiece
    m: ( n board -- n1 )
    dup 0 < if drop 0 then
  ;m method zerotest
  m:  ( n board -- n2 )
    dup piece-max >= if drop 0 current-solution-index 1 - this [current] zerotest [to-inst] current-solution-index then
  ;m method piecemaxtest
  m: ( nstart ncurrentlevel board -- nnextindex )
    [to-inst] current-solution-index
    begin
      this [current] findpiece \ .s cr
      if \ here because no solution so must back trace once
        drop \ throw away bad solution
        current-solution-index 1 - this [current] zerotest [to-inst] current-solution-index \ step back current solution pointer
        current-solution-index this [current] pieceonboard@  \ .s ." should be 0 to 959" cr
        this [current] collisionpiece@ \ .s ." should be some address" cr
        [bind] piece piece@ \ .s ." should be 0 to 959" cr
        1 + this [current] piecemaxtest  \ get last solved piece and go past that solution
      else \ found solution store it and step forward
        current-solution-index this [current] pieceonboard!
        current-solution-index 1 + [to-inst] current-solution-index
        0 \ start a new search from the start of total pieces
      then
      oneshot true = if
        current-solution-index nowlow# < if current-solution-index [to-inst] nowlow# then
        current-solution-index nowhigh# > if current-solution-index 1 - [to-inst] nowhigh# then
        nowhigh# 0 < if 0 [to-inst] nowhigh# then
      then
      view# 1 + [to-inst] view#
      view# 1000 > if
        page 1 1 at-xy
        oneshot false = if true [to-inst] oneshot then
        current-solution-index 1 - dup . this [current] pieceonboard@ .
        nowlow# ."  low:" . nowlow# this [current] pieceonboard@ .
        nowhigh# ." high:" . nowhigh# this [current] pieceonboard@ .
        0 [to-inst] view#
      then
      current-solution-index piece-max >= \ if true then solution reached if false continue
      key? or
    until
  ;m method solveit
  public
  m: ( board -- ) \ free allocated memory for the board pieces
    boardtest @ boardtest = boardpiecearray 0 <> and
    if
      boardpiecearray free throw
      0 [to-inst] boardpiecearray
    then
    boardtest @ boardtest =
    if
      0 boardtest !
      thedisplay [bind] displaypieces destruct
      0 [to-inst] thedisplay
    then
  ;m overrides destruct
  m: ( board -- )
    boardconstruct @ false =
    if
     piece-max 0 do
       piece heap-new
       dup i this [current] collisionpiece!
       dup i swap [bind] piece piece!
       [bind] piece collisionlist!
     loop
     true boardconstruct ! \ board has been constructed so shared data is now setup
    then
    boardtest @ boardtest = if this [current] destruct then \ deallocate past board to allow new board to be constructed
    aboardpiece% %size boardpieces  * allocate throw  [to-inst] boardpiecearray \ make dynamic board pieces array
    boardpiecearray aboardpiece% %size boardpieces  * true fill \ start board empty
    0 [to-inst] current-solution-index \ start with no solution started
    0 [to-inst] view#
    25 [to-inst] nowlow#
    0 [to-inst] nowhigh#
    false [to-inst] oneshot
    displaypieces heap-new [to-inst] thedisplay \ start and setup display of board
    boardtest boardtest ! \ set up construct test now that stuff has been allocated
  ;m overrides construct
  m: ( npiece# nboard# board -- )
    swap dup rot rot dup 0 >= swap piece-max < and
    if this [current] pieceonboard!  else 2drop then
  ;m method board!
  m: ( nboard# board -- )
    this [current] pieceonboard@
  ;m method board@
  m: ( board -- )
    this [current] destruct
    this [current] construct
  ;m method clearboard

  m: ( nstart ncurrentlevel board -- nnextindex ncurrentlevel )
    0 [to-inst] view#
    25 [to-inst] nowlow#
    0 [to-inst] nowhigh#
    false [to-inst] oneshot
    this [current] solveit
    current-solution-index
  ;m method solvecontinue
  m: ( board -- nnextindex ncurrentlevel )
    this [current] clearboard
    0 0 this [current] solveit current-solution-index
  ;m method solvestart
  m: ( board -- ) \ to view the current board solution
    \ populate the display with the current board
    0 0 { p c }
    boardpieces 0 do
      i this [current] pieceonboard@ dup to p true <>
      if
        p this [current] collisionpiece@ to c  \ have piece object now just get piece data with it and place in display
        0 p c [bind] piece subpiece@ i thedisplay displaypiece!
        1 p c [bind] piece subpiece@ i thedisplay displaypiece!
        2 p c [bind] piece subpiece@ i thedisplay displaypiece!
        3 p c [bind] piece subpiece@ i thedisplay displaypiece!
        4 p c [bind] piece subpiece@ i thedisplay displaypiece!
      then
    loop
    thedisplay showdisplay
  ;m method showboard
  m: ( npiece# board -- )
    0 0 { np# p c  }
    np# this [current] pieceonboard@ to p
    p true <>
    if
      p this [current] collisionpiece@ to c
      0 p c [bind] piece subpiece@ np# thedisplay displaypiece!
      1 p c [bind] piece subpiece@ np# thedisplay displaypiece!
      2 p c [bind] piece subpiece@ np# thedisplay displaypiece!
      3 p c [bind] piece subpiece@ np# thedisplay displaypiece!
      4 p c [bind] piece subpiece@ np# thedisplay displaypiece!
    then
    thedisplay showdisplay
  ;m method showapieceonboard
  m: ( board -- )
    thedisplay [bind] displaypieces destruct
    thedisplay [bind] displaypieces construct
  ;m method cleardisplay
  m: ( npiece# board -- )
    0 0 { np# p c }
    np# this [current] pieceonboard@ to p
    p true <>
    if
      cr
      p this [current] collisionpiece@ to c
      0 p c [bind] piece subpiece@ rot ." x:" . swap ."  y:" . ."  z:" . np# . ." a" cr
      1 p c [bind] piece subpiece@ rot ." x:" . swap ."  y:" . ."  z:" . np# . ." b" cr
      2 p c [bind] piece subpiece@ rot ." x:" . swap ."  y:" . ."  z:" . np# . ." c" cr
      3 p c [bind] piece subpiece@ rot ." x:" . swap ."  y:" . ."  z:" . np# . ." d" cr
      4 p c [bind] piece subpiece@ rot ." x:" . swap ."  y:" . ."  z:" . np# . ." e" cr
    then
  ;m method showpiecesubs
  m: ( board -- ) \ print out list of pieces for each board location
    cr boardpieces  0 do i this [current] board@ . ." :" i . cr loop
  ;m method seeboardpieces
  m: ( ncolltest ncollindex board -- ) \ will display the piece in collison piece list for ncollindex then display if it collides with ncolltest piece
  \ essentialy see if two pieces collide and does this test from the collision list data created in this board object
    cr dup this [current] collisionpiece@ [bind] piece piece@ .
    this [current] collisionpiece@ [bind] piece collisionlist? . cr
  ;m method seeacollision
  m: ( board -- )
    cr piece-max 0
    do
      i this [current] collisionpiece@ [bind] piece piece@ . \ just display the collision list piece value
      i i this [current] collisionpiece@ [bind] piece collisionlist? . cr \ this should be true all the time because a piece will collide with itself!
    loop
  ;m method testpiececollarray
  m: ( board -- )
    this [current] construct cr
    10 this [current] testallpieces . ." <- this should be false!" cr
    1 [to-inst] current-solution-index
    0 0 this [current] board!
    0 this [current] testallpieces . ." <- this should be true!" cr
    10 this [current] testallpieces . ." <- this should be false!" cr
    this [current] clearboard cr
    0 this [current] findpiece . . ." <- this should be false 0!" cr
    0 0 this [current] board!
    1 [to-inst] current-solution-index
    0 this [current] findpiece . . ." <- this should be false 8!" cr
    8 1 this [current] board!
    2 [to-inst] current-solution-index
    0 this [current] findpiece . . ." <- this should be false 16!" cr
    this [current] clearboard cr
  ;m method testingsolutionwords
end-class board

board heap-new constant btest
btest solvestart
btest seeboardpieces page
btest showboard
