require ./Gforth-Objects/objects.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
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

\ ***************************************************************************************************************************************

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
