@ECHO OFF

CALL config.bat

IF EXIST build_exe RMDIR /S /Q build_exe
python cx_setup.py build_exe
python cx_clean.py

CALL "%VCVARS32%"
REN build_exe "sqluploadergen-%VERSION%"
