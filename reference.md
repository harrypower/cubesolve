## This is somewhat of an outline of objects and or data structures and methods needed to go forward with general cube puzzle solver!

* ### voxel
  newpieces.fs
  * construct ___ ( voxel -- ) \ construct voxel with 0 0 0 as voxel coordinates
  * voxel!___( ux uy uz voxel -- ) \ store the voxel coordinates
  * voxel@      ( voxel -- ux uy uz ) \ retrieve voxel coordinates
  * intersect?  ( uvoxel voxel -- nflag ) \ nflag is true if uvoxel is intersecting with voxel nflag is false if not intersecting

* ### piece
  newpieces.fs
  * #### construct        ( piece -- ) \ construct piece object
  * #### destruct         ( piece -- ) \ destruct and free data in object
  * #### add-voxel        ( ux uy uz piece -- ) \ add a voxel to this piece with ux uy uz address
  * #### get-voxel-object ( uindex piece -- uvoxel ) \ return a voxel object at uindex
  * #### get-voxel        ( uindex piece -- ux uy uz ) \ retrieve voxel data from uindex voxel in this piece
  * #### voxel-quantity@  ( piece -- usize ) \ return voxel quantity
  * #### intersect?       ( upiece piece -- nflag ) \ test for intersection of upiece with this piece on any voxel
  * #### same?            ( upiece piece -- nflag ) \ test upiece agains this piece for exact voxels match forward or backward
  * #### copy             ( upiece piece -- ) \ exact copy upiece to this piece

* pieces object
  * will be the base object that manages what constitutes groups of pieces for any grouping purpose
  * will contain piece objects
  * methods to put and get piece objects into a linked list to represent a group of pieces for many reasons
  * methods to allow indexed piece get

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
