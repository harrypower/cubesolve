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
* *** note multidimentional cell array object does not initalize the array once created.. so ensure when using this object to do that at construct time.****
* confirm serialize stuff works properly for each object.  Newpieces.fs , piece-array.fs, fast-puzzle-board.fs now have working serialize methods.
* Make chain-piece-ref object that will hold an array of double-linked-lists to index the piece chains that are allowed!
* update reference.md for fast-puzzle-board object and remove hole-array-piece-list or at least put it at bottom with note about not using it!   Also put a note about not using board object either!

### New idea to incorporate to this project
* in the past version i concentrated on making reference list and using that fast list to brute force solution by filling the board holes.  My new idea is to use this reference list and add other lists to allow a piece chain type solution.
* this piece chain idea is that each piece from the reference list has two ends.  The ends of each piece has a place where another piece can be adjacent to the reference piece and if i chain together one at a time a piece then after 24 successful piece chains plus the first piece the puzzle is solved.   
* The method is simply start with a piece and then chain to the next piece and keep chaining pieces until i find all 25 pieces!
* The chain pieces list does not need to identify which end it chains to!
* so I would need to make a list of each reference piece and its end chain pieces that are allowed with it.  I think the piece-array object could be used to make this list quickly.  Then from there it is a matter of brute force going through the list of chain pieces.
* my thinking here is this method reduces the combinations of testing done for the brute force solution because i am focusing on complete pieces rather then filling the board holes.  Rather i would be placing a piece at a time and each piece placed reduces the next ones to test for significantly!
* Got the basic chain looping solution working now.  I need to reduce combinations further.  
* First idea is to bring back the hole/voxel piece list.  This will be used as a way to test if there is a blockage in current puzzle solution.  This means if there is after placing a piece a possibility that some hole of voxel can not be solved that means the last piece placed needs to be removed before trying combinations.  This blockage testing takes time but the trade off is it saves time because if a piece is placed and it produces a blockage then that combination does not need to be explored and then retraced.  I think the upshot of this is it will take more time to place the first pieces on puzzle board but the latter pieces that do not need to be tested will save magnitudes of wasted combinational testing time!
* Ok now i have reintroduced the group or assembly idea.  This will simply take the chain reference array and make first a list of all the existing chains that go together in pairs to make a pair group or assembly.  Then the next group is a group of all the pair groups to make a group of groups.  Then from there the next group will be the group of group of groups.  Then from that all i need to do is find three such group of group of groups ( or 8 piece assemblies ) that go together to produce in entirety 24 pieces and find with that set of three assemblies one set that with another piece the solution to the puzzle is solved.  The main issue here is memory to make these lists.    
