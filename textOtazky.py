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


def Abc(qfull,text):
    answer = 0
    qstart = 0
    bodu = re.search(r"\d bod",qfull).group()[0] ##Zjištění proměných
    if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
        qstart=qfull.find("bod")+6
        cislo = int(qfull[slice(qstart-2,qstart)].replace(r"[a-zA-Z]",""))
    else:
        qstart=qfull.find("body")+7
        cislo = int(qfull[slice(qstart-3,qstart)].replace(r"[a-zA-Z]",""))
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
    spravna = input("Zadejte správnou odpověď: #%i"%(cislo)).replace('\n', '').replace('\r', '')

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
    

def intTryParse(value):
    try:
        return int(value)
    except ValueError:
        return 0

def Otevrena(qfull,text):
 #VKládání otázky s otevřenoou odpovědí
    try:
        file = open("OtevrenaTxt.txt","x")
    except:
        file = open("OtevrenaTxt.txt","a")

    #Zjištění proměných (Pokud by byla chyba v zadávání nastave error ještě před zapisováním)
    bodu = re.search(r"\d bod",qfull).group()[0]
    qstart = 0
    if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
        qstart=qfull.find("bod")+6
        cislo = intTryParse(qfull[slice(qstart-2,qstart)].replace(r"[a-zA-Z].","").replace(".",""))
    else:
        qstart=qfull.find("body")+7
        cislo = intTryParse(qfull[slice(qstart-3,qstart)].replace(r"[a-zA-Z]","").replace(".",""))
    ukol = qfull[qstart:]
    if(re.search(r"\d\.\d",ukol)):
        ukol = ukol[re.search(r"\d\.\d",ukol).span()[1]:]
    while(ukol[0]=="\n" or ukol[0]==" "):
        ukol=ukol[1:]
    if(re.search(r"\d",ukol)):
        if(re.search(r"\d",ukol).span()[1] == len(ukol)):
            ukol=ukol[:len(ukol)-1]

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
    file.write(input("Zadejte správnou odpověď: #%i"%(cislo)))
    file.write("</Spravna>\r\n")
    file.write("</Otazka>\r\n")
    file.flush()
    file.close()
    file = None

def AnoNe(qfull,text):
    try:
        file = open("AnoNeTxt.txt","x")
    except:
        file = open("AnoNeTxt.txt","a")

    bodu = re.search(r"\d bod",qfull).group()[0]

    qstart = 0
    if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
        qstart=qfull.find("bod")+5
        cislo = int(qfull[slice(qstart-2,qstart)].replace(r"[a-zA-Z]",""))
    else:
        qstart=qfull.find("body")+7
        cislo = int(qfull[slice(qstart-3,qstart)].replace(r"[a-zA-Z]",""))
    qend = qfull.find("\nA N")
    if(qend == -1):
        qend = qfull.find(" A N")
    ukol = qfull[qstart:qend]
    podukoly = []
    l = True
    while(l):
        sqnumindex = re.search(r"\d\.\d",qfull).span()[1] ##Číslo podotázky
        qfull = qfull[sqnumindex+1:] ##zkrácení o první číslo
        try:
            nextsq = re.search(r"\d\.\d",qfull).span()
        except:
            nextsq = None
            l = False
        if(nextsq != None):
            podukoly.append(qfull[:nextsq[0]-1]) ##Text podotázky
        else:
            podukoly.append(qfull)
    
    print("Zadejte správné odpovědi: (Zkopírovat z výsledků) #%i"%(cislo))
    odpovedi = sys.stdin.read()
    flush_input()
    odpovedisor = ""

    for i in range(len(podukoly)):
        c = re.search(r"\d\.\d",odpovedi).span()[1]
        odpovedisor += odpovedi[c+1]
        odpovedi = odpovedi[c+1:]

    ##ZAPISOVÁNÍ
    file.write("<Otazka>\r\n")
    file.write("    <Typ>txtAnoNe</Typ>\r\n")
    file.write("    <Text>\r\n")
    file.write(text)
    file.write("</Text>\r\n")
    file.write("    <Bodu>")
    file.write(bodu)
    file.write("</Bodu>\r\n")
    file.write("    <Ukol>") ##Text otázky ( úvod k dalším otázkám)
    file.write(ukol)
    file.write("</Ukol>\n")
    file.write("    <Moznosti>\r\n")
    for index,podukol in enumerate(podukoly):
        file.write("        <Moznost")
        file.write(str(index))
        file.write(">\r\n")
        file.write(podukol)
        file.write("</Moznost")
        file.write(str(index))
        file.write(">\r\n")
    file.write("    </Moznosti>\r\n")
    file.write("    <Spravna>")
    file.write(odpovedisor)
    file.write("</Spravna>\r\n")
    file.write("</Otazka>\r\n")
    file.flush()
    file.close()


