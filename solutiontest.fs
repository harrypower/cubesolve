require ./allpieces.fs
require ./piece-array.fs
require ./group-list.fs

0 puzzle-pieces make-all-pieces heap-new constant map \ this object is used to make reference lists from start pieces
constant ref-piece-list \ this is the reference list of piece created above
ref-piece-list piece-array heap-new constant ref-piece-array \ this object takes reference list and makes a reference array of list for indexing faster
2 group-list heap-new constant piece-pair-list \ this object will contain the reference pair list of all pieces

: makepairs ( -- )
  ref-piece-array [bind] piece-array quantity@ 0 ?do
    i ref-piece-array [bind] piece-array upiece@
    ref-piece-array [bind] piece-array quantity@ 0 ?do
      dup i ref-piece-array [bind] piece-array upiece@
      intersect? false = if
        i j piece-pair-list [bind] group-list group!
        \ store the pairs into piece-pair-list here
      then
    loop drop
  loop ;

\ makepairs
\ piece-pair-list group-dims@ drop . ." total pairs !" cr

: findthree ( -- )
  0 0 0 0 { p0 p1 p2 total }
  ref-piece-array [bind] piece-array quantity@ 0 ?do
    i ref-piece-array [bind] piece-array upiece@ to p0
    ref-piece-array [bind] piece-array quantity@ 0 ?do
      p0 i ref-piece-array [bind] piece-array upiece@ dup to p1
      intersect? false = if
        ref-piece-array [bind] piece-array quantity@ 0 ?do
          p0 i ref-piece-array [bind] piece-array upiece@ dup to p2
          intersect? p1 p2 intersect? or false = if
            total dup 1 + to total . ." current three total .. current k " k . cr
          then
        loop
      then
    loop
  loop total ;

\ findthree . ." three group total !" cr

: uref-piece@ ( uindex -- upiece )
  ref-piece-array [bind] piece-array upiece@ ;

ref-piece-array bind piece-array quantity@ constant refqty@

: test-intersect? ( upiece0 upiece1 upiece2 upiece3 upiece4 -- nflag )
  { p0 p1 p2 p3 p4 }
  try
    p0 p1 intersect? throw
    p0 p2 intersect? throw
    p0 p3 intersect? throw
    p0 p4 intersect? throw
    p1 p2 intersect? throw
    p1 p3 intersect? throw
    p1 p4 intersect? throw
    p2 p3 intersect? throw
    p2 p4 intersect? throw
    p3 p4 intersect? throw
    false
  restore
  endtry ;

: findfive ( -- total )
  0 0 0 0 0 0 { p0 p1 p2 p3 p4 total }
  refqty@ 0 ?do
    i uref-piece@ to p0
    0 0 at-xy i . ." m"
    refqty@ 0 ?do
      i uref-piece@ to p1
      0 2 at-xy i . ." l"
      refqty@ 0 ?do
        i uref-piece@ to p2
        0 4 at-xy i . ." k"
        refqty@ 0 ?do
          i uref-piece@ to p3
          refqty@ 0 ?do
            i uref-piece@ to p4
            p0 p1 p2 p3 p4 test-intersect? false = if
              total 1 + dup to total
              0 6 at-xy . ." total"
              0 8 at-xy i . ." i"
              0 10 at-xy j . ." j"
            then
          loop
        loop
      loop
    loop
  loop total ;

page
findfive . ." total for five grouped !" cr
