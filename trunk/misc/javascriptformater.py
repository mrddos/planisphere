#-------------------------------------------------------------------------------
# Name:        JavaScriptFormater
# Purpose:
#
# Author:      Administrator
#
# Created:     01-10-2010
# Copyright:   (c) Administrator 2010
# Licence:     <your licence>
#-------------------------------------------------------------------------------
#!/usr/bin/env python

import sys
import string



def parse_file(content):
    print(content)


def test():
    pass


def do_format(content):
    s = 0
    line = ""

    lines = []
    indent = 0
    for ch in content:
        if (ch == ";"):
            line += ";"
            lines.append("\t" * indent + line)
            line = ""
            continue
        elif (ch == "{"):
            lines.append("\t" * indent + line)
            line = "{"
            lines.append("\t" * indent + line)
            indent += 1;
            line = ""
            continue
        elif (ch == "}"):
            lines.append("\t" * indent + line)
            line = "}"
            indent -= 1;
            lines.append("\t" * indent + line)

            line = ""
            continue

        line += ch

    return lines




def readfile(file_name):
    file = open(file_name, 'r')
    lines = file.readlines()
    content = ""
    for line in lines:
        content += line
    return content

def download(url):
    pass

def print_lines(lines):
    for line in lines:
        print line

def format(src, dest):
    content = ""
    if (src.startswith('http://')):
        content = download(src)
    elif (src[1] == ':'):
        content = readfile(src)

    lines = do_format(content)
    print_lines(lines)






def main(argv):
    argv_len = len(argv)
    if (argv_len == 1):
        return
    elif (argv_len == 2):
        format(argv[1], None)
    elif (argv_len == 3):
        format(argv[1], argv[2])




if __name__ == '__main__':
    #argv = sys.argv
    argv = ['This.Program.File.Name', 'E:\\share\\RequestVacationPanel.js']
    main(argv)