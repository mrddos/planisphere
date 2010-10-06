#-------------------------------------------------------------------------------
# Name:        Batch Rename
# Purpose:
#
# Author:      Administrator
#
# Created:     01-10-2010
# Copyright:   (c) Administrator 2010
# Licence:     <your licence>
#-------------------------------------------------------------------------------
#!/usr/bin/env python




import shutil
import sys
import os


def compdir(dir1, dir2):
    pass
    print 1

def main():

    argv_len = len(sys.argv)

    if (argv_len < 3):
        return
    compdir(sys.argv[1], sys.argv[2])


if __name__ == '__main__':
    main()