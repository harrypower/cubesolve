require ./Gforth-Objects/objects.fs
require ./newpieces.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  protected
  char% inst-var pieces-array-start \ start of pieces array address
  char% inst-var pieces-array-quantity \ quantity of pieces in array
  struct
    cell% field piece-cell
  end-struct piece-cell%
  m: ( npiece nindex piece-array -- ) \ store piece object into array
    piece-cell% %size * pieces-array-start @ + piece-cell !
  ;m method npiece!

  public
  m: ( npieces piece-array -- ) \ construct the array from the contents of npieces!  Note the size is fixed at construct time!
    { npieces } npieces pieces-quantity@ dup pieces-array-quantity !
    piece-cell% %size * allocate throw pieces-array-start !
    pieces-array-quantity @ 0 do?
      i npieces get-a-piece
      piece heap-new dup i this npiece!
      [bind] piece copy
    loop
  ;m overrides construct

  m: ( piece-array -- ) \ destruct the memory used!
    \ free the piece's in the array
    \ free the array itself
    \ clean up other stuff
  ;m overrides destruct

  m: ( nindex piece-array -- npiece) \ retrieve npiece from array at nindex location
    piece-cell% %size * pieces-array-start @ + piece-cell @ ;m method npiece@

  m: ( piece-array -- nquantity ) \ return the array size
    pieces-array-quantity @ ;m method quantity@

end-class piece-array
