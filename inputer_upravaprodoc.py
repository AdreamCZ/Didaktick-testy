import textOtazky
import Otazky
import sys
import re

def flush_input():
    try:
        import msvcrt
        while msvcrt.kbhit():
            msvcrt.getch()
    except ImportError:
        import sys, termios    #for linux/unix
        termios.tcflush(sys.stdin, termios.TCIOFLUSH)

def Menu():
    typ = input("Jaký typ otázky budeš zadávat ? (1-Práce s textem, 2-normální, cokoliv jiného = konec) (Konec zadávání Ctrl+Z \ UNIX Ctrl+D)")
    if(typ=="1"):
        Text_questions()
    elif(typ=="2"):
        Questions()
    else:
        if(input("Opravdu ukončit ? (y/n)").capitalize=="Y"):
            exit()

def Vloz_text():  ## Vkládání text 
    print("Vložte text")
    p = sys.stdin.read()
    flush_input()
    text = ""
    while(p):
        text+=p
        text+="\n"
        ##Zvýrazněná slova
        b = input("Vložte slovo které je zvýrazněno tlustě ")
        while(b):
            if(text.count(b) < 1):
                print("Slovo nenalezeno")
                b = input("Vložtě další tučné slovo")
            elif text.count(b) == 1:
                s = text.find(b)
                text = text[:s]+"(!b)"+text[s:s+len(b)] + "(?b)" + text[s+len(b):] # Vložení značek (!b) a (?b) na začatek a konec zvýrazněného slova
                b = input("Vložte další tučné slovo ")
            elif text.count(b)>1:
                vyskyty = []
                print("Vyber správné slovo : ")
                for i in range(text.count(b)):
                    if(i==0):
                        vyskyty.append(text.find(b)) ##vytváří seznam všech výskytů slova
                    else:
                        vyskyty.append(text.find(b,vyskyty[i-1]+1))
                    print(str(i)+ ". : " + text[vyskyty[i]-12:vyskyty[i]+12])
                vyskyt = int(input("Zadejte číslo správného : "))
                s = vyskyty[vyskyt]
                text = text[:s]+"(!b)"+text[s:s+len(b)] + "(?b)" + text[s+len(b):] # Vložení značek (!b) a (?b) na začatek a konec zvýrazněného slova
                b = input("Vložte další tučné slovo ")

        ##Podtržená slova
        u = input("Vložte slovo(a) které je podtrženo ")
        while(u):
            if(text.count(u) < 1):
                print("Slovo nenalezeno")
                u = input("Vložte další podtržené slovo")
            elif text.count(u) == 1:
                s = text.find(u)
                text = text[:s]+"(!u)"+text[s:s+len(u)] + "(?u)" + text[s+len(u):] # Vložení značek (!u) a (?u) na začatek a konec podtrženého slova
                u = input("Vložte další podtržené slovo ")
            elif text.count(u)>1:
                vyskyty = []
                print("Vyber správné slovo : ")
                for i in range(text.count(u)):
                    if(i==0):
                        vyskyty.append(text.find(u)) ##vytváří seznam všech výskytů slova
                    else:
                        vyskyty.append(text.find(u,vyskyty[i-1]+1))
                    print(str(i)+ ". : " + text[vyskyty[i]-12:vyskyty[i]+12])
                vyskyt = int(input("Zadejte číslo správného : "))
                s = vyskyty[vyskyt]
                text = text[:s]+"(!u)"+text[s:s+len(u)] + "(?u)" + text[s+len(u):] # Vložení značek (!u) a (?u) na začatek a konec podtrženého slova
                u = input("Vložte další podtržené slovo ")

        

    return text

def Text_questions(text = None):
    if(text == None):
        text = Vloz_text()
    try:
        textOtazky.Abc(text)
    except:
            print(sys.exc_info()[0])
            textOtazky.Abc(text)
    try:
        textOtazky.Otevrena(text)
    except:
        print(sys.exc_info()[0])
        textOtazky.Otevrena(text)
    try:
        textOtazky.AnoNe(text)
    except:
        print(sys.exc_info()[0])
        textOtazky.AnoNe(text)
    pokracuj = input("Další otázky se stejným textem ? (A/N)")
    if(pokracuj == "A"):
        Text_questions(text)
    else:
        Menu()

def Questions():
    try:
        Otazky.Abc()
    except:
        print(sys.exc_info()[0])
        Otazky.Abc()
    try:
        Otazky.Serazeni()
    except:
        print(sys.exc_info()[0])
        Otazky.Serazeni()
    try:
        Otazky.AnoNe()
    except:
        print(sys.exc_info()[0])
        Otazky.AnoNe()

    Menu()

#Menu()