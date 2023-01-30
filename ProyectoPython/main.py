import os

CARPETA = 'contactos/'
EXTENSION = '.txt'

class Contacto:
  def __init__(self, nombre, telefono, categoria):
    self.nombre = nombre
    self.telefono = telefono
    self.categoria = categoria

def app():
  crear_directorio()
  mostrar_menu()
  
  preguntar = True
  while preguntar:
    opcion = input('seleccione una opcion: \r\n')
    opcion = int(opcion)
    
    if opcion == 1:
      agregar_contacto()
    elif opcion == 2:
      editar_contacto()
    elif opcion == 3:
      mostrar_contacto()
    elif opcion == 4:
      buscar_contacto()
    elif opcion == 5:
      eliminar_contacto()
    elif opcion == 6:
      print('saliendo..')
      preguntar = False
    else:
      print('opcion no valida')

def eliminar_contacto():
  print()
  nombre = input('Seleccione el contacto a eliminar \r\n')
  try:
    os.remove(CARPETA + nombre + EXTENSION)
    print('\r\n elimnaodo\r\n')
  except IOError:
    print('no se pudo eliminar')

def buscar_contacto():
  nombre = input('Seleccione el contacto a buscar \r\n')

  try:
    with open(CARPETA  + nombre + EXTENSION) as contacto:
        print('\r\ninformación del contacto \r\n')
        for linea in contacto:
            print(linea.rstrip())
        print('\r\n')
  except IOError:
    print('El archivo no existe')
    print(IOError)

def mostrar_contacto():
  print('')
  archivos = os.listdir(CARPETA)

  archivos_txt = [i for i in archivos if i.endswith(EXTENSION)]

  for archivo in archivos_txt:
    with open(CARPETA + archivo) as contacto:
        for linea in contacto:
            print(linea.rstrip())
        print('\r\n')    

def agregar_contacto():
  print('escribe los datoa para agregar un nuevo contacto')
  nombre_contacto = input('Nombre del contacto:\r\n')
  
  existe = existe_contacto(nombre_contacto)
  
  if not existe:
  
    with open(CARPETA + nombre_contacto + EXTENSION, 'w') as archivo:
      
      telefono_contactto = input('Agrega el telefono:\r\n')
      categoria_contacto = input('categoria del contacto:\r\n')
      
      contacto = Contacto(nombre_contacto, telefono_contactto, categoria_contacto)
      
      archivo.write('nombre: '+ contacto.nombre + '\r\n')
      archivo.write('telefono: '+ contacto.telefono + '\r\n')
      archivo.write('categoria: '+ contacto.categoria + '\r\n')
      
      print('\r\n contacto creado correcatmente \r\n')

      
  else:
    print('\r\n ese contacto ya existe\r\n')
  
def editar_contacto():
  print('Escribe nombre a actualizar')
  nombre_anterior = input('Nombre del contacto que qiere editar:\r\n')
  existe = existe_contacto(nombre_anterior)
  
  if existe:
    with open(CARPETA + nombre_anterior +EXTENSION, 'w') as archivo:

        nombre_contacto = input('Nuevo nombre del contacto:\r\n')
        telefono_contactto = input('Agrega el nuevo telefono:\r\n')
        categoria_contacto = input('nueva categoria del contacto:\r\n')

        contacto = Contacto(nombre_contacto, telefono_contactto,categoria_contacto)

        archivo.write('nombre: '+ contacto.nombre + '\r\n')
        archivo.write('telefono: '+ contacto.telefono + '\r\n')
        archivo.write('categoria: '+ contacto.categoria + '\r\n')

    os.rename(CARPETA + nombre_anterior + EXTENSION, CARPETA + nombre_contacto + EXTENSION)

    print('contacto editado correctamente\r\n')
    
  else:
    print('el contacto no existe\r\n') 
  
def existe_contacto(nombre_contacto):
  return os.path.isfile(CARPETA + nombre_contacto + EXTENSION)
  
def mostrar_menu():
  print('seleccione del meno lo que desea hacer:')
  print('1) Agregar nuevo contacto')
  print('2) Editar contacto')
  print('3) ver contactos')
  print('4) buscar contacto')
  print('5) eliminar contacto')
  print('6) salir del programa')
  
def crear_directorio():
  if not os.path.exists(CARPETA):
  	os.makedirs(CARPETA)

app()