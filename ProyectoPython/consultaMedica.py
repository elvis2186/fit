citas = {}
citas['pacientes'] = []

def app():
    print('\r\nRegistro de citas médicas\r\n')
    agregar_paciente()

    descripcion = input('Descripcion de la atención:\r\n')
    citas['descripcion'] = descripcion
    fecha_cita = input('Fecha de la cita:\r\n')
    citas['fecha'] = fecha_cita
    doctor = input('Doctor de la atención:\r\n')
    citas['doctor'] = doctor

    doctor = citas['doctor']
    fecha = citas['fecha']
    print(f'Cita con el doctor: {doctor} para el día: {fecha} \r\n')

    print(citas)

def agregar_paciente():
 paciente = {}

 nombre = input('Nombre del paciente\r\n')
 paciente['nombre'] = nombre
 correo = input('Correo del paciente:\r\n')
 paciente['correo'] = correo
 cedula = input('Cédula del paciente: \r\n')
 paciente['cedula'] = cedula

 citas['pacientes'].append(paciente)

 print('\r\n')
app()