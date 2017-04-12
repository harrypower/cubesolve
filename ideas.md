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

* Next additions or bugs to work on
  * start on the ideas for using the list generated in make-all-pieces as this list is the total valid pieces for the puzzle solution!
  * maybe a new object for the solution or maybe just some words putting the parts together to solve the puzzle from the make-all-pieces list!
  * make a pair object a triad object and a five piece grouping object ... the idea here is to find all the total five piece groupings list that is possible and then to solve the puzzle simply five five piece groupings need to be put together for the final solution!  So if a list of all the five piece groups was made that would be used for the final solution!
  * storage ideas
    ```
    struct
      cell% field pair-a
      cell% field pair-b
    end-struct pieces-pair%
    struct
      cell% field three-a
      cell% field three-b
      cell% field three-c
    end-struct pieces-three%
    struct
      cell% field five-a
      cell% field five-b
      cell% field five-c
      cell% field five-d
      cell% field five-e
    end-struct pieces-five%  
    ```
  * the struff stored in these fields are the reference index numbers of the pieces found will allpieces.fs object and put into the piece-array.fs object.
  * ok so make object to find all the pairs and store as pieces-pair% above .. link list will work i think here because when used random pair seeking is not done rather starting at first pair and then seeking to next pair is what will happen.
  * Then from pairs list find the pieces-three% list or what pieces go with the pairs list to make three that do not intersect.  again a link list i think will be the way to go.
  * i need to know if making these pairs, three's and fives are possible in memory .... aka will they grow too large or will the intersections be significant that this is doable in memory?
