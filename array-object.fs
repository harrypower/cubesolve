require ./Gforth-Objects/objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \ this is a multi dimension cell array
  destruction implementation
  protected
  cell% inst-var storage-location
  cell% inst-var dimensions
  cell% inst-var dimension-sizes
  cell% inst-var dimension-multiply
  m: ( udim0 ... udimx multi-cell-array -- )
    dimensions @ 0 ?do
      dimension-sizes @ i cells + !
    loop
  ;m method dimension-sizes!
  m: ( multi-cell-array -- udim0 ... udimx )
    dimensions @ 0 ?do
      dimension-sizes @ i cells + @
    loop
  ;m method dimension-sizes@

  m: ( multi-cell-array -- )
    cell dimension-multiply @ !
    dimensions @ 0 > if
    dimensions @ 1 ?do
      dimension-sizes @ i 1 - cells + @
      dimension-multiply @ i 1 - cells + @ *
      dimension-multiply @ i cells + !
    loop then
  ;m method dimension-multiply-make

  m: ( multi-cell-array -- umult0 ... umultx )
    dimensions @ 0 ?do
      i cells dimension-multiply @ + @
    loop
  ;m method dimension-multiply@

  m: ( udim0 ... udimx multi-cell-array -- uaddr )
    storage-location @ { uaddr }
    dimensions @ 0 ?do
      dimension-multiply @ i cells + @ * uaddr + to uaddr
    loop uaddr
  ;m method array-addr@
  public
  m: ( umaxdim0 ... umaxdimx udimension-quantity multi-cell-array -- )
    dup dimensions !
    cells allocate throw dimension-sizes !
    this dimension-sizes!
    this dimension-sizes@ dimensions @ 1 ?do * loop cells allocate throw storage-location !
    dimensions @ cells allocate throw dimension-multiply !
    this dimension-multiply-make
  ;m overrides construct

  m: ( multi-cell-array -- )
    dimension-sizes @ free throw
    storage-location @ free throw
    dimension-multiply @ free throw
    0 dimensions !
    0 dimension-sizes !
    0 storage-location !
    0 dimension-multiply !
  ;m overrides destruct

  m: ( nvalue udim0 ... udimx multi-cell-array -- )
    this array-addr@ !
  ;m method cell-array!

  m: ( udim0 ... udimx multi-cell-array -- nvalue )
    this array-addr@ @
  ;m method cell-array@

  m: ( multi-cell-array -- )
    cr this [parent] print cr
    storage-location @ . ." storage-location @" cr
    dimensions @ . ." dimensions @ " cr
    this dimension-multiply@ dimensions @ 0 ?do . loop
     ." dimension-multiply@ in backward order!" cr
    this dimension-sizes@ dimensions @ 0 ?do . loop
    ." dimension-sizes@ in backward order!" cr
  ;m overrides print
end-class multi-cell-array

\ ***************************************************************************************************************************************
\\\
8 1 multi-cell-array heap-new constant testmulti
: singletest
  8 0 ?do i i testmulti [bind] multi-cell-array cell-array! loop
  8 0 ?do i dup . testmulti [bind] multi-cell-array cell-array@ . cr loop ;
singletest
testmulti bind multi-cell-array destruct
5 4 2 testmulti bind multi-cell-array construct
20 value stuff
: doubletest
  4 0 ?do
    5 0 ?do
      stuff dup 1 + to stuff i j testmulti [bind] multi-cell-array cell-array!
    loop
  loop
  4 0 ?do
    5 0 ?do
      i j testmulti [bind] multi-cell-array cell-array@
      . ." stored " i . ." i " j . ." j " cr
    loop
  loop
  ;

doubletest
testmulti bind multi-cell-array destruct
cr .s cr ." tripletest start " cr cr
5 4 3 3 testmulti bind multi-cell-array construct
: tripletest
  3 0 ?do
    4 0 ?do
      5 0 ?do
        stuff dup 1 + to stuff i j k testmulti [bind] multi-cell-array cell-array!
      loop
    loop
  loop
  cr .s ." here" cr
  3 0 ?do
    4 0 ?do
      5 0 ?do
        i j k testmulti [bind] multi-cell-array cell-array@
        . ." stored " i . ." i " j . ." j " k . ." k " cr
      loop
    loop
  loop
  ;

tripletest
testmulti bind multi-cell-array print
testmulti bind multi-cell-array destruct
