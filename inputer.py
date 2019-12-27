import textOtazky
import Otazky
import flush_input
import sys
import re

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
    p = input("Vložte první odstavec textu ")
    text = ""
    out = []
    while(p):
        text+=p
        text+="\n"
        ##Zvýrazněná slova
        b = input("Vložte slovo které je zvýrazněno tlustě ")
        while(b):
            if(text.count(b) < 1):
                print("Slovo nenalezeno")
                break
            elif text.count(b) == 1:
                s = text.find(b)
                text = text[:s]+"(!b)"+text[s:s+len(b)] + "(?b)" + text[s+len(b):] # Vložení značek (!b) a (?b) na začatek a konec zvýrazněného slova
                b = input("Vložte další zvýrazněné slovo ")
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
                b = input("Vložte další zvýrazněné slovo ")

        ##Podtržená slova
        u = input("Vložte slovo(a) které je podtrženo ")
        while(u):
            if(text.count(u) < 1):
                print("Slovo nenalezeno")
                break
            elif text.count(u) == 1:
                s = text.find(u)
                text = text[:s]+"(!u)"+text[s:s+len(u)] + "(?u)" + text[s+len(u):] # Vložení značek (!u) a (?u) na začatek a konec podtrženého slova
                u = input("Vložte další zvýrazněné slovo ")
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
                u = input("Vložte další zvýrazněné slovo ")

        p = input("Vložte další odstavec (Pro konec nic nevkládej)")

    return text

def Text_questions():
    try:
        textOtazky.Abc(Vloz_text())
    except:
            print(sys.exc_info()[0])
            textOtazky.Abc(Vloz_text())
    try:
        textOtazky.Otevrena(Vloz_text())
    except:
        print(sys.exc_info()[0])
        textOtazky.Otevrena(Vloz_text())

    Menu()

def Questions():
    try:
        Otazky.Abc()
    except:
        print("Chyba v zadávání")
        Otazky.Abc()

    Menu()

Menu()