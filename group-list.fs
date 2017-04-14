require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/double-linked-list.fs

[ifundef] destruction
  interface
     selector destruct ( -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class
  destruction implementation
  protected
  cell% inst-var list-storage
  cell% inst-var group-size
  cell% inst-var transfer-pad
  public
  m: ( ugroup-size group-list -- ) \ constructor
  \ ugroup-size defines the dimention of the group per record and is fixed for the life of this object
    group-size !
    double-linked-list heap-new list-storage !
    group-size @ cell * allocate throw transfer-pad !
  ;m overrides construct

  m: ( group-list -- ) \ destruct
    list-storage @ [bind] double-linked-list destruct
    transfer-pad @ free throw
    0 group-size !
  ;m overrides destruct

  m: ( upieceindex0 ... upieceindexx group-list -- ) \ takes the upieceindex data and stores it in the group list
  \ note the upieceindex0 ... upieceindexx data can vary in cell sizes but is fixed at construct time and will always consume same stack cell quantitys
    group-size @ 0 ?do
      transfer-pad @ i cell * + !
    loop
    transfer-pad @ cell group-size @ * list-storage @ [bind] double-linked-list ll!
  ;m method group!

  m: ( uindex group-list -- upieceindex0 ... upieceindexx ugroup-size ) \ return the group data for the uindex record in this objects list data structures
    list-storage @ [bind] double-linked-list nll@ drop
    group-size @ 0 ?do
      dup i cell * + @ swap
    loop drop
    group-size @
  ;m method group@

  m: ( group-list -- uquantity ugroup-size ) \ quantity of the current group list recores and its group-size per record
    list-storage @ [bind] double-linked-list ll-size@
    group-size @
  ;m method group-dims@
end-class group-list

\ *************************************************************************************************************************************

\\\

2 group-list heap-new constant testgroup

7 9 testgroup group!
33 99 testgroup group!
0 testgroup group@ . . . ." should be 2 7 9!" cr
testgroup group-dims@ . . ." should be 2 2!" cr
testgroup destruct

3 testgroup construct
2 4 8 testgroup group!
100 200 300 testgroup group!
500 499 498 testgroup group!
1 testgroup group@ . . . . ." should be 3 100 200 300!"

testgroup destruct
