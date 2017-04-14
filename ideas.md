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

* group list object
  * will be a general purpose list to group numbers or index values for example.
  * will be configurable to handle any size group with any amount of lists of this group!
  * to do this the link list object should be used as the list will be added to in quantity's that are unknown at time of use.
  * method to store new groups
  * method to retrieve an indexed group values
  * method to retrieve the current size of the list of groups.

* Next additions or bugs to work on
  * start on the ideas for using the list generated in make-all-pieces as this list is the total valid pieces for the puzzle solution!
  * maybe a new object for the solution or maybe just some words putting the parts together to solve the puzzle from the make-all-pieces list!
  * take group-list object and use to make a pair list from the make-all-pieces object that contains all the 480 piece combinations.
  * then make another list with pair list to find all the three pieces that work together.
  * then make another list to find all the four pieces that work together.
  * then make another list to find all the five pieces that work together.
  * now with the list of five pieces that work together all that is needed is to find five groups of five that work for the final solution.
  * then from this try to generalize this process to work for other size puzzles with other size pieces as starting point.
  * These steps are an attempt to see if these groups can be placed in memory.  I am not sure how many group lists can be stored in memory so these steps may start to answer this question.
