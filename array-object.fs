require ./Gforth-Objects/objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \ this is just a 2d cell array
  destruction implementation
  protected
  cell% inst-var storage-location
  cell% inst-var records
  cell% inst-var fields
  cell% inst-var fields*
  m: ( ufields uindex cell-array -- uaddr )
    fields* @ * swap cell * + storage-location @ +
  ;m method calc-array-addr
  public
  m: ( ufields urecords cell-array -- )
    2dup records ! fields !
    * cell * allocate throw storage-location !
    fields @ cell * fields* !
  ;m overrides construct

  m: ( cell-array -- )
    storage-location @ free throw
    0 records !
    0 fields !
    0 fields* !
  ;m overrides destruct

  m: ( ufield uindex cell-array -- nvalue )
    this calc-array-addr @ ;m method ncell-array@

  m: ( nvalue ufield uindex cell-array -- )
    this calc-array-addr ! ;m method ncell-array!

  m: ( nvalue0 ... nvaluex uindex cell-array -- ) \ this stores one row or index of values at a time so there needs to be a complete row of values
    0 swap this calc-array-addr { addr }
    -1 fields @ 1 - ?do
      addr cell i * + !
    1 -loop
  ;m method xcell-array!

  m: ( uindex cell-array -- nvalue0 ... nvaluex ufield )
    0 swap this calc-array-addr { addr }
    fields @ 0 ?do
      addr cell i * + @
    loop fields @
  ;m method xcell-array@

  m: ( cell-array -- ufields urecords )
    fields @ records @ ;m method size@
end-class cell-array

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
    this dimension-multiply@ .s ." dimension-multiply@ " cr
    this dimension-sizes@ .s ." dimension-sizes@" cr
  ;m overrides print
end-class multi-cell-array
\ ***************************************************************************************************************************************
3 3 4 3 multi-cell-array heap-new constant testmulti
.s cr

783 0 0 0 testmulti bind multi-cell-array cell-array!
0 0 0 testmulti bind multi-cell-array cell-array@ . .s
1 0 0 testmulti bind multi-cell-array cell-array@ .
999 1 0 0 testmulti bind multi-cell-array cell-array!
1 0 0 testmulti bind multi-cell-array cell-array@ .

\\\
3 10 cell-array heap-new constant testarray
88 2 3 testarray ncell-array!
2 3 testarray ncell-array@ . ." should be 88!" cr
testarray size@ swap . . ." should be 3 10!" cr

7 8 9 0 testarray xcell-array!
0 testarray xcell-array@ . . . . ." should be 3 9 8 7!" cr

testarray destruct
5 20 testarray construct
testarray destruct
