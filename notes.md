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
* figure out the basic objects that get instantiated and the data they contain to move forward with this project
* understand how fast-puzzle-board is different then puzzleboard and decide if it is the direction


### New idea to incorporate to this project
* in the past version i concentrated on making reference list and using that fast list to brute force solution by filling the board holes.  My new idea is to use this reference list and add other lists to allow a piece chain type solution.
* this piece chain idea is that each piece from the reference list has two ends.  The ends of each piece has a place where another piece can be adjacent to the reference piece and if i chain together one at a time a piece then after 24 successful piece chains plus the first piece the puzzle is solved.   
* I can do this for example by starting with a reference piece and that is the center then place a new chain piece on each end of the reference and then place another piece on the ends of these new pieces and so on until a solution is found.  This would mean that the center piece plus 12 pieces on each side would be the solution!
* so I would need to make a list of each reference piece and its end chain pieces that are allowed with it.  I think the fast-puzzle-board object could be used to make this list quickly.  Then from there it is a matter of brute force going through the list of chain pieces.
* my thinking here is this method reduces the combinations of testing done for the brute force solution because i am focusing on complete pieces rather then filling the board holes.  Rather i would be placing a piece at a time and each piece placed reduces the next ones to test for significantly!
