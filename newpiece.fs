require objects.fs

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

: bulk! ( nx ny nz naddr nindex -- )
  loc% %size * + { naddr }
  naddr z ! naddr y ! naddr x ! ;
: bulk@ ( naddr nindex nx ny nz --)
  loc% %size * + dup dup
  x @ -rot y @ swap z @ ;
: bshape! ( nx ny nz nbase-shapes-addr nindex -- ) \ to store basic-shape data array
  blc% %size * + { nbsa }
  nbsa z ! nbsa y ! nbsa x ! ;
: bshape@ ( nbase-shapes-addr nindex nx ny nz -- ) \ get basic-shape x y z data
  blc% %size * + dup dup
  x @ -rot y @ swap z @ ;
: creatextrans ( -- )
  bshape-max @ 0 do base-shapes i bulk@ shapes-x i bulk! loop
  bshape-max @ 0 do base-shapes i bulk@ rot 1 + -rot shapes-x i bshape-max @ + bulk! loop ;
: createxytrans ( -- )
  sx-max @ 0 do shapes-x i bulk@ shapes-xy i bulk! loop
  sx-max @ 0 do shapes-x i bulk@ swap 1 + swap shapes-xy i sx-max @ + bulk! loop
  sx-max @ 0 do shapes-x i bulk@ swap 2 + swap shapes-xy i sx-max @ 2 * + bulk! loop
  sx-max @ 0 do shapes-x i bulk@ swap 3 + swap shapes-xy i sx-max @ 3 * + bulk! loop
  sx-max @ 0 do shapes-x i bulk@ swap 4 + swap shapes-xy i sx-max @ 4 * + bulk! loop ;
: createxyztrans ( -- )
  sxy-max @ 0 do shapes-xy i bulk@ shapes-xyz i bulk! loop
  sxy-max @ 0 do shapes-xy i bulk@ 1 + shapes-xyz i sxy-max @ + bulk! loop
  sxy-max @ 0 do shapes-xy i bulk@ 2 + shapes-xyz i sxy-max @ 2 * + bulk! loop
  sxy-max @ 0 do shapes-xy i bulk@ 3 + shapes-xyz i sxy-max @ 3 * + bulk! loop ;
: all6rotations ( -- )
  sxyz-max @ 0 do shapes-xyz i bulk@ all-orient i bulk! loop
  sxyz-max @ 0 do shapes-xyz i bulk@ rot swap all-orient i sxyz-max @ + bulk! loop
  sxyz-max @ 0 do shapes-xyz i bulk@ rot swap -rot all-orient i sxyz-max @ 2 * + bulk! loop
  sxyz-max @ 0 do shapes-xyz i bulk@ swap all-orient i sxyz-max @ 3 * + bulk! loop
  sxyz-max @ 0 do shapes-xyz i bulk@ rot all-orient i sxyz-max @ 4 * + bulk! loop
  sxyz-max @ 0 do shapes-xyz i bulk@ -rot all-orient i sxyz-max @ 5 * + bulk! loop ;
: test-voxel ( nx1 ny1 nz1 nx2 ny2 nz2 -- nflag ) \ compare nx1 ny1 nz1 to nx2 ny2 nz2
\ nflag is false if no collisions
\ nflag is true for any collision
  >r >r >r rot r> = rot r> = rot r> = and and ;
: test-voxeltovoxels ( nx ny nz nindex -- nflag ) \ compare nx ny nz voxel to nindex piece all voxels
\ nflag is false if no collisions
\ nflag is true for any collision
  { nx ny nz nindex }
  nx ny nz all-orient a nindex bshape@ test-voxel
  nx ny nz all-orient b nindex bshape@ test-voxel
  nx ny nz all-orient c nindex bshape@ test-voxel
  nx ny nz all-orient d nindex bshape@ test-voxel
  nx ny nz all-orient e nindex bshape@ test-voxel
  or or or or ;
: test-collision ( nindex1 nindex2 -- nflag ) \ compare one piece for collision with another piece
  \ nflag is false if no collisions
  \ nflag is true for any collision
  { nindex1 nindex2 }
  all-orient a nindex1 bshape@ nindex2 test-voxeltovoxels
  all-orient b nindex1 bshape@ nindex2 test-voxeltovoxels
  all-orient c nindex1 bshape@ nindex2 test-voxeltovoxels
  all-orient d nindex1 bshape@ nindex2 test-voxeltovoxels
  all-orient e nindex1 bshape@ nindex2 test-voxeltovoxels
  or or or or ;
: xyz. ( nx ny nz -- )
  rot ." x:" . swap ."  y:" . ."  z:" . ;

struct
  cell% field pieces
end-struct thepieces%
0 variable current-solution-piece current-solution-piece !
25 variable parray-max parray-max ! \ total pieces in static array
1000 variable nopiece nopiece ! \ the value that means no piece is present
create theboard \ note is a static board used in all board objects
theboard thepieces% %size parray-max @ * dup allot erase \ array of 25 board pieces

: piece! ( npiece nindex -- )
  thepieces% %size * theboard + ! ;
: piece@ ( nindex -- npiece )
  thepieces% %size * theboard + @ ;
: emptyboard ( -- )
  parray-max @ 0 do nopiece @ i piece! loop ; \ put no piece in array at start
: construct ( -- )
  0 0 0 base-shapes a 0 bshape! \ first shape
  1 0 0 base-shapes b 0 bshape!
  2 0 0 base-shapes c 0 bshape!
  2 0 1 base-shapes d 0 bshape!
  3 0 1 base-shapes e 0 bshape!
  0 0 1 base-shapes a 1 bshape! \ second shape
  1 0 1 base-shapes b 1 bshape!
  2 0 1 base-shapes c 1 bshape!
  2 0 0 base-shapes d 1 bshape!
  3 0 0 base-shapes e 1 bshape!
  0 0 0 base-shapes a 2 bshape! \ third shape
  1 0 0 base-shapes b 2 bshape!
  1 0 1 base-shapes c 2 bshape!
  2 0 1 base-shapes d 2 bshape!
  3 0 1 base-shapes e 2 bshape!
  0 0 1 base-shapes a 3 bshape! \ fourth shape
  1 0 1 base-shapes b 3 bshape!
  1 0 0 base-shapes c 3 bshape!
  2 0 0 base-shapes d 3 bshape!
  3 0 0 base-shapes e 3 bshape!
  creatextrans
  createxytrans
  createxyztrans
  all6rotations
  \ at moment the piece data base is populated
  emptyboard ;
: testing ( -- )
  base-shapes e 3 bshape@ . . . cr
  base-shapes d 2 bshape@ . . . cr
  ." XXXXXXXX" cr
  bshape-max @  0 do base-shapes i bulk@ xyz. ."  #" i . cr loop
  ." ********" cr
  sx-max @ 0 do shapes-x i bulk@ xyz. ."  #" i . cr loop
  ." yyyyyyyyy" cr
  sxy-max @ 0 do shapes-xy i bulk@ xyz. ."  #" i . cr loop
  ." zzzzzzzzz" cr
  sxyz-max @ 0 do shapes-xyz i bulk@ xyz. ."  #" i . cr loop
  ." all------" cr
  allorient-max @ 0 do all-orient i bulk@ xyz. ."  #" i . cr loop
  pindex-max @ 0 do
    all-orient a i bshape@ rot ." a:" . swap . .
    all-orient b i bshape@ rot ." b:" . swap . .
    all-orient c i bshape@ rot ." c:" . swap . .
    all-orient d i bshape@ rot ." d:" . swap . .
    all-orient e i bshape@ rot ." e:" . swap . . ." #" i . cr
  loop ;
: testcompare ( -- )
  1 2 3 1 2 3 test-voxel . ."  <- should be true!" cr
  1 2 3 1 2 5 test-voxel . ."  <- should be false!" cr
  4 0 0 4 0 0 test-voxel . ."  <- should be true!" cr
  0 0 0 0 0 0 test-voxel . ."  <- should be true!" cr
  4 4 4 4 4 4 test-voxel . ."  <- should be true!" cr
  4 0 4 4 4 0 test-voxel . ."  <- should be false!" cr
  0 0 test-collision . ."  <- collision should be true!" cr
  0 1 test-collision . ."  <- collision should be true!" cr
  0 25 test-collision . ."  <- collision should be false!" cr
  all-orient a 0 bshape@ xyz. ."  :a 0" cr
  all-orient b 0 bshape@ xyz. ."  :b 0" cr
  all-orient c 0 bshape@ xyz. ."  :c 0" cr
  all-orient d 0 bshape@ xyz. ."  :d 0" cr
  all-orient e 0 bshape@ xyz. ."  :e 0" cr
  all-orient a 25 bshape@ xyz. ."  :a 25" cr
  all-orient b 25 bshape@ xyz. ."  :b 25" cr
  all-orient c 25 bshape@ xyz. ."  :c 25" cr
  all-orient d 25 bshape@ xyz. ."  :d 25" cr
  all-orient e 25 bshape@ xyz. ."  :e 25" cr
  0 0 0 0 test-voxeltovoxels . ."  <- collision should be true!" cr
  3 0 1 0 test-voxeltovoxels . ."  <- collision should be true!" cr
  3 0 2 0 test-voxeltovoxels . ."  <- collision should be false!" cr ;
: testing2 ( -- )
  499 3 piece!
  cr 3 piece@ . ." should be 499!" cr
  parray-max @ 0 do i piece@ . ."  #" i . cr loop ;
: testpiece ( ntestpiece -- nflag ) \ test ntestpiece with all pieces currently in solution for collision
  \ nflag is false for no collision
  \ nflag is true for a collision
  { ntestpiece }
  try
    current-solution-piece @ 0 ?do
      ntestpiece i piece@ test-collision
      throw \ if a collision then return true
    loop
    false \ if no collision then return false
  restore
  endtry ;
: findpiece ( nstart -- nsolution nflag ) \ test all piece combination placements with current pieces in solution for collision
  \ nflag is false if solution is found
  \ nflag is true if no solutions is found
  \ nsolution is the last pindex solution value tested
  true true rot pindex-max @ swap do 2drop i testpiece if i true else i false leave then loop ;
: zerotest ( n -- n1 )
  dup 0 < if drop 0 then ;
: pmaxtest ( n -- n2 )
  dup pindex-max @ >= if drop 0 current-solution-piece @ 1 - zerotest current-solution-piece ! then ;

0 value view#
25 value nowlow
false value oneshot

: solvepuzzle ( -- )
  construct
  emptyboard
  0 current-solution-piece !
  0 \ start the search from the begining of total pieces
  begin
    findpiece
    if
      \ .s ." no more solutions! " cr parray-max @ current-solution-piece !
      \ here because no solutions on path need to back trace
      drop \ throw away bad solution
      current-solution-piece @ 1 - dup zerotest current-solution-piece ! \ step back current solution pointer
      piece@ 1 + pmaxtest \ get last solved piece and go past that solution
    else
      \ store found solution
      current-solution-piece @ piece! current-solution-piece @ 1 + current-solution-piece !
      0 \ start a new search from the start of total pieces
    then
    oneshot true = if
      current-solution-piece @ nowlow < if current-solution-piece @ to nowlow then
      then
    view# 1 + to view#
    view# 100 > if
      oneshot false = if true to oneshot then
      current-solution-piece @ dup . ."  " piece@ . nowlow ."  low:" .  nowlow piece@ . cr
      0 to view#
    then
    current-solution-piece @ parray-max @ >= \ if true then solution reached if false continue
  until
  ." yay found the solution" ;
: test4 ( -- )
  construct
  emptyboard
  0 current-solution-piece !
  0 0 piece!
  cr ." testpiece " 0 testpiece . cr
  ." findpiece " 0 findpiece . . .s cr
  1 current-solution-piece !
  cr ." testpiece " 0 testpiece . cr
  ." findpiece " 0 findpiece . . .s cr
  cr ." testpiece " 8 testpiece . cr
  ." findpiece " 8 findpiece . . .s cr
  8 1 piece!
  2 current-solution-piece !
  cr ." testpiece " 8 testpiece . cr
  ." findpiece " 8 findpiece . . .s cr ;

\ testing
\ testing2
\ testcompare
\ solvepuzzle
\ findpiece . .
\ test4
