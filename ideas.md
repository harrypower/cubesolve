# This is somewhat of an outline of objects and or data structures and methods needed to go forward with general cube puzzle solver!

* voxel object
  * will be the base element used by the piece, pieces, board and translation orientation objects
  * will manage all the things important to voxels
  * contains x y and z
  * methods to get x y z data in and out
  * methods to test this voxel to a given voxel for intersecting

* piece object
  * will be the base object that manage what constitutes a piece or a collection of voxels
  * will contain voxel objects
  * methods to put and get voxel objects into this piece and out of this piece
  * method to get quantity of voxels in this piece
  * methods to test if this piece intersect with another given piece

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

* Next additions or bugs to work on
  * working on the constructor for the below referenced object !
  * still some bug in multi-cell-array object or not.. mmmm !
  * make an object that groups the reference list into all pieces that fill voxel holes ... list is ok because each voxel hole will have a varing list of pieces that fill it. so this will be structured as an array of lists.  the array will be the size of the reference pieces found.  the array will contain the list object that contains lists of the reference pieces that can be used to fill the voxel hole.
  * solution for now will be just fill the holes.  Use the above mentioned object to cycle through the list of pieces for the list of holes.
  * use the fast intersect method to find list of pieces to fill the array of list up with.
