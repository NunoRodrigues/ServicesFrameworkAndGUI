@ECHO OFF
:Loop
IF "%1"=="" GOTO Continue

djpeg -greyscale -dither none %1 temp.pnm

gocr049 -i temp.pnm -o temp.ocr

SHIFT
GOTO Loop
:Continue

