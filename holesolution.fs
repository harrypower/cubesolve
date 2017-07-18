require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./Gforth-Objects/stringobj.fs
require ./newpieces.fs
require ./puzzleboard.fs
require ./allpieces.fs
require ./piece-array.fs
require ./ref-puzzle-pieces.fs
require ./serialize-obj.fs

: pause-for-key ( -- nkey ) \ halt until a key is pressed then continue nkey is the key pressed when this word returns
  begin key? until
  key 10 ms ;

: key-test-wait ( -- nkey ) \ if keyboard is pressed pause until it is pressed again
  \ the second key pressed is returned as nkey
  key?
  if
    key drop 10 ms
    pause-for-key
  else 0
  then ;

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]
defer -hole-solution
save-instance-data class
  destruction implementation  \ ( hole-solution -- )
  protected
  selector next-hole          \ ( hole-solution -- )
  selector solution-size@     \ ( hole-solution -- usize )
  cell% inst-var board-array        \ multi-cell-array object containing current puzzle board in this hole-solution
  cell% inst-var a-ref-piece-array  \ piece-array object containing the reference piece array passed to this hole-solution
  cell% inst-var a-hapl             \ hole-array-piece-list object containing the reference hapl passed to this hole-solution
  cell% inst-var solution-piece-list  \ double-linked-list object containing reference pieces of current puzzle solution
  inst-value sol-piece-voxel-list   \ a linked list of solution voxels that match the solution-piece-list reference pieces as placed
  inst-value x-display-size         \ contains constants that are used to display the board in a terminal
  inst-value y-display-size
  inst-value z-display-size
  inst-value x-max
  inst-value y-max
  inst-value z-max
  inst-value x-now
  inst-value y-now
  inst-value z-now
  inst-value final-solution         \ holds the number of pieces needed that constitutes a solution
  inst-value solveloops             \ for display purposes the amount of loops between showing current solution
  inst-value solvelow
  inst-value solvelow-flag
  inst-value solvehigh
  inst-value del-piece
  m: ( hole-solution -- nflag ) \ test if puzzle can be solved and if so place piece count needed for soulution in final-solution
  \ nflag is true if puzzle can be solved and false if the pieces do not add up to a solution
    x-max y-max * z-max *
    0 a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@
    2dup / [to-inst] final-solution
    mod 0= if true else false then
  ;m method test-solvable?
  m: ( uref-piece hole-solution -- nflag ) \ test if uref-piece can be placed into current board
  \ this test will look through the current pieces in solution-piece-list using a-ref-piece-array object fast-intersect? test!
  \ nflag is true if uref-piece can be placed into current board and false if uref-piece can not be placed into current board
    0 { uref-piece nnumber }
    solution-piece-list @ [bind] double-linked-list ll-set-start
    solution-piece-list @ [bind] double-linked-list ll-size@
    0 =
    if
      true \ list empty so no intersection
    else
      begin
        solution-piece-list @ [bind] double-linked-list ll-cell@ to nnumber solution-piece-list @ [bind] double-linked-list ll> true =
        if \ at end of list
          nnumber uref-piece \ .s ." data into fast test at end" cr
          a-ref-piece-array @ [bind] piece-array fast-intersect?
          if false true else true true then \ if intersection found leave loop with false on stack otherwise leave loop with true on stack
        else \ in middle of list
          nnumber uref-piece \ .s ." data into fast test in middle" cr
          a-ref-piece-array @ [bind] piece-array fast-intersect?
          if false true else false then \ if an intersection is found leave loop with false on stack otherwise continue loop
        then
      until
    then
  ;m method intersect-test?

  m: ( unumber ux uy uz hole-solution -- ) \ place unumber in board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array! ;m method board-array!
  m: ( ux uy uz hole-solution -- unumber ) \ retrieve unumber from board-array at ux uy uz board-array address
    board-array @ [bind] multi-cell-array cell-array@ ;m method board-array@
  m: ( uvoxel uref-piece hole-solution -- ux uy uz ) \ get voxel address from uref-pieces
    a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel ;m method voxel-address@
  m: ( uref-piece hole-solution -- ) \ store uref-piece in board-array as defined by the voxels in the piece that uref-piece defines
    { uref-piece } uref-piece a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@ 0 ?do
      uref-piece \ store the reference piece in board-array
      i uref-piece this voxel-address@
      this board-array!
    loop ;m method add-board-piece
  m: ( uref-piece hole-solution -- ) \ remove uref-piece from board-array as defined by the voxels in the piece that uref-piece defines
    { uref-piece } uref-piece a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece voxel-quantity@ 0 ?do
      true \ true is the place holder for no piece in board-array
      i uref-piece this voxel-address@
      this board-array!
    loop ;m method del-board-piece
  m: ( nx ny nz hole-solution -- ) \ store solution piece voxel in sol-piece-voxel-list linked list
    0 41 at-xy .s ." adding this to sol-piece-voxel-list" 500 ms 0 41 at-xy ."                                                                                  "
    sol-piece-voxel-list [bind] double-linked-list ll-cell! \ nz
    sol-piece-voxel-list [bind] double-linked-list ll-cell! \ ny
    sol-piece-voxel-list [bind] double-linked-list ll-cell! \ nx
  ;m method sol-piece-voxel-list!
  m: ( hole-solution -- nx ny nz ) \ retrieve solution piece voxel from sol-piece-voxel-list linked list and remove them from the linked list itself
    sol-piece-voxel-list [bind] double-linked-list ll-set-end
    sol-piece-voxel-list [bind] double-linked-list ll-cell@    \ nx
    sol-piece-voxel-list [bind] double-linked-list delete-last
    sol-piece-voxel-list [bind] double-linked-list ll-set-end
    sol-piece-voxel-list [bind] double-linked-list ll-cell@    \ ny
    sol-piece-voxel-list [bind] double-linked-list delete-last
    sol-piece-voxel-list [bind] double-linked-list ll-set-end
    sol-piece-voxel-list [bind] double-linked-list ll-cell@    \ nz
    sol-piece-voxel-list [bind] double-linked-list delete-last
  ;m method sol-piece-voxel-list@
  m: ( uref-piece hole-solution -- ) \ add uref-piece to solution-piece-list at last in list position
  \ also add uref-piece to board-array multi-cell-array object to allow fast display
    dup
    solution-piece-list @ [bind] double-linked-list ll-cell!
    this add-board-piece
    x-now y-now z-now this sol-piece-voxel-list!
  ;m method add-solution-piece
  m: ( hole-solution -- ) \ delete the last reference added to solution-piece-list
  \ remove the last reference added from the board-array to ensure it is updated
    solution-piece-list @ [bind] double-linked-list ll-set-end
    solution-piece-list @ [bind] double-linked-list ll-cell@
    this del-board-piece
    solution-piece-list @ [bind] double-linked-list delete-last ;m method del-solution-piece
  m: ( hole-solution -- usize ) \ return the current size of solution-piece-list
    solution-piece-list @ [bind] double-linked-list ll-size@ ;m overrides solution-size@

  m: ( uref-piece hole-solution -- nflag ) \ test intersecting for uref-piece and place piece in solution-piece-list
  \ nflag is true if uref-piece can be placed in solution-piece-list and is placed in list and board array
  \ nflag is false if uref-piece can not be placed in solution-piece-list and is not placed in list and board array
  \ note uref-piece is placed into solution-piece-list and put on the board-array for display purposes if it can be placed at all
    dup this intersect-test? if this add-solution-piece true else drop false then ;m method place-piece?
  m: ( ux uy hole-solution -- ) \ display current solution reference numbers
    0 { ux uy nnumber }
    solution-piece-list @ [bind] double-linked-list ll-set-start
    begin
      ux uy 1 + dup to uy at-xy
      solution-piece-list @ [bind] double-linked-list ll-cell@ to nnumber
      solution-piece-list @ [bind] double-linked-list ll>
      nnumber .
      ux 5 + uy at-xy
      0 nnumber a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel
      rot . swap . .
      0 nnumber a-ref-piece-array @ [bind] piece-array upiece@ [bind] piece get-voxel
      ux 15 + uy at-xy
      a-hapl @ [bind] hole-array-piece-list hole-list-quantity@ .
    until ;m method display-current-solution-list

  m: ( hole-solution -- ux uy uz ) \ return current hole address to fill with out changing it
    x-now y-now z-now ;m method current-hole
  m: ( hole-solution -- ) \ increment current hole address
    x-now 1 + dup [to-inst] x-now
    x-max = if
      0 [to-inst] x-now
      y-now 1 + dup [to-inst] y-now
      y-max = if
        0 [to-inst] y-now
        z-now 1 + dup [to-inst] z-now
        z-max = if
          0 [to-inst] z-now
        then
      then
    then ;m method hole+
\  m: ( hole-solution -- ) \ decrement current hole address
\    x-now 1 - dup [to-inst] x-now
\    0< if x-max 1 - [to-inst] x-now
\      y-now 1 - dup [to-inst] y-now
\      0< if y-max 1 - [to-inst] y-now
\        z-now 1 - dup [to-inst] z-now
\        0< if z-max 1 - [to-inst] z-now
\        then
\      then
\    then ;m method hole-
  m: ( hole-solution -- ) \ increment to next hole address to fill
    begin
      this current-hole
      this board-array@
      true = if true else this hole+ false then
    until
  ;m overrides next-hole
  m: ( hole-solution -- ) \ impliment the hole solution
  \  0 0 { del-ref  del-piece }
    begin
      \ del-piece this current-hole 0 40 at-xy .s ." this is hole at entry"
      \ 500 ms
      \ 0 40 at-xy ."                                                       "
      \ 2drop 2drop
      \ del-piece x-del x-now = and y-del y-now = and z-del z-now = and
      del-piece true = if
        \ xyx stuff
        this sol-piece-voxel-list@ 0 39 at-xy .s ." pre test"
        z-now = swap y-now = and swap x-now = and 0 40 at-xy .s ." test result"
        700 ms 0 39 at-xy ."                                                                      "
        0 40 at-xy ."                                             "
        true = if
          this del-solution-piece
          0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
          this next-hole
          \ 0 39 at-xy ." condition met second delete piece"
          \ 1000 ms
          \ 0 39 at-xy ."                                    "
        \ else 0 39 at-xy ." !!!!!continue !!!!!" 200 ms ."                                           "
        then
        false [to-inst] del-piece
      then
      this current-hole
      a-hapl @ [bind] hole-array-piece-list next-ref-piece-in-hole@
      if \ at end of hole references
        this place-piece?
        if \ next hole because hole was filled with last one .. now exit this begin until
        \  0 39 at-xy ." hole filled but at end of references"
        \  300 ms
        \  0 39 at-xy ."                                      "
          true
        else \ last hole because hole was not filled with last one  .. now exit  this begin until
        \  0 39 at-xy ." at delete process!"
        \  300 ms
        \  0 39 at-xy ."                    "
          \ 0 41 at-xy ." deleted location "
          \ x-del . y-del . z-del .
          \ 1000 ms
          \ 0 41 at-xy ."                                                "
          true [to-inst] del-piece
          this del-solution-piece
          0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
          true
  \        begin
  \          0 39 at-xy ." starting delete process "
  \          solution-piece-list @ [bind] double-linked-list ll-set-end
  \          solution-piece-list @ [bind] double-linked-list ll@ anumberbuffer swap move
  \          anumberbuffer @ to del-ref  \ now del-ref has the reference piece that will be deleted
  \          this del-solution-piece
  \          0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
  \          this next-hole
  \          this current-hole 0 43 at-xy rot . swap . .
  \          del-ref a-ref-piece-array @ [bind] piece-array upiece@ to del-piece
  \          0 del-piece [bind] piece voxel-quantity@ 0 ?do
  \            i del-piece [bind] piece get-voxel ( x y z ) 0 40 at-xy .s
  \            this current-hole ( x y z x' y' z' ) 0 41 at-xy .s
  \            3 pick = swap 4 pick = and swap 4 pick = and swap drop swap drop swap drop
  \            or
  \          loop
  \          invert
  \          0 42 at-xy .s ." loop exit ?"
  \          solution-piece-list @ [bind] double-linked-list ll-size@ 30 42 at-xy .
  \          pause-for-key drop
  \          0 43 at-xy ."                                            "
  \          0 42 at-xy ."                                            "
  \          0 39 at-xy ."                                            "
  \          0 40 at-xy ."                                             "
  \          0 41 at-xy ."                                             "
  \          \ this current-hole
  \          \ 0 = swap 0 = and swap 0 = and true =
  \          solution-piece-list @ [bind] double-linked-list ll-size@ 0 = if
  \            0 38 at-xy ." down to first piece placed!"
  \            200 ms
  \            0 38 at-xy ."                             "
  \            \ page
  \            this current-hole
  \            a-hapl @ [bind] hole-array-piece-list next-ref-piece-in-hole@ drop
  \            this place-piece? drop
  \            \ this see-solution
  \            drop true
  \          then
  \        until
  \        0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
  \        true
        then
      else \ not at end of hole references
        this place-piece?
        if \ next hole because hole was filled with last one .. now exit this begin until
          true
        \  0 38 at-xy ." hole filled exit"
        \  300 ms
        \  0 38 at-xy ."                  "
        else \ next reference at same hole because hole was not filled ... do not exit this begin until
          false
        \  0 38 at-xy ." repeat hole solution"
        \  300 ms
        \  0 38 at-xy ."                      "
        then
      then
    until
  ;m method the-hole-solution
  m: ( namount hole-solution -- ) \ used only by restoring puzzle to load data into solution-piece-list
    0 ?do
      this do-retrieve-dnumber true = if d>s this add-solution-piece else 2drop then
    loop
  ;m method retrieve-solution-piece-list
  m: ( namount hole-solution -- ) \ used ony by restoring puzzle to load data into all the instance values of this hole-solution
    0 ?do
      -hole-solution this do-retrieve-inst-value
    loop
  ;m method retrieve-values
  public
  m: ( uref-piece-array uhapl hole-solution -- ) \ constructor
    this [parent] construct
    6 [to-inst] x-display-size
    1 [to-inst] y-display-size
    x-display-size y-display-size * [to-inst] z-display-size
    a-hapl !
    a-ref-piece-array !
    a-hapl @ [bind] hole-array-piece-list hole-max-address@ [to-inst] z-max [to-inst] y-max [to-inst] x-max
    x-max y-max z-max 3 multi-cell-array heap-new board-array !
    z-max 0 ?do
      y-max 0 ?do
        y-max 0 ?do
          true i j k this board-array!
        loop
      loop
    loop
    double-linked-list heap-new solution-piece-list !
    0 [to-inst] x-now 0 [to-inst] y-now 0 [to-inst] z-now
    0 [to-inst] solveloops
    this test-solvable? drop
    final-solution [to-inst] solvelow
    false [to-inst] solvelow-flag
    0 [to-inst] solvehigh
    false [to-inst] del-piece
    double-linked-list heap-new [to-inst] sol-piece-voxel-list
  ;m overrides construct
  m: ( hole-solution -- ) \ destructor
    this [parent] destruct
    board-array @ [bind] multi-cell-array destruct
    board-array @ free throw
    solution-piece-list @ [bind] double-linked-list destruct
    solution-piece-list @ free throw
    sol-piece-voxel-list [bind] double-linked-list destruct
    sol-piece-voxel-list free throw
  ;m overrides destruct

  m: ( hole-solution -- ) \ basic terminal view of the board-array reference pieces solution
    z-max 0 ?do
      y-max 0 ?do
        x-max 0 ?do
          i x-display-size * j y-display-size * k z-display-size * + at-xy
          i j k this board-array@ dup true = if drop ." *****" else 5 u.r then
        loop
      loop
    loop
  ;m method see-solution
  m: ( hole-solution -- )
    begin
      this next-hole
      this the-hole-solution
      this solution-size@ solvehigh > if this solution-size@ [to-inst] solvehigh then
      solveloops 3000 > if true [to-inst] solvelow-flag then
      this solution-size@ solvelow < solvelow-flag and if this solution-size@ [to-inst] solvelow then
      solveloops 3000 > if
        0 [to-inst] solveloops this see-solution
        40 0 at-xy this solution-size@ . ." solution-size"
        40 1 at-xy solvehigh . ." highest"
        40 2 at-xy solvelow . ." lowest"
        this current-hole 40 3 at-xy rot . swap . . ." current-hole"
        40 7 this display-current-solution-list
        0 35 at-xy ." press any key to pause ... press any key to continue ... press x key to stop ... press r to reset low and high "
      else
        solveloops 1 + [to-inst] solveloops
      then
      page
      this see-solution
      40 0 at-xy this solution-size@ . ." solution-size"
      40 1 at-xy solvehigh . ." highest"
      40 2 at-xy solvelow . ." lowest"
      this current-hole 40 3 at-xy rot . swap . . ." current-hole"
      40 7 this display-current-solution-list
      0 35 at-xy ." press any key to pause ... press any key to continue ... press x key to stop ... press r to reset low and high "
      \ key-test-wait case
      pause-for-key case
        120 of true endof
        114 of final-solution [to-inst] solvelow 0 [to-inst] solvehigh false false [to-inst] solvelow-flag endof
        this solution-size@ final-solution = swap
      endcase
    until
  ;m method continue-solving
  m: ( hole-solution -- ) \ solve puzzle and display partial solutions and steps working on along the way
    this test-solvable? if
      page
      this current-hole
      a-hapl @ [bind] hole-array-piece-list next-ref-piece-in-hole@ drop
      this place-piece? drop
      this see-solution
      this continue-solving
      page this see-solution
      this solution-size@ final-solution = if 40 0 at-xy ." solution found!" else 40 0 at-xy ." solving halted!" then
    else
      ." The puzzle board can not hold the puzzle pieces in an even quantity!  Puzzle is not solvable!" cr
    then
  ;m method start-solving

  m: ( hole-solution -- nsolution-string ) \ makes and returns the current solution state
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct
    ['] retrieve-solution-piece-list this do-save-name \ to restore solution-piece-list data and board-array data from solution-piece-list
    this solution-size@ dup this do-save-nnumber \ quantity of solution-piece-list data
    solution-piece-list @ [bind] double-linked-list ll-set-start
    0 ?do
      solution-piece-list @ [bind] double-linked-list ll-cell@
      solution-piece-list @ [bind] double-linked-list ll> drop
      this do-save-nnumber
    loop
    ['] retrieve-values this do-save-name \ to restore values
    14 this do-save-nnumber \ this is the count of the values that are saved below
    ['] x-display-size this do-save-inst-value
    ['] y-display-size this do-save-inst-value
    ['] z-display-size this do-save-inst-value
    ['] x-max this do-save-inst-value
    ['] y-max this do-save-inst-value
    ['] z-max this do-save-inst-value
    ['] x-now this do-save-inst-value
    ['] y-now this do-save-inst-value
    ['] z-now this do-save-inst-value
    ['] final-solution this do-save-inst-value
    ['] solveloops this do-save-inst-value
    ['] solvelow this do-save-inst-value
    ['] solvelow-flag this do-save-inst-value
    ['] solvehigh this do-save-inst-value
    save$
  ;m method save-solution
  m: ( nsolution-string hole-solution -- ) \ restore the solutions state from nsolution-string
    this destruct
    a-ref-piece-array @ a-hapl @ this construct \ ensture object is set to beginning state
    save$ [bind] strings copy$s \ saves the strings object data to be used for retrieval
    this do-retrieve-data true = if d>s rot rot -hole-solution rot rot this $->method else 2drop 2drop abort" restore data incorrect!" then
    \ this above lines retrieves the solution-piece-list data and the board-array is restored with it.
    this do-retrieve-data true = if d>s rot rot -hole-solution rot rot this $->method else 2drop 2drop abort" restore data incorrect!" then
    \ this above line retrieves all the instance values for this hole-solution to continue ... values saved are in save-solution method
  ;m method restore-solution

  m: ( uref-piece hole-solution -- nflag ) \ test intersect-test? method
    this place-piece? ;m method test-intersect

  m: ( hole-solution -- )  \ test del-solution-piece
    this del-solution-piece ;m method removeit

  m: ( hole-solution -- usize nflag ) \ usize is the current solution size nflag is true if solution is found false if not found
    this solution-size@ dup
    final-solution =
  ;m method  currentsize@

  m: ( hole-solution -- ux uy uz ) \ test next hole solution
    this next-hole
    this current-hole ;m method nexthole@
end-class hole-solution
' hole-solution is -hole-solution
\ ***************************************************************************************************************************************
\\\
0 puzzle-pieces make-all-pieces heap-new constant map         \ this object is used to make reference lists from start pieces
constant ref-piece-list                                       \ this is the reference list of piece`s created above
ref-piece-list piece-array heap-new constant ref-piece-array  \ this object takes reference list from above and makes a reference array of list for indexing faster

ref-piece-array puzzle-board hole-array-piece-list heap-new constant hapl

ref-piece-array hapl hole-solution heap-new constant testsolution

testsolution destruct
testsolution free throw
\ page
\ testsolution start-solving
