@ECHO OFF

python clean.py
python configure.py
IF ERRORLEVEL 1 GOTO :EOF
CALL build_exe.bat
CALL build_dist.bat
