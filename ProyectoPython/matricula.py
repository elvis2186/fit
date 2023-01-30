registro = {}
registro['estudiantes'] = []

def programa():
  print('\r\nmatrícula')
  
  nombreRegistro = input('Escriba el nombre del registro: \r\n')
  registro['nombre'] = nombreRegistro
  agregar_estudiante()
  print(registro)

def agregar_estudiante():
    estudiante = {}
    registroActual = registro['nombre']
    nombreEstudiante = input(f'agregue el nombre del estudiante para el registro {registroActual}\r\n')
    edadEstudiante = input(f'agregue la edad del estudiante para el registro {registroActual}\r\n')
    montoEstudiante = input(f'agregue el monto de la matricula del estudiante para el registro {registroActual}\r\n')
    
    estudiante['nombre'] = nombreEstudiante
    estudiante['edad'] = edadEstudiante
    estudiante['monto'] = montoEstudiante

    registro['estudiantes'].append(estudiante)

programa()
