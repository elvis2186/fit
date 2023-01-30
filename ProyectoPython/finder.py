import os
import re

CARPETA = 'HANG_FIRE_L/'
EXTENSION = '.txt'

jobsFail = []

def main():
    print('Iniciando buscador')
    with open('LogFile_2601_08117.txt') as file:
        print('leyendo del archivo')        
        for line in file:
            line_fail = line.strip()          
            try:
             job_found = re.search('Failed to process the job \'(.+?)\': an exception occurred',line_fail).group(1)
             test_Only_Numbers = re.match('[0-9]', job_found)
             if test_Only_Numbers:
                jobsFail.append(job_found)
            except AttributeError:
                pass

    with open(CARPETA + 'job' + EXTENSION, 'w') as archivo:
        for job_id in jobsFail:
            archivo.write('\''+job_id+'\',' + '\r')
    print('termina buscador')        
main()    