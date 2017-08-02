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

  #### private methods that are normaly used directly by inherited class to save and restore data into save$ strings object
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


* ### board
  puzzleboard.fs
  * `construct`             _( board -- )_
  * `destruct`              _( board -- )_
  * `set-board-dims`        _( ux uy uz board -- )_
      - set max board size and allocate the board-array memory
  * `get-board-dims`        _( board -- ux-max uy-max uz-max )_
      - get dimensions of this board
  * `board-piece-quantity@` _( board -- uquantity )_
      - return how many pieces are currently on the board
  * `voxel-on-board?`       _( ux uy uz board -- nflag )_
      - ux uy uz is a voxel to test if it can be placed on an empty board
  * `piece-on-board?`       _( upiece board -- nflag )_
      - test if upiece can be placed on an empty board nflag is true if piece can be placed false if not
  * `piece-on-this-board?`  _( upiece board -- nflag )_
      - test if upiece could be placed on the current populated board
  * `place-piece-on-board`  _( upiece board -- nflag )_
      - place upiece on the current board if it can be placed without intersecting with other pieces
  * `nget-board-piece`      _( uindex board -- upiece )_
      - retrieve uindex piece from this board in the form of a piece object
  * `see-board`             _( board -- )_
      - crude terminal board display

* ### piece-array
  piece-array.fs
  * `construct` _( upieces piece-array -- )_
      - construct the array from the contents of upieces!  Note the size is fixed at construct time!
      - construct the intersect array of reference pieces.
  * `destruct` _( piece-array -- )_
  * `upiece@` _( uindex piece-array -- upiece)_
      - retrieve upiece from array at uindex location
  * `fast-intersect?` _( uindex0 uindex1 piece-array -- nflag )_
      - return nflag from intersect-array to get fast intersect detection for uindex0 and uindex1 pieces
      - nflag is true if an intersection between uindex0 and uindex1 is found
      - nflag is false if no intersection is found
  * `quantity@` _( piece-array -- nquantity )_
      - return the array size

* translation and orientation object
  * will have the job of taking a piece and creating all the pieces that are derived from the translations and rotations of the piece in the board space
  * will contain piece and pieces objects and voxel objects
  * will use board object to test piece voxel validity on the board
  * may contain board objects to facilitate piece or voxel board placement testing of orientations translations and or rotations
  * methods to manipulate a piece as a translation and rotation to make other piece orientations

* group list object
  * will be a general purpose list to group numbers or index values for example.
  * will be configurable to handle any size group with any amount of lists of this group!
  * to do this the link list object should be used as the list will be added to in quantity's that are unknown at time of use.
  * method to store new groups
  * method to retrieve an indexed group values
  * method to retrieve the current size of the list of groups.

* hole-array-piece-list object
  * this takes ref-piece-array object and puzzle-board object to create an array of pieces in a list that are representing holes or voxels of the board for this puzzle.
  * methods for getting the size of the list in each hole.
  * methods for getting the next reference piece in a given hole

* solution object
  * will take ref-piece-array object and hole-array-piece-list object and puzzle-board
  * will store a working puzzle board array to fast access pieces on board in a reference form
  * will use the fast intersect method from ref-piece-array to solve intersections.
  * will mechanize the sequential hole filling to solve puzzle

* Next additions or bugs to work on
  * convert all the objects to use the save-instance-data object to allow complete data saving of the objects data!
