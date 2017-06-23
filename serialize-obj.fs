require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/stringobj.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]


object class
  destruction implementation  \ ( save-instance-data -- )
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
    >name dup false = if 0 0 else name>string then
  ;m method xt>$
  m: ( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-value named in string caddr u
    this $>xt dup false <> if <to-inst> else 2drop then
  ;m method #$>value
  m: ( unumber nclass caddr u save-instance-data -- ) \ put unumber into the inst-var named in string caddr u
    this $>xt dup false <> if execute ! else 2drop then
  ;m method #$>var
  m: ( nclass caddr u save-instance-data -- ) \ caddr u is a method to be executed
    this $>xt dup false <> if execute else 2drop then
  ;m method $->method
  public
  m: ( save-instance-data -- ) \ constructor
    strings heap-new [to-inst] save$
    string heap-new [to-inst] numberbuffer$
  ;m overrides construct
  m: ( save-instance-data -- ) \ destructor
    save$ [bind] strings destruct
    numberbuffer$ [bind] string destruct
  ;m overrides destruct
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
    ['] somevara this xt>$ save$ [bind] strings !$x
    somevara @ this #sto$ save$ [bind] strings !$x
  ;m overrides save-some-stuff-test
  m: ( atest -- )
    save$ [bind] strings @$x
    save$ [bind] strings @$x s>number? drop d>s
    rot rot -atest rot rot
    this #$>var
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
  destruction implementation  \ ( atest -- )
  save-recall implementation
  protected
  cell% inst-var somevarb
  public
  m: ( atest -- ) \ constructor
    this [parent] construct
    8234 somevarb !
  ;m overrides construct
  m: ( atest -- ) \ destructor
    this [parent]  destruct
  ;m overrides destruct
  m: ( atest -- )
    this [parent] save-some-stuff-test
    ['] somevarb this xt>$ save$ [bind] strings !$x
    somevarb @ this #sto$ save$ [bind] strings !$x
  ;m overrides save-some-stuff-test
  m: ( atest -- )
    this [parent] retrieve-some-stuff-test
    save$ [bind] strings @$x
    save$ [bind] strings @$x s>number? drop d>s
    rot rot -btest rot rot
    this #$>var
  ;m overrides retrieve-some-stuff-test
  m: ( atest -- nvar )
    somevarb @ ;m method getvarb
  m: ( atest -- )
    0 somevarb ! ;m method clearvarb
end-class btest
' btest is -btest

btest heap-new constant nexttest
cr

nexttest getvarb . ." < b should be 8234" cr
nexttest getvara . ." < a should be 923" cr
nexttest save-some-stuff-test ." saved it " cr
nexttest clearvarb ." b cleared it!" cr
nexttest clearvara ." a cleared it!" cr
nexttest getvarb . ." < b should be 0" cr
nexttest getvara . ." < a should be 0" cr
nexttest retrieve-some-stuff-test ." retrieved it" cr
nexttest getvarb . ." < should be 8234" cr
nexttest getvara . ." < should be 923" cr
