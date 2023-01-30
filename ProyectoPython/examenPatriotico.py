
def programa():
    print('\r\n')
    print('\r\n************** Este es el examen final de ciencias sociales ****************\r\n')
    print('Debes colocar tu respuesta sobre los días patrioticos\r\n')

    calificacion = int(0)

    print('Haremos unas preguntas, en el cual debes responder SI o NO\r\n')
    print('\r\n\n')
    pregunta1 = input('Pregunta 1: ¿Ricardo Miró escribió el himno Nacional de Panamá?\r\n')
    if(pregunta1 == 'NO'):
        calificacion += 1
    print('\r\n')

    pregunta2 = input('Pregunta 2: ¿Los símbolos patrios son: el escudo, el himno y la bandera?\r\n')
    if(pregunta2 == 'SI'):
        calificacion += 1
    print('\r\n')

    pregunta3 = input('Pregunta 3: ¿El día 3 de noviembre se celebra la separación de Panamá de Colombia?\r\n')
    if(pregunta3 == 'SI'):
        calificacion += 1
    print('\r\n')

    print(f'Tu puntaje final en la calificación es: {calificacion} de 3')

    if(calificacion == 3):
        print('Excelente lo has logrado!')
    elif(calificacion == 2):
        print('Bien, intentalo nuevamente!')
    elif(calificacion == 1):
        print('uff muy bajo!')
    else:
        print('carita triste!')        

programa()         
