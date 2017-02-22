# This is somewhat of an outline of objects and or data structures and methods needed to go forward with general cube puzzle solver!

* voxel object
  * will be the base element used by the piece, pieces, board and translation orientation objects
  * will manage all the things important to voxels
  * contains x y and z
  * methods to get x y z data in and out

* piece object
  * will be the base object that manage what constitutes a piece or a collection of voxels
  * will contain voxel objects
  * methods to put and get voxel objects into this piece and out of this piece
  * method to get quantity of voxels in this piece 

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
  * may contain board objects to facilitate piece or voxel board placement testing of orientations translations and or rotations
  * methods to manipulate a piece as a translation and rotation to make other piece orientations
