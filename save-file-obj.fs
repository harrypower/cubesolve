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
  inst-value working$
  inst-value parse$
  inst-value separate$
  public
  m: ( save -- ) \ create string of the data to be saved
    separate$ @$ working$ !$
    save$ [bind] strings reset
    save$ [bind] strings $qty 0 ?do
      save$ [bind] strings @$x working$ !+$
      separate$ @$ working$ !+$
    loop
  ;m method make-save-data
  m: ( save -- ) \ save the save data
    working$ @$ save-fid write-file throw
  ;m method do-save-data
  m: ( save -- caddr u nflag ) \ caddr u contains the data found before the seperator and nflag is true
    \ caddr u contains an empty string if the seperator is not found and nflag is false
    \ working$ is adjusted to have the remander of the string after seperator removed and the part before seperator is also removed
    \ note working$ could be an empty string after this process ends !
    separate$ @$ working$ [bind] string split$ ( caddrf uf caddrl ul nflag )
    >r 2swap parse$ [bind] string !$ working$ [bind] string !$ parse$ [bind] string @$ r>
    \ working$ [bind] string @$ dump cr ." ^ (do-parse-data)" cr
  ;m method (do-parse-data)
  m: ( save -- ) \ parse the data from working$ buffer
    try
      this (do-parse-data) false = if true throw then 2drop
      begin
        this (do-parse-data) false = if true throw then \ 2dup dump cr
        save$ [bind] strings !$x
        working$ [bind] string len$ 0 = \ .s cr working$ [bind] string @$ dump cr
      until
      false
    restore if save$ [bind] strings destruct then
    endtry
  ;m method do-parse-data
  m: ( nstrings save -- ) \ save the data to restart later
    \ nstrings is the strings object that contains strings of data to be put into a save file
    \ note the nstrings strings object is copied into this object and not altered in any way
    save$ [bind] strings destruct
    save$ [bind] strings copy$s
    cr ." Enter the path and file name to save data > "
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
      save-fid flush-file throw
      save-fid close-file throw
    then
  ;m method save-data
  m: ( save -- nstrings ) \ retrieve the saved data from file
    save$ [bind] strings destruct
    cr ." Enter the path and file name of data file to retrieve > "
    save-file 250 accept [to-inst] save-file-size
    save-file save-file-size file-status swap drop
    false = if \ directory and file name there so retrieve the data
      save-file save-file-size slurp-file
      working$ [bind] string !$
      this do-parse-data
    else
      cr ." The path and file name does not exist!"
    then
    save$
  ;m method retrieve-data
  m: ( save -- ) \ constructor
    250 allocate throw [to-inst] save-file
    strings heap-new [to-inst] save$
    string heap-new [to-inst] working$
    string heap-new [to-inst] separate$
    string heap-new [to-inst] parse$
    s" |||" separate$ [bind] string !$
  ;m overrides construct
  m: ( save -- ) \ destructor
    save-file free throw
    save$ [bind] strings destruct
    working$ [bind] string destruct
    separate$ [bind] string destruct
  ;m overrides destruct
end-class save

\ ***********************************************************************************************************************

\\\

save heap-new constant testsave
strings heap-new constant dataset

s" somedata" dataset !$x
s" 923" dataset !$x
s" moredata" dataset !$x
s" 27" dataset !$x

 dataset testsave save-data

 testsave retrieve-data
