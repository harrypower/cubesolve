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
  base-shapes blc% %size 4 * dup allot erase \ 4 sets of pieces data
  create orientations
  orientations blc% %size 2 * 5 * 4 * 4 *  dup allot erase \ basic piece one orientation
  \ 2 x translation placements 5 y translation placements 4 z translation placements of 4 sets of pieces
  protected
  m: ( nx ny nz nbase-shapes-addr nindex piece -- ) \ to store basic-shape data array
    { nx ny nz nbsa nindex }
    nx nbsa x nindex blc% %size * + !
    ny nbsa y nindex blc% %size * + !
    nz nbsa z nindex blc% %size * + !
  ;m method bshape!
  m: ( nbase-shapes-addr nindex piece -- nx ny nz )
    { nbsa nindex }
    nbsa x nindex blc% %size * + @
    nbsa y nindex blc% %size * + @
    nbsa z nindex blc% %size * + @
  ;m method bshape@
  public
  m: ( piece -- )
    0 0 0 base-shapes a 0 this bshape!
    1 0 0 base-shapes b 0 this bshape!
    2 0 0 base-shapes c 0 this bshape!
    2 0 1 base-shapes d 0 this bshape!
    3 0 1 base-shapes e 0 this bshape!
  ;m overrides construct
  m: ( piece -- )
    base-shapes e 0 this bshape@
  ;m method testing
end-class piece

piece class

end-class board

piece heap-new constant ptest
ptest testing
