require objects.fs

interface
    selector destruct ( -- ) \ to free allocated memory in objects that use this
end-interface destruction

object class
  destruction implementation
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
  inst-value collist-a  \ the address to start of collision list
  inst-value collist-f  \ a flag if true then collision list is valid false means list not calculated yet

  struct
    char% field piece-flag
  end-struct collist%

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
  m: ( piece -- ) \ populate the collision  list for thispiece#
    0 collist-a <> thispiece# nopiece <> collist-f false = and and
    if
      pindex-max 0 do
        thispiece# i this test-collision
        collist-a piece-flag i collist% %size * + c!
      loop
      true [to-inst] collist-f
    then
  ;m method makecollisionlist
  m: ( piece -- ) \ allocate room for the collision list or clear list if allocated already
    0 collist-a <>
    if
      collist-a collist% %size pindex-max * erase
    else
      collist% %size pindex-max * allocate throw [to-inst] collist-a
      collist-a collist% %size pindex-max * erase
    then
    false [to-inst] collist-f
  ;m method create-collist
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
    then
    0 [to-inst] collist-a \ at construct time the collsion list is not allocated yet
    this create-collist
    nopiece [to-inst] thispiece#  \ start with no piece
  ;m overrides construct
  m: ( piece -- ) \ free allocated memory for this piece
    0 collist-a <> if
      collist-a free throw
    then
    0 [to-inst] collist-a
    false [to-inst] collist-f
    nopiece [to-inst] thispiece#
  ;m overrides destruct
  m: ( npiece# piece -- ) \ set the piece# and create the collision list
    dup dup 0 >= -rot pindex-max < rot and if [to-inst] thispiece# else drop nopiece [to-inst] thispiece# then
  ;m method piece!
  m: ( piece -- npiece# ) \ get the current piece#
    thispiece#
  ;m method piece@
  m: ( piece -- ) \ create the collision list for this piece
    this makecollisionlist
  ;m method collisionlist!
  m: ( npiece# piece -- nflag ) \ test the npiece# collistion value from collision list
  \ nflag is true if npiece# has collided with thispiece# from the collision list
  \ nflag is false if npiece# has not collided with thispiece# in the collision list or the collision list does not exist
    collist-f true =
    if
      collist-a swap collist% %size * + c@
      if true else false then
    else
      drop false
    then
  ;m method collisionlist?
  m: ( piece -- ) \ testing basic data set creation
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
  m: ( piece -- ) \ esting collision detection words
    cr
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
  m: ( piece -- ) \ testing collision detection list processes of this object
    this destruct
    this construct
    500 this piece!
    this collisionlist!
    cr
    500 this collisionlist? . ." < this should be true!" cr
    501 this collisionlist? . ." < this should be true!" cr
    510 this collisionlist? . ." < this should be false!" cr
    520 this collisionlist? . ." < this should be false!" cr
    540 this collisionlist? . ." < this should be true!" cr

    this destruct
    this construct
    0 this piece!
    this collisionlist!
    cr
    0 this collisionlist? . ." < this should be true!" cr
    3 this collisionlist? . ." < this should be true!" cr
    10 this collisionlist? . ." < this should be false!" cr
  ;m method testcollistionlist
  m: ( piece -- ) \ test collision list full
    cr
    this destruct
    this construct
    959 this piece!
    this collisionlist!
    pindex-max 0 do i this collisionlist? i . . cr loop
  ;m method testcollsionlistfull
end-class piece

\ piece heap-new constant ptest
\ ptest testcompare
\ ." .........." cr
\ ptest testing
\ ." .........." cr
\ ptest testcollistionlist
\ ptest testcollsionlistfull

object class
 destruction implementation
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
 inst-value current-solution-index \ will be used in solution to index through the bpieces
 25 constant bpieces
 960 constant piece-max
 false variable boardconstruct boardconstruct ! \ test if the board has been constructed once
 inst-value view#
 inst-value nowlow#
 inst-value oneshot

protected
m: ( npieceaddr nindex board -- ) \ store collision list piece
  apiece% %size * allpiecesarray pieceaddr + !
;m method collisionpiece!
m: ( nindex board -- npieceaddr ) \ retrieve collision list piece
  apiece% %size * allpiecesarray pieceaddr + @
;m method collisionpiece@
m: ( npieceaddr nindex board -- ) \ store a piece on the board
  aboardpiece% %size * boardpiecearray pieceindex + !
;m method ponboard!
m: ( nindex board -- npieceaddr ) \ retreave a piece on the board
  aboardpiece% %size * boardpiecearray pieceindex + @
;m method ponboard@
m: ( ntestpiece board -- nflag ) \ test ntestpiece with all pieces currently in solution for collision
  \ nflag is false for no collision
  \ nflag is true for a collision
  { ntestpiece }
  try
    current-solution-index 0
    ?do
      ntestpiece i this ponboard@ this collisionpiece@ collisionlist?
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
    2drop i this testallpieces if i true else i false leave then
  loop
;m method findpiece
m: ( n board -- n1 )
  dup 0 < if drop 0 then
;m method zerotest
m:  ( n board -- n2 )
  dup piece-max >= if drop 0 current-solution-index 1 - this zerotest [to-inst] current-solution-index then
;m method pmaxtest
public
m: ( board -- ) \ free allocated memory for the board pieces
  boardtest @ boardtest = boardpiecearray 0 <> and
  if
    boardpiecearray free throw
    0 [to-inst] boardpiecearray
  then
;m overrides destruct
m: ( board -- )
  boardconstruct @ false =
  if
   piece-max 0 do
     piece heap-new
     dup i this collisionpiece!
     dup i swap piece!
     collisionlist!
   loop
   true boardconstruct ! \ board has been constructed so shared data is now setup
  then
  boardtest @ boardtest = if this destruct then \ deallocate past board to allow new board to be constructed
  aboardpiece% %size bpieces * allocate throw  [to-inst] boardpiecearray \ make dynamic board pieces array
  boardpiecearray aboardpiece% %size bpieces * true fill \ start board empty
  boardtest boardtest ! \ set up construct test now that stuff has been allocated
  0 [to-inst] current-solution-index \ start with no solution started
  0 [to-inst] view#
  25 [to-inst] nowlow#
  false [to-inst] oneshot
;m overrides construct
m: ( npiece# nboard# board -- )
  swap dup rot rot dup 0 >= swap piece-max < and
  if this ponboard! else 2drop then
;m method board!
m: ( nboard# board -- )
  this ponboard@
;m method board@
m: ( board -- )
  this destruct
  this construct
;m method clearboard
m: ( board -- )
  this clearboard
  0 [to-inst] current-solution-index
  0 \ start at beginning
  begin
    this findpiece \ .s cr
    if \ here because no solution so must back trace once
      \ .s ." no solution place" cr
      \ current-solution-index . ." current index!" cr
      drop \ throw away bad solution
      current-solution-index 1 - this zerotest [to-inst] current-solution-index \ step back current solution pointer
      \ current-solution-index . ." next index!" cr
      current-solution-index this ponboard@ \ .s ." should be 0 to 959" cr
      this collisionpiece@ \ .s ." should be some address" cr
      piece@ \ .s ." should be 0 to 959" cr
      1 + this pmaxtest \ get last solved piece and go past that solution
      \ .s ." next testable solution!" cr
    else \ found solution store it and step forward
      current-solution-index this ponboard! 
      current-solution-index 1 + [to-inst] current-solution-index
      0 \ start a new search from the start of total pieces
    then
    \ current-solution-index 1 - dup . this ponboard@ . cr
    oneshot true = if
      current-solution-index nowlow# < if current-solution-index [to-inst] nowlow# then
    then
    view# 1 + [to-inst] view#
    view# 1000 > if
      page 10 10 at-xy
      oneshot false = if true [to-inst] oneshot then
      current-solution-index 1 - dup . this ponboard@ .
      nowlow# ."  low:" .  nowlow# this ponboard@ . cr
      0 [to-inst] view#
    then
    current-solution-index piece-max >= \ if true then solution reached if false continue
  until
;m method solveit
m: ( board -- )
  cr
  piece-max 0
  do
    i this collisionpiece@ piece@ . \ just display the collision list piece value
    i i this collisionpiece@ collisionlist? . cr \ this should be true all the time because a piece will collide with itself!
  loop
;m method testpiececollarray
m: ( board -- )
  this construct cr
  10 this testallpieces . ." <- this should be false!" cr
  1 [to-inst] current-solution-index
  0 0 this board!
  0 this testallpieces . ." <- this should be true!" cr
  10 this testallpieces . ." <- this should be false!" cr
  this clearboard cr
  0 this findpiece . . ." <- this should be false 0!" cr
  0 0 this board!
  1 [to-inst] current-solution-index
  0 this findpiece . . ." <- this should be false 8!" cr
  8 1 this board!
  2 [to-inst] current-solution-index
  0 this findpiece . . ." <- this should be false 16!" cr
  this clearboard cr
;m method testingsolutionwords
m: ( board -- ) \ print out list of pieces for each board location
  cr
  bpieces 0 do i this board@ . ." :" i . cr loop
;m method seeboardpieces
m: ( ncolltest ncolindex board -- )
  cr
  dup this collisionpiece@ piece@ .
  this collisionpiece@ collisionlist? . cr
;m method seeacollision
end-class board

 board heap-new constant btest
 btest solveit
 btest seeboardpieces
( btest testingsolutionwords
 7 0 btest seeacollision
 8 0 btest seeacollision
 15 8 btest seeacollision
 16 8 btest seeacollision )

 ( 5 0 btest board!
 20 15 btest board!
 -20 6 btest board!
 1000 8 btest board!
 btest seeboardpieces )
 ( btest testpiececollarray
 btest seeboardpieces
 btest destruct
 btest construct
 btest testpiececollarray
 btest seeboardpieces
 btest destruct
 btest construct )
