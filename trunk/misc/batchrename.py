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

def help():
    """
    """
    pass

def rename(src, src_pattern, dest_pattern):
    name = "1"
    return src

def batch_rename():
    pass

def batch_rename_pattern(src_pattern, dest_pattern):
    s = "E:\\Pics\\general"
    d = "E:\\Pics\\general2"

    if (not os.path.isdir(d)):
        os.mkdir(d)

    files = os.listdir(s)
    for file in files:
        if (file.endswith('.jpg') or file.endswith('.JPG')):
            new_name = rename(file, src_pattern, dest_pattern)
            shutil.copy(s + "\\" + file, d + "\\" + new_name)



def main():

    argv_len = len(sys.argv)
    if (argv_len == 1):
        batch_rename_pattern(None, None)
    elif (argv_len == 2):
        batch_rename(sys.argv[1], None)
    elif (argv_len == 3):
        batch_rename(sys.argv[1], sys.argv[2])


if __name__ == '__main__':
    main()