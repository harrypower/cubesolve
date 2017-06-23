require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./Gforth-Objects/stringobj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation  \ ( save -- )
  protected

  inst-value save-file
  inst-value save-file-size
  inst-value save-fid
  inst-value save$

  public
  m: ( nclass caddr u save -- xt ) \ from caddr u string is an instance data name and returns its xt
    \ xt is false if caddr u string does not match any instance data names or method names
    \ this works by using a defered name that is later assigned this class name
    { nclass caddr u }
    caddr u find-name false = if
      nclass push-order
      caddr u find-name dup false = if drop false else name>int then
      nclass drop-order
    else
      caddr u find-name name>int
    then ;m method $>xt
  m: ( nxt save -- caddr u ) \ from the xt of an instance data name return the caddr u string of that named instance data
    \ caddr u is valid if xt is an instance data or method name
    >name dup false = if 0 0 else name>string then
  ;m method xt>$
  m: ( unumber caddr u save -- ) \ put unumber into the inst-value named in string caddr u
    this $>xt dup false <> if <to-inst> else 2drop then
  ;m method #$>value
  m: ( unumber caddr u save -- ) \ put unumber into the inst-var named in string caddr u
    this $>xt dup false <> if execute ! else 2drop then
  ;m method #$>var
  m: ( caddr u save -- ) \ caddr u is a method to be executed
    this $>xt dup false <> if execute else 2drop then
  ;m method $>method
  m: ( save -- ) \ save instance values into save-data
  ;m method make-save-inst-value
  m: ( save -- ) \ retrieve instance values back to there place
  ;m method retrieve-inst-value
  m: ( save -- ) \ create string of the data to be saved
  ;m method make-save-data
  m: ( save -- ) \ save the save data
  ;m method do-save-data
  m: ( save -- ) \ save the data to restart later
    ." Enter the path and file name to save data > "
    save-file 250 accept [to-inst] save-file-size
    save-file save-file-size file-status swap drop
    false = if \ directory and file name there so delete-file
      save-file save-file-size delete-file
    else false then
    false = if
      save-file save-file-size w/o create-file
    else true then
    false <> if
      drop ." could not save file!"
    else
      [to-inst] save-fid
      this make-save-data
      this do-save-data
      save-fid flush-file
      save-fid close-file
    then
  ;m method save-data
  m: ( save -- ) \ constructor
    strings heap-new [to-inst] save$
  ;m overrides construct
  m: ( save -- ) \ destructor
    save$ [bind] strings destruct
  ;m overrides destructor
end-class save
