require ./Gforth-Objects/objects.fs

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
  public
  m: ( npieces piece-array -- ) \ construct the array from the contents of npieces!  Note the size is fixed at construct time!
  ;m overrides construct

  m: ( piece-array -- ) \ destruct the memory used!
  ;m overrides destruct

  m: ( nindex piece-array -- npiece) \ retrieve npiece from array at nindex location
  ;m method npiece@

  m: ( piece-array -- nquantity ) \ return the array size
    pieces-array-quantity @ ;m method quantity@

end-class piece-array
