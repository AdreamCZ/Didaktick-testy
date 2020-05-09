import Otazky
import textOtazky
import sys
import re
import inputer


def flush_input():
    try:
        import msvcrt
        while msvcrt.kbhit():
            msvcrt.getch()
    except ImportError:
        import sys, termios    #for linux/unix
        termios.tcflush(sys.stdin, termios.TCIOFLUSH)



##Třídění
while(True):
    print("Otázky s Textem ? (A/N)")
    tvolba = input()
    print(tvolba)
    if(tvolba.lower()=="n"):
        print("Zadejte blok otázek bez textu")
        qs = sys.stdin.read()
        flush_input()

        while(re.search(r"\d bod",qs)): #Opakuj pokud se v řetězci všech otázek vyskytuje určení počtu bodů
            qstart = re.search(r"\d bod",qs).span()[0] #Začátek otázky
            #Zjištění konce otázky
            if(re.search(r"\d bod",qs[qstart+7:])): #Pokud je někde další určení počtu bodů, použiju ho jako konec:
                qend = re.search(r"\d bod",qs[qstart+7:]).span()[0]+qstart+7 
            else: #Pokud už nenásledují žádné další otázky
                qend = len(qs) #konec je délka zbývajícího řetězce
            question = qs[qstart:qend] #Získaná jedna otázka
            qs = qs[qend:]#Zkrácení řetězce všech otázek o tu kterou jsem právě uložil
            #Zkrácení o nadbytečné znaky nakonci
            if(question.rfind("max.")>len(question)-8): 
                question = question[:question.rfind("max.")] #Smaže max.
            while(question[-1]=="\n"):
                question=question[:len(question)-1]
                print("Zkracuju newline")
            poslednicislo = None
            cisla = re.finditer(r"\d",question)
            for poslednicislo in cisla: #Najde poslední výskyt čísla
                pass
            while(poslednicislo.span()[1] > len(question)-5): #Pokud je poslední číslo u konce
                print("Zkracuju cislo")
                question=question[:len(question)-1] #Smaže poslední
                poslednicislo = None
                cisla = re.finditer(r"\d",question)
                for poslednicislo in cisla:
                    pass


            if(re.search("Seřaďte",question) or re.search("seřaďte",question)):
                Otazky.Serazeni(question)
                print("Typ Serazeni")
            elif(re.search("A N",question)):
                Otazky.AnoNe(question)
                print("Typ ANO/NE")
            elif(re.search("Přiřaďte",question) or re.search("přiřaďte",question)):
                Otazky.Prirazeni(question)
                print("Typ přižazení")
            elif(re.search(re.escape("A)"),question)):
                Otazky.Abc(question)
                print("Typ ABC")
            else:
                print("CHYBA: Otázce nebyl přiřazen typ")
                print(question)
    else:
        text = inputer.Vloz_text()
        print("Zadejte blok otázek s textem")
        qs = sys.stdin.read()
        flush_input()

        while(re.search(r"\d bod",qs)):
            qstart = re.search(r"\d bod",qs).span()[0]
            if(re.search(r"\d bod",qs[qstart+7:])):
                qend = re.search(r"\d bod",qs[qstart+7:]).span()[0]+qstart+7
            else:
                qend = len(qs)
            question = qs[qstart:qend]
            #Zkrácení o nadbytečné znaky nakonci
            if(question.rfind("max.")>len(question)-8):
                question = question[:question.rfind("max.")]
            while(question[-1]=="\n"):
                question=question[:len(question)-1]
                print("Zkracuju newline")
            poslednicislo = None
            cisla = re.finditer(r"\d",question)
            for poslednicislo in cisla:
                pass
            while(poslednicislo.span()[1] > len(question)-5):
                print("Zkracuju cislo")
                print(question)
                question=question[:len(question)-1]
                poslednicislo = None
                cisla = re.finditer(r"\d",question)
                for poslednicislo in cisla:
                    pass

            qs = qs[qend:]
            if(re.search("A N",question)):
                textOtazky.AnoNe(question,text)
                print("Typ ANO/NE")
            elif(re.search(re.escape("A)"),question)):
                textOtazky.Abc(question,text)
                print("Typ ABC")
            elif(re.search("napište",question) or re.search("vypište",question) or re.search("Napište",question) or re.search("Vypište",question)):
                if(len(re.findall(r"\d\.\d",question)) > 1):
                    bodu = re.search(r"\d bod",question).group()[0]
                    for i in range(len(re.findall(r"\d\.\d",question))):
                        if(len(re.findall(r"\d\.\d",question)) >1):
                            podcisla= re.finditer(r"\d\.\d",question)
                            podcisla.__next__()
                            podcislo2 = podcisla.__next__()
                            q1 = question[re.search(r"\d\.\d",question).span()[1]:podcislo2.span()[0]]
                            question = question[podcislo2.span()[0]:]
                            q1 = bodu+" bod "+q1
                            textOtazky.Otevrena(q1,text)
                            print("-------")
                            print(q1)
                            print("-------")
                        else:
                            textOtazky.Otevrena(bodu+" bod " + question,text)
                            print("-------")
                            print(question)
                            print("-------")
                else:
                    textOtazky.Otevrena(question,text)
                print("Typ otevřená")
            else:
                print("CHYBA: Otázce nebyl přiřazen typ")
                print(question)
