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

makepairs
piece-pair-list group-dims@ drop . ." total pairs !" cr
