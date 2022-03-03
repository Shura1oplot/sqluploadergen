@ECHO OFF

CALL config.bat

SET "DIST=releases\sqluploadergen_%VERSION%.7z"
IF EXIST "%DIST%" DEL "%DIST%"
assets\tools\7za a "%DIST%" "sqluploadergen-%VERSION%"
