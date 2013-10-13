PROCESSOR 16F877
 
         MOVLW   00
         TRIS    06
         
         TRI
 
         CLRF    06
again    INCF    06
         GOTO    again
 
         END