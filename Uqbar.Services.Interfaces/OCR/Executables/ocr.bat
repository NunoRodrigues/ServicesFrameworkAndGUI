@ECHO OFF
IF "%1"=="" GOTO EndOfFile

del tmp.pnm
del tmp.ocr

djpeg -greyscale -dither none %1 tmp.pnm

gocr049 -i tmp.pnm -o tmp.ocr

type tmp.ocr

:EndOfFile

