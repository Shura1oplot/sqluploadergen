import sys
import os
from cx_Freeze import setup, Executable


if sys.version_info[:2] != (3, 8):
    raise Exception("Python 3.8 required!")


build_exe_options = {
    "build_exe": os.path.abspath("build_exe"),
    "packages": [],
    "excludes": ["tkinter", "distutils"],
    "include_files": ["assets", "example"],
}

target = Executable(
    script="sqluploadergen.py",
    base="Console",
)

setup(
    name="sqluploadergen",
    version="0.1.0",
    description="SQL Stream Uploader Generator",
    options={"build_exe": build_exe_options},
    executables=[target, ],
)
