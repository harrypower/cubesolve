## This is somewhat of an outline of objects and or data structures and methods needed to go forward with general cube puzzle solver!

* ### voxel
  newpieces.fs
  * `construct`       _( voxel -- )_
      - construct voxel with 0 0 0 as voxel coordinates
  * `voxel!`          _( ux uy uz voxel -- )_
      - store the voxel coordinates
  * `voxel@`          _( voxel -- ux uy uz )_
      - retrieve voxel coordinates
  * `intersect?`      _( uvoxel voxel -- nflag )_
      - nflag is true if uvoxel is intersecting with voxel nflag is false if not intersecting


* ### piece
  newpieces.fs
  * `construct`         _( piece -- )_
      - construct piece object
  * `destruct`          _( piece -- )_
      - destruct and free data in object
  * `add-voxel`         _( ux uy uz piece -- )_
     - add a voxel to this piece with ux uy uz address
  * `get-voxel-object`  _( uindex piece -- uvoxel )_
     - return a voxel object at uindex
  * `get-voxel`         _( uindex piece -- ux uy uz )_
     - retrieve voxel data from uindex voxel in this piece
  * `voxel-quantity@`   _( piece -- usize )_
     - return voxel quantity
  * `intersect?`        _( upiece piece -- nflag )_
     - test for intersection of upiece with this piece on any voxel
  * `same?`             _( upiece piece -- nflag )_
     - test upiece agains this piece for exact voxels match forward or backward
  * `copy`              _( upiece piece -- )_
     - exact copy upiece to this piece
  * `serialize-data@`   _( piece -- nstrings )_
     - to save this piece data
  * `serialize-data!`   _( nstrings piece -- )_
     - to restore previously saved data

* ### pieces
  newpieces.fs
  * `construct`         _( pieces -- )_
     - construct pieces object
  * `destruct`          _( pieces -- )_
     - destruct pieces object
  * `add-a-piece`       _( upiece pieces -- )_
     - copies contents of upiece object and puts the copied piece object in a-pieces-lis
  * `get-a-piece`       _( uindex pieces -- upiece )_
     - retrieve uindex piece from a-pieces-list
  * `pieces-quantity@`  _( pieces -- usize )_
     - return the quantity of pieces in this piece list
  * `serialize-data@`   _( pieces -- nstrings )_
     - to save this list of pieces
  * `serialize-data!`   _( nstrings pieces -- )_
     - to restore previously saved list of pieces


* ### save-instance-data
  serialize-obj.fs
  * `construct`         _( save-instance-data -- )_
  * `destruct`          _( save-instance-data -- )_
  * `serialize-data@`   _( save-instance-data -- nstrings )_
     - method that is empty and is the suggested name of method for making serialized data
  * `serialize-data!`   _( nstrings save-instance-data -- )_
     - method that is empty and is the suggested name of the method for retrieving the serialized data in nstrings

  #### private inst-value  
  * `save$`           
      - this is the strings object handle containing the serialized data

  #### private methods that are not normally used directly by inherited class but can be used if understood how they work.
    * `#sto$`           _( ns save-instance-data -- caddr u )_
        - convert ns to string
    * `$>xt`            _( nclass caddr u save-instance-data -- xt )_
        - caddr u string is an instance data name and returns its xt. nclass is also needed
    * `xt>$`            _( nxt save-instance-data -- caddr u )_
        - from the xt of an instance data name return the caddr u string of that named instance data
    * `#$>value`        _( unumber nclass caddr u save-instance-data -- )_
        - put unumber into the inst-value named in string caddr u ... note nclass is needed!
    * `#$>var`          _( unumber nclass caddr u save-instance-data -- )_
        - put unumber into the inst-var named in string caddr u ... note nclass is needed!
    * `$->method`       _( nclass caddr u save-instance-data -- )_
        - caddr u is a method to be executed ... note nclass is needed!

  #### private methods that are normally used directly by inherited class to save and restore data into save$ strings object
    * `do-save-name`            _( xt save-instance-data -- )_
        - saves the name string of xt by getting the nt first name to save$
    * `do-save-inst-value`      _( xt save-instance-data -- )_
        - saves the instance value referenced by xt to save$
    * `do-save-inst-var`        _( xt save-instance-data -- )_
        - saves the instance var referenced by xt to save$
    * `do-save-nnumber`         _( nnumber save-instance-data -- )_
        - saves nnumber to save$ - note this is a cell wide number
    * `do-retrieve-dnumber`     _( save-instance-data -- dnumber nflag )_
        - retrieve string number from save$
    * `do-retrieve-data`        _( save-instance-data -- caddr u dnumber nflag )_
        - retrieve string name and string number from save$
    * `do-retrieve-inst-var`    _( nclass save-instance-data -- )_
        - restores instance var from save$
    * `do-retrieve-inst-value`  _( nclass save-instance-data -- )_
        - restores instance value from save$

* ### piece-array
  piece-array.fs
  * `construct`         _( upieces piece-array -- )_
      - construct the array from the contents of upieces!  Note the size is fixed at construct time!
      - construct the intersect array of reference pieces.
  * `destruct`          _( piece-array -- )_
  * `upiece@`           _( uindex piece-array -- upiece)_
      - retrieve upiece from array at uindex location
  * `fast-intersect?`   _( uindex0 uindex1 piece-array -- nflag )_
      - return nflag from intersect-array to get fast intersect detection for uindex0 and uindex1 pieces
      - nflag is true if an intersection between uindex0 and uindex1 is found
      - nflag is false if no intersection is found
  * `quantity@`         _( piece-array -- nquantity )_
      - return the array size
  * `serialize-data@`   _( piece-array -- nstrings )_
      - return the serialized data of this object in nstrings
  * `serialize-data!`   _( nstrings piece-array -- )_
      - nstrings contains the serialized data to restore this object

* ### make-all-pieces
  allpieces.fs
  * `construct`         _( uindex upieces make-all-pieces -- upieces2 )_
      - constructor
      - uindex is the reference to the pieces object piece defined in newpuzzle.def file
      - upieces2 is the returned pieces object that contains the total list of pieces that can be in board as defined by upieces and puzzle-board
      - note newpuzzle.def contains the dimensions of the board used here in three constants
  * `destruct`          _( make-all-pieces -- )_

* ### fast-puzzle-board
  fast-puzzle-board.fs
  * `construct`         _( uref-piece-array fast-puzzle-board -- )_
      - constructor
      - uref-piece-array is a piece array that contains all the pieces as a reference
      - note newpuzzle.def contains the dimensions of the board used here in three constants
  * `destruct`          _( fast-puzzle-board -- )_
      - destructor
      - release all memory allocated
  * `max-board-index@`  _( fast-puzzle-board -- uindex )_
      - return the max board index address or simply the total voxel count x*y*z
  * `output-board`      _( fast-puzzle-board -- )_
      - terminal display
  * `board-pieces@`     _( fast-puzzle-board -- uquantity )_
    - return current board piece quantity
  * `max-board-pieces@` _( fast-puzzle-board -- uquantity )_
    - return quantity of pieces to solve this board puzzle
  * `board-dims@`       _( fast-puzzle-board -- ux uy uz )_
    - return the board dimensions
  * `board-piece?`      _( uref-piece fast-puzzle-board -- nflag )_
    - test if uref-piece can be placed in current board
    - nflag is true if uref-piece can be placed on the current board
    - nflag is false if uref-piece can not be placed on current board
  * `board-piece!`      _( uref-piece fast-puzzle-board -- nflag )_
    - test if uref-piece can be placed in current board
    - nflag is true if uref-piece can be placed on the current board
    - nflag is false if uref-piece can not be placed on current board
  * `nboard-piece@`     _( uindex fast-puzzle-board -- uref-piece )_
    - get uref-piece from board piece list at uindex location
  * `remove-last-piece` _( uref-piece fast-puzzle-board -- )_
    - remove last piece put on this board
  * `clear-board`       _( fast-puzzle-board -- )_
    - empty board of its pieces but keep the internal references to pieces so construct does not need to be used
  * `serialize-data@`   _( fast-puzzle-board -- nstrings )_
    - return nstrings that contain data to serialize this object
  * `serialize-data!`   _( nstrings fast-puzzle-board -- )_
    - nstrings contains serialized data to restore this object
  * `print`            _( fast-puzzle-board -- )_
    - print stuff for testing

* ### chain-ref
  chain-ref.fs
  * `construct`       _( upieces chain-ref -- )_
    - constructor
  * `destruct`        _( chain-ref -- )_
    - destructor
  * `next-chain@`     _( uref chain-ref -- unext-chain nflag )_
    - retrieve next chain reference for given uref piece
    - unext-chain is a piece that can chain to uref piece
    - nflag is true when the list of possible chains for uref is at the end and will reset for next retrieval to be at beginning of list
  * `chain-quantity@` _( uref chain-ref -- uchain-quantity )_
    - return chain quantity for given uref piece
  * `serialize-data@` _( chain-ref -- nstrings )_
    - return nstrings that contain data to serialize this object
  * `serialize-data!` _( nstrings chain-ref -- )_
    - nstrings contains serialized data to restore this object
