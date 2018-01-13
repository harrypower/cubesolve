## Notes for cubesolve!

### Files looked at sofar
* newpuzzle.def
* newpieces.fs
* puzzleboards.fs
* allpieces.fs
* serialize-obj.fs
* piece-array.fs
* fast-puzzle-board.fs

### Things still to do in the above Files
* confirm serialize stuff works properly for each object
* understand how fast-puzzle-board is different then puzzleboard and decide if it is the direction
  #### fast-puzzle-board.fs vs puzzleboard.fs
  * puzzleboard.fs makes board object that uses linked list of pieces.
  * fast-puzzle-board.fs makes fast-puzzle-board object that uses an array of pieces.  
  * fast-puzzle-board.fs file uses a simple number to reference a piece.  This number is the reference to pieces passed to the fast-puzzle-board.fs as a linked list but internaly stored as an array.  The board object from puzzleboard.fs uses a piece object for all its inputs so it is slower because that piece object needs to be tested with other pieces objects internaly that are stored as linked list.  This fast-puzzle-board object because it has the reference internally stored as an array that is faster to access.  This means all the fast-puzzle-board object inputs are all simply this reference number and accessing the piece is faster.  All these methods in fast-puzzle-board need to be fleshed out yet but it is the way to go forward.  There is no need to generate this reference internally because the reference is used externally also so just use the externally generated one passed to it internally.
  * make-all-pieces from allpieces.fs will need to continue to use board from puzzleboard.fs because at first the linked list is the best way to then generate the reference array of pieces used later by fast-puzzle-board.
  * because board is still needed then there is no reason to change allpieces.fs as it currently depends on board so there is no change needed here!
  * So fast-puzzle-board is used in the algorithm for solving the puzzle rather then board object because of its speed.
  * fast-puzzle-board will also be able to display the board and serialize its data. 

### New idea to incorporate to this project
* in the past version i concentrated on making reference list and using that fast list to brute force solution by filling the board holes.  My new idea is to use this reference list and add other lists to allow a piece chain type solution.
* this piece chain idea is that each piece from the reference list has two ends.  The ends of each piece has a place where another piece can be adjacent to the reference piece and if i chain together one at a time a piece then after 24 successful piece chains plus the first piece the puzzle is solved.   
* I can do this for example by starting with a center piece for solution then place a new chain piece on each end of the center piece and then place another piece on the ends of these new pieces and so on until a solution is found.  This would mean that the center piece plus 12 pieces on each side would be the solution!
* so I would need to make a list of each reference piece and its end chain pieces that are allowed with it.  I think the fast-puzzle-board object could be used to make this list quickly.  Then from there it is a matter of brute force going through the list of chain pieces.
* my thinking here is this method reduces the combinations of testing done for the brute force solution because i am focusing on complete pieces rather then filling the board holes.  Rather i would be placing a piece at a time and each piece placed reduces the next ones to test for significantly!
