#-------------------------------------------------------------------------------
# Name:        clean SVN folder
# Purpose:
#
# Author:      healer.kx
#
# Created:     07-10-2010
# Copyright:   (c) Administrator 2010
# Licence:     <your licence>
#-------------------------------------------------------------------------------
#!/usr/bin/python
#coding=UTF-8
#FileName: clearsvn.py

# The code need refactor
import getopt, os, sys, shutil
def usage():
    print """Usage: clear.py dir_path [svn|cvs]"""


def clearfolders(rootdir, folder_name = ".svn"):
    for (path, dirs, files) in os.walk(rootdir):
        if folder_name in dirs:
            shutil.rmtree(os.path.join(path, folder_name))
            print 'Delete %s/.svn or /.cvs' % path


if __name__ == "__main__":
    try:
        opts, args = getopt.getopt(sys.argv[1:], '', [])
    except getopt.GetoptError:
        usage()
        sys.exit()
    if not args:
        usage()
        sys.exit()
    else:
        print 'Begin:---------------------------------------------'
        for arg in args:
            clearfolders(arg)
        print 'End!-----------------------------------------------'