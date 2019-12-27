import sys
import re
import os
abc = ["A)","B)","C)","D)","E)","F)"]

def flush_input():
    try:
        import msvcrt
        while msvcrt.kbhit():
            msvcrt.getch()
    except ImportError:
        import sys, termios    #for linux/unix
        termios.tcflush(sys.stdin, termios.TCIOFLUSH)


def Abc(text):
    while(True):
        answer = 0
        print("Otázka formátu A)B)C) (N=další)") #ABC
        qfull = sys.stdin.read()
        flush_input()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull)<5):
            break
        qstart = 0
        bodu = re.search(r"\d bod",qfull).group()[0] ##Zjištění proměných
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        moznosti = []
        while(qfull.find(abc[answer+1]) != -1):
            moznosti.append(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
        ##Poslední odpověď 
        moznosti.append(qfull[qfull.find(abc[answer]):])
        qfull = ""
        spravna = input("Zadejte správnou odpověď: ").replace('\n', '').replace('\r', '')

        try:
            file = open("ABCTxt.txt","x")
        except:
            file = open("ABCTxt.txt","a")
        
        file.write("<Otazka>\r\n")
        file.write("    <Typ>txtABC</Typ>\r\n") ##Vložení výchozího textu
        file.write("    <Text>")
        file.write(text)
        file.write("</Text>\r\n")
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <Bodu>")
        file.write(bodu)
        file.write("</Bodu>\r\n")
        file.write("    <Ukol>")
        file.write(question)
        file.write("</Ukol>\r\n")
        file.write("    <Moznosti>\r\n")
        for index,moznost in enumerate(moznosti):
            file.write("        <Moznost")
            file.write(str(index+1))
            file.write(">")
            file.write(moznost) ##Text odpovědi
            file.write("</Moznost")
            file.write(str(index+1))
            file.write(">\r\n")
        file.write("    </Moznosti>\r\n")
        ##Vložení správné odpovědi
        file.write("    <Spravna>")
        file.write(spravna)
        file.write("</Spravna>\r\n")
        file.write("</Otazka>\r\n")
        file.flush()
        file.close()
        file = None
    

def Otevrena(text):
    while(True): #VKládání otázky s otevřenoou odpovědí
        try:
            file = open("OtevrenaTxt.txt","x")
        except:
            file = open("OtevrenaTxt.txt","a")
        print("Otevřená otázka (N=pokračovat na další)")
        qfull = sys.stdin.read()
        flush_input()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            return
        #Zjištění proměných (Pokud by byla chyba v zadávání nastave error ještě před zapisováním)
        bodu = re.search(r"\d bod",qfull).group()[0]
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        ukol = qfull[qstart:]

        file.write("<Otazka>\r\n")
        file.write("    <Typ>txtOtevrena</Typ>\r\n")
        file.write("    <Text>")
        file.write(text)
        file.write("</Text>\r\n")
        file.write("    <Bodu>")
        file.write(bodu)
        file.write("</Bodu>\r\n")
        file.write("    <Ukol>")
        file.write(ukol)
        file.write("</Ukol>\r\n")
        
        file.write("    <Spravna>")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("</Spravna>\r\n")
        file.write("</Otazka>\r\n")
        file.flush()
        file.close()
        file = None
