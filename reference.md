## This is somewhat of an outline of objects and or data structures and methods needed to go forward with general cube puzzle solver!

* ### voxel
  newpieces.fs
  * construct       _( voxel -- ) \ construct voxel with 0 0 0 as voxel coordinates_
  * voxel!          _( ux uy uz voxel -- ) \ store the voxel coordinates_
  * voxel@          _( voxel -- ux uy uz ) \ retrieve voxel coordinates_
  * intersect?      _( uvoxel voxel -- nflag ) \ nflag is true if uvoxel is intersecting with voxel nflag is false if not intersecting_

* ### piece
  newpieces.fs
  * construct         _( piece -- ) \ construct piece object_
  * destruct          _( piece -- ) \ destruct and free data in object_
  * add-voxel         _( ux uy uz piece -- ) \ add a voxel to this piece with ux uy uz address_
  * get-voxel-object  _( uindex piece -- uvoxel ) \ return a voxel object at uindex_
  * get-voxel         _( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel in this piece_
  * voxel-quantity@   _( piece -- usize ) \ return voxel quantity_
  * intersect?        _( upiece piece -- nflag ) \ test for intersection of upiece with this piece on any voxel_
  * same?             _( upiece piece -- nflag ) \ test upiece agains this piece for exact voxels match forward or backward_
  * copy              _( upiece piece -- ) \ exact copy upiece to this piece_
  * serialize-data@   _( piece -- nstrings ) \ to save this piece data_
  * serialize-data!   _( nstrings piece -- ) \ to restore previously saved data_

* ### pieces
  newpieces.fs
  * construct         _( pieces -- ) \ construct pieces object_
  * destruct          _( pieces -- ) \ destruct pieces object_
  * add-a-piece       _( upiece pieces -- ) \ copies contents of upiece object and puts the copied piece object in a-pieces-list_
  * get-a-piece       _( uindex pieces -- upiece ) \ retrieve uindex piece from a-pieces-list_
  * pieces-quantity@  _( pieces -- usize ) \ return the quantity of pieces in this piece list_
  * serialize-data@   _( pieces -- nstrings ) \ to save this list of pieces_
  * serialize-data!   _( nstrings pieces -- ) \ to restore previously saved list of pieces_

* ### save-instance-data
  serialize-obj.fs
  * construct         _( save-instance-data -- ) \ constructor_
  * destruct          _( save-instance-data -- ) \ destructor_
  * serialize-data@   _( save-instance-data -- nstrings ) \ method that is empty and is the suggested name of method for making serialized data_
  * serialize-data!   _( nstrings save-instance-data -- ) \ method that is empty and is the suggested name of the method for retrieving the serialized data in nstrings_
  #### private inst-value not used  
    * save$           _this is the strings object handle containing the serialized data_
  #### private methods that are not normally used directly by inherited class but can be used if understood how they work.
    * #sto$            _( ns save-instance-data -- caddr u ) \ convert ns to string_
    * $>xt             _( nclass caddr u save-instance-data -- xt ) \ caddr u string is an instance data name and returns its xt. nclass is also needed_
    * xt>$             _( nxt save-instance-data -- caddr u ) \ from the xt of an instance data name return the caddr u string of that named instance data_
    * #$>value         _( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-value named in string caddr u_ ... note nclass is needed!_
    * #$>var           _( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-var named in string caddr u ... note nclass is needed!_
    * $->method        _( nclass caddr u save-instance-data -- ) \ caddr u is a method to be executed ... note nclass is needed!_
  #### private methods that are normaly used directly by inherited class to save and restore data into save$ strings object_
    * do-save-name     _( xt save-instance-data -- ) \ saves the name string of xt by getting the nt first name to save$_
    * do-save-inst-value  _( xt save-instance-data -- ) \ saves the instance value referenced by xt to save$_
    * do-save-inst-var    _( xt save-instance-data -- ) \ saves the instance var referenced by xt to save$_
    * do-save-nnumber     _( nnumber save-instance-data -- ) \ saves nnumber to save$ - note this is a cell wide number_
    * do-retrieve-dnumber _( save-instance-data -- dnumber nflag ) \ retrieve string number from save$_
    * do-retrieve-data    _( save-instance-data -- caddr u dnumber nflag ) \ retrieve string name and string number from save$_
    * do-retrieve-inst-var  _( nclass save-instance-data -- ) \ restores instance var from save$_
    * do-retrieve-inst-value  _( nclass save-instance-data -- ) \ restores instance value from save$_

* board object
  * will manage all that the board space needs to be managed
  * will contain an array of the current puzzle board with voxel like addresses of x y and z
  * will contain piece pieces and voxels possibly!
  * methods to set and get the board size
  * methods to set and get pieces that can be placed on the board
  * methods to set and get voxels or a piece on the board
  * methods to display the board at the command line showing both pieces on the board or the voxels that are placed on the board from pieces
  * methods to test a piece or voxel to see if it can be placed on the board

* translation and orientation object
  * will have the job of taking a piece and creating all the pieces that are derived from the translations and rotations of the piece in the board space
  * will contain piece and pieces objects and voxel objects
  * will use board object to test piece voxel validity on the board
  * may contain board objects to facilitate piece or voxel board placement testing of orientations translations and or rotations
  * methods to manipulate a piece as a translation and rotation to make other piece orientations

* piece array object
  * this is a fixed array object that will contain the reference piece array
  * this will allow the total pieces found in the translation and orientation object to be indexed
  * this array and indexing of all the pieces for puzzle board are used in puzzle solution
  * when then array is created it is of fixed size
  * at array creation the pieces list that is to be stored is given to this array and placed in the array at this time.
  * method to retrieve a given indexed piece that is stored already
  * method to retrieve the total size of piece array
  * indexed piece retrieval should be fast not stored in a linked list!

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
