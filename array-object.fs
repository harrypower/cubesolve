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
  public
  m: ( urecords ufields cell-array -- )
    2dup fields ! records !
    * cell * allocate throw storage-location !
  ;m overrides construct

  m: ( cell-array -- )
    storage-location @ free throw
    0 records !
    0 fields !
  ;m overrides destruct

  m: ( uindex ufield cell-array -- nvalue )
  ;m method ncell-array@

  m: ( nvalue uindex ufield cell-array -- )
  ;m method ncell-array!

  m: ( nvalue1 ... nvaluex uindex ufield cell-array -- )
  ;m method xcell-array!

  m: ( uindex cell-array -- nvalue1 ... nvaluex ufield )
  ;m method xcell-array@

  m: ( cell-array -- urecords ufields )
    records @ fields @ ;m method size@
end-class cell-array
