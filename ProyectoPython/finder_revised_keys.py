import os
import re

CARPETA = 'HANG_FIRE_L/'
EXTENSION = '.txt'

jobsFail = []
KeysFail = []

def main():
    print('Iniciando buscador')
    print('leyendo del archivo LogFile_2601_08117.txt')                                
    read_reverse_order('LogFile_2601_08117.txt')
    
    with open('bckLogFile_2601_08117.txt') as file:
        for line in file:        
            line_fail = line.strip()                                       
            try:
                test_Only_key_fail = re.search('registros afectados del cuestionario (.+?)$',line_fail).group(1)
                #if test_Only_key_fail not in jobsFail:
                if jobsFail.count(test_Only_key_fail) <= 0:
                 jobsFail.append(test_Only_key_fail)                   
            except AttributeError:
                pass
    
    with open('llave_revisada.txt') as filer:
     for lin in filer:
        key = lin.strip()            
        try:
            if key not in jobsFail:              
             KeysFail.append(key)        
        except AttributeError:
         pass

    print(KeysFail)
    print('termina buscador')

def read_reverse_order(file_name):
    f1 = open("bckLogFile_2601_08117.txt", "w")    
    # Open file in read mode
    with open(file_name, 'r') as read_obj:
        # get all lines in a file as list
        lines = read_obj.readlines()
        lines = [line.strip() for line in lines]
        # reverse the list
        lines = reversed(lines)
        # Return the list with all lines of file in reverse order
        for line in lines:
          f1.write(line+ '\r')
        f1.close()
main()    