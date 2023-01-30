import webbrowser

def app():
    print('Hello Music DC')
    print('What music do you want to play:\r\n')
    start_menu()

    #categorias = ['ciencia', 'sociales', 'geografía']
    #print(categorias[0])
    #for categoria in categorias:
     #print(categoria)

    clientes = [{'nombre':'Ana',
                'edad': 25,
                'estado': 'activo'},
                {'nombre':'Mario',
                'edad': 32,
                'estado': 'activo'}]

    for clie in clientes:
        for llave,valor in clie.items():
            #print(llave)
            print(valor)

    request = True
    reset = restart_request(request)

    while reset:
        option = input('Choose one: \r\n')
        option = int(option)
        if (option == 1):
            start_pop()
        elif (option == 2): 
            start_regueeton()
        elif (option == 3):
            start_rock()
        elif (option == 4):
            start_salsa()
        elif (option == 5):
            reset = restart_request(False)
        else:
            print('Invalid option\r\n')


def restart_request(request):
    return request

def start_pop():
    print('You\'re listen POP now')
    webbrowser.open('https://music.stingray.com/', new = 2)

def start_regueeton():
    print('You\'re listen regueeton now')

def start_rock():
    print('You\'re listen rock now')

def start_salsa():
    print('You\'re listen salsa now')          

def start_menu():
    print('1) POP \r\n')
    print('2) REGUEETON \r\n')
    print('3) ROCK \r\n')
    print('4) SALSA \r\n')
    print('5) EXIT APP \r\n')

app()    