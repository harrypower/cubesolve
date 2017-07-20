require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/stringobj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]
[ifundef] serialize
  interface
    selector serialize-data@ ( object-name -- nstrings ) \ nstrings is all the data from object-name to store in a strings object
    selector serialize-data! ( nstrings object-name -- ) \ nstrings contains all the data to be restored to object-name
  end-interface serialize
[endif]

object class
  destruction implementation  \ ( save-instance-data -- )
  serialize implementation
  protected
  inst-value save$
  inst-value numberbuffer$
  m: ( ns save-instance-data -- caddr u ) \ convert ns to string
    s>d swap over dabs <<# #s rot sign #> #>> numberbuffer$ !$ numberbuffer$ @$ ;m method #sto$
  m: ( nclass caddr u save-instance-data -- xt ) \ caddr u string is an instance data name and returns its xt
    \ xt is false if caddr u string does not match any instance data names or method names
    \ this works by using a defered name that is later assigned this class name
    { nclass caddr u }
    caddr u find-name false = if
      nclass push-order
      caddr u find-name dup false = if drop false else name>int then
      nclass drop-order
    else
      caddr u find-name name>int
    then ;m method $>xt
  m: ( nxt save-instance-data -- caddr u ) \ from the xt of an instance data name return the caddr u string of that named instance data
    \ caddr u is valid if xt is an instance data or method name
    >name dup false = if 0 0 else name>string then ;m method xt>$
  m: ( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-value named in string caddr u
    this $>xt dup false <> if <to-inst> else 2drop then ;m method #$>value
  m: ( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-var named in string caddr u
    this $>xt dup false <> if execute ! else 2drop then ;m method #$>var
  m: ( nclass caddr u save-instance-data -- ) \ caddr u is a method to be executed
    this $>xt dup false <> if this swap execute else 2drop then ;m method $->method
  m: ( xt save-instance-data -- ) \ saves the name string of xt by getting the nt first name to save$
    this xt>$ save$ [bind] strings !$x ;m method do-save-name
  m: ( xt save-instance-data -- ) \ saves the instance value referenced by xt to save$
    dup this do-save-name
    execute this #sto$ save$ [bind] strings !$X ;m method do-save-inst-value
  m: ( xt save-instance-data -- ) \ saves the instance var referenced by xt to save$
    dup this do-save-name
    execute @ this #sto$ save$ [bind] strings !$x ;m method do-save-inst-var
  m: ( nnumber save-instance-data -- ) \ saves nnumber to save$ - note this is a cell wide number
    this #sto$ save$ [bind] strings !$x ;m method do-save-nnumber
  m: ( save-instance-data -- dnumber nflag ) \ retrieve string number from save$
    save$ [bind] strings @$x s>number? ;m method do-retrieve-dnumber
  m: ( save-instance-data -- caddr u dnumber nflag ) \ retrieve string name and string number from save$
    save$ [bind] strings @$x this do-retrieve-dnumber ;m  method do-retrieve-data
  m: ( nclass save-instance-data -- ) \ restores instance var from save$
    { nclass } this do-retrieve-data
    true = if
      d>s rot rot nclass rot rot this #$>var
    else 2drop 2drop then ;m method do-retrieve-inst-var
  m: ( nclass save-instance-data -- ) \ restores instance value from save$
    { nclass } this do-retrieve-data
    true = if
      d>s rot rot nclass rot rot this #$>value
    else 2drop 2drop then ;m method do-retrieve-inst-value
  public
  m: ( save-instance-data -- ) \ constructor
    strings heap-new [to-inst] save$
    string heap-new [to-inst] numberbuffer$
  ;m overrides construct
  m: ( save-instance-data -- ) \ destructor
    save$ [bind] strings destruct
    numberbuffer$ [bind] string destruct
  ;m overrides destruct
  m: ( save-instance-data -- nstrings )
    \ basicaly this method needs to be custom for each object using it
    \ this method needs to use the methods provided to save data into save$ refered to strings object
    \ save$ \ this will return the nstrings required
  ;m overrides serialize-data@
  m: ( nstrings save-instance-data -- )
    \ basicaly this method needs to be custom for each object using it
    \ this method needs to retrieve the data in nstrings to restore the objects data
    \ the data could be broken into parts that save inst-value and inst-var and other data
    \ you could do this by having a method that is each catagorie
    \ this method name could be stored in the save-data method code so all the you need to do here is retrieve that method name and execute it
    \ >> this do-retrieve-data true = if d>s rot rot -defered-object-name rot rot this $->method else 2drop 2drop abort" restore data incorrect!" then
    \ the above line of code is an example of retrieving the name and a number from save$ data and that name is executed and the number could be used as an index of saved items
    \ save$ [bind] strings copy$s \ saves the nstrings object data to be used for retrieval in this method
  ;m overrides serialize-data!
end-class save-instance-data

\ ************************************************************************************************************************************************
\\\

interface
  selector retrieve-some-stuff-test ( atest -- )
  selector save-some-stuff-test ( atest -- )
end-interface save-recall

defer -atest
save-instance-data class
  destruction implementation  ( atest -- )
  save-recall implementation
  protected
  cell% inst-var somevara
  public
  m: ( atest -- ) \ constructor
    this [parent] construct
    923 somevara !
  ;m overrides construct
  m: ( atest -- ) \ destructor
    this [parent]  destruct
  ;m overrides destruct
  m: ( atest -- )
    ['] somevara this do-save-inst-var
  ;m overrides save-some-stuff-test
  m: ( atest -- )
    -atest this do-retrieve-inst-var
  ;m overrides retrieve-some-stuff-test
  m: ( atest -- nvar )
    somevara @ ;m method getvara
  m: ( atest -- )
    0 somevara ! ;m method clearvara
end-class atest
' atest is -atest

atest heap-new constant letssee
cr
letssee getvara . ." < should be 923" cr
letssee save-some-stuff-test ." saved it " cr
letssee clearvara ." cleard it!" cr
letssee getvara . ." < should be 0" cr
letssee retrieve-some-stuff-test ." retrieved it" cr
letssee getvara . ." < should be 923" cr

defer -btest
atest class
  destruction implementation  \ ( btest -- )
  save-recall implementation
  protected
  cell% inst-var somevarb
  inst-value somevaluea
  public
  m: ( btest -- ) \ constructor
    this [parent] construct
    8234 somevarb !
    999 [to-inst] somevaluea
  ;m overrides construct
  m: ( btest -- ) \ destructor
    this [parent]  destruct
  ;m overrides destruct
  m: ( btest -- )
    this [parent] save-some-stuff-test
    ['] somevarb this do-save-inst-var
    ['] somevaluea this do-save-inst-value
  ;m overrides save-some-stuff-test
  m: ( btest -- )
    this [parent] retrieve-some-stuff-test
    -btest this do-retrieve-inst-var
    -btest this do-retrieve-inst-value
  ;m overrides retrieve-some-stuff-test
  m: ( btest -- nvar )
    somevarb @ ;m method getvarb
  m: ( btest -- nvalue )
    somevaluea ;m method getvaluea
  m: ( btest -- )
    0 somevarb ! ;m method clearvarb
  m: ( btest -- )
    0 [to-inst] somevaluea ;m method clearvaluea
  m: ( btest -- )
    save$ ;m method getsave$
  m: ( nnumber btest -- )
    cr ." method execution from data is working!" cr
    . ." <- number retrieved! should be 23459!" cr
    this do-retrieve-dnumber true = if d>s . else ." bad number!" 2drop then
    ." < this is a test number after method and number retrieve and should be 555!" cr
  ;m method test-method-execution
  m: ( btest -- )
    this do-retrieve-data
    true = if d>s rot rot -btest rot rot this $->method else 2drop 2drop ." method execution not working!" then
  ;m method retrieve-method-number
  m: ( btest -- ) \ save a method and a number
    save$ [bind] strings destruct
    save$ [bind] strings construct
    ['] test-method-execution this do-save-name 23459 this do-save-nnumber
    555 this do-save-nnumber
  ;m method save-method-number

end-class btest
' btest is -btest

btest heap-new constant nexttest
cr

nexttest getvarb . ." < b should be 8234" cr
nexttest getvara . ." < a should be 923" cr
nexttest getvaluea . ." < avalue should be 999" cr
nexttest save-some-stuff-test ." saved it " cr
nexttest clearvarb ." b cleared it!" cr
nexttest clearvara ." a cleared it!" cr
nexttest clearvaluea ." avalue cleard it!" cr
nexttest getvarb . ." < b should be 0" cr
nexttest getvara . ." < a should be 0" cr
nexttest getvaluea . ." < avalue should be 0" cr
nexttest retrieve-some-stuff-test ." retrieved it" cr
nexttest getvarb . ." < should be 8234" cr
nexttest getvara . ." < should be 923" cr
nexttest getvaluea . ." < avalue should be 999" cr

nexttest save-method-number
nexttest retrieve-method-number
