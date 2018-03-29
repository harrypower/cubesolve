require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs
require ./serialize-obj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

defer -group-lists
save-instance-data class
  destruction implementation
  protected
  cell% inst-var list-storage
  cell% inst-var group-size

  m: ( ntotal-inst-vars group-lists -- ) \ used by serialize only to restore inst-value
    0 ?do
      -group-lists this do-retrieve-inst-var
    loop
  ;m method serial-group-inst-var!

  m: ( nquantity group-lists -- ) \ used by serialize only to restore list data
    0 ?do
      group-size @ 0 ?do
        this [current] do-retrieve-dnumber true <> abort" group data bad!"
        d>s \ list-storage @ [bind] double-linked-list ll-cell!
      loop \ data now on stack
      group-size @ 0 ?do \ store data in the correct order
        list-storage @ [bind] double-linked-list ll-cell!
      loop
    loop
  ;m method serial-group-list!
  public
  m: ( ugroup-size group-lists -- ) \ constructor
  \ ugroup-size defines the dimention of the group per record and is fixed for the life of this object
    this [parent] construct
    group-size !
    double-linked-list heap-new list-storage !
  ;m overrides construct

  m: ( group-lists -- ) \ destruct
    this [parent] destruct
    list-storage @ [bind] double-linked-list destruct
    list-storage @ free throw
    0 group-size !
  ;m overrides destruct

  m: ( upieceindex0 ... upieceindexx group-lists -- ) \ takes the upieceindex data and stores it in the group list
  \ note the upieceindex0 ... upieceindexx data can vary in cell sizes but is fixed at construct time and will always consume same stack cell quantitys
    group-size @ 0 ?do
      list-storage @ [bind] double-linked-list ll-cell!
    loop
  ;m method group!

  m: ( uindex group-lists -- upieceindex0 ... upieceindexx ugroup-size ) \ return the group data for the uindex record in this objects list data structures
    group-size @ * list-storage @ [bind] double-linked-list nll@ 2drop
    group-size @ 0 ?do
      list-storage @ [bind] double-linked-list ll-cell@
      list-storage @ [bind] double-linked-list ll> drop
    loop
    group-size @
  ;m method ugroup@

  m: ( group-lists -- ) \ reset the list to the beggining for group@> usage
    list-storage @ [bind] double-linked-list ll-set-start ;m method group-list-start

  m: ( group-lists -- upieceindex0 ... upieceindexx ugroup-size nflag ) \ return next group data nflag is true when at the end of the data
    \ the data will reset to beginning for next call when nflag is true
    \ nflag is false when data is normal retrieved and list internal index updated to next item
    list-storage @ [bind] double-linked-list ll> true = if
    \ step back for data
      group-size @ 2 ?do
        list-storage @ [bind] double-linked-list ll< drop
      loop
    then
    list-storage @ [bind] double-linked-list ll< drop
    group-size @ 0 ?do
      list-storage @ [bind] double-linked-list ll-cell@
      list-storage @ [bind] double-linked-list ll> drop
    loop
    group-size @
    list-storage @ [bind] double-linked-list ll> dup
    true = if this [current] group-list-start else list-storage @ [bind] double-linked-list ll< drop then
  ;m method group@>

  m: ( group-lists -- uquantity ugroup-size ) \ quantity of the current group list and its group-size per record
    list-storage @ [bind] double-linked-list ll-size@ group-size @ /
    group-size @
  ;m method group-dims@

  m: ( group-lists -- nstrings ) \ return nstrings that contain data to serialize this object
    this [parent] destruct \ to reset save data in parent class
    this [parent] construct

    ['] serial-group-inst-var! this do-save-name
    1 this do-save-nnumber  \ there is 1 inst-var saved here to serialize and retrieve later

    ['] group-size this do-save-inst-var

    ['] serial-group-list! this do-save-name
    list-storage @ [bind] double-linked-list ll-set-start
    this [current] group-dims@
    drop dup this [current] do-save-nnumber
    0 ?do
      this [current] group@> drop 0 ?do this [current] do-save-nnumber loop
    loop

    s" End of group data!" save$ [bind] strings !$x
    save$
  ;m overrides serialize-data@

  m: ( nstrings group-lists -- ) \ nstrings contains serialized data to restore this object
    this [current] destruct
    this [parent] construct
    double-linked-list heap-new list-storage !
    save$ [bind] strings copy$s \ copies the strings object data to be used for retrieval
    this [current] do-retrieve-data true = if d>s rot rot -group-lists rot rot this [current] $->method else 2drop 2drop true abort" group list inst-var data incorrect!" then
    this [current] do-retrieve-data true = if d>s rot rot -group-lists rot rot this [current] $->method else 2drop 2drop true abort" group list data incorrect!" then
  ;m overrides serialize-data!

end-class group-lists
' group-lists is -group-lists

\ *************************************************************************************************************************************

\\\

2 group-lists heap-new constant testgroup

7 9 testgroup group!
33 99 testgroup group!
cr
0 testgroup ugroup@ . . . ." should be 2 7 9!" cr
testgroup group@> . . . . ." should be true 2 33 99!" cr
testgroup group@> . . . . ." should be false 2 7 9!" cr
testgroup group-dims@ . . ." should be 2 2!" cr
strings heap-new constant first$s
testgroup bind group-lists serialize-data@
first$s bind strings copy$s
testgroup bind group-lists destruct

.s ." stack " cr
3 testgroup bind group-lists construct
2 4 8 testgroup group!
100 200 300 testgroup group!
500 499 498 testgroup group!
1 testgroup ugroup@ . . . . ." should be 3 100 200 300!" cr
2 testgroup ugroup@ . . . . ." should be 3 500 499 498!" cr
testgroup group@> . . . . . ." should be true 3 500 499 498!" cr
testgroup group@> . . . . . ." should be false 3 2 4 8!" cr
testgroup group@> . . . . . ." should be false 3 100 200 300!" cr
testgroup group-dims@ . . ." should be 3 3!" cr
." serialize test! " cr
strings heap-new constant temp$s
testgroup bind group-lists serialize-data@ temp$s bind strings copy$s
temp$s testgroup bind group-lists serialize-data!
testgroup group@> . . . . . ." should be false 3 2 4 8!" cr
testgroup group@> . . . . . ." should be false 3 100 200 300!" cr
testgroup group-dims@ . . ." should be 3 3!" cr

first$s testgroup bind group-lists serialize-data!
testgroup group@> . . . . ." should be false 2 7 9!" cr
testgroup group-dims@ . . ." should be 2 2!" cr

testgroup bind group-lists destruct
.s ." stack" cr
