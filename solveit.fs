require newpiece-obj.fs

: #to$ ( n -- caddr u1 ) \ convert n to string
    s>d
    swap over dabs
    <<# #s rot sign #> #>> ;

    
