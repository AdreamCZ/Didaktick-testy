import re
import sys

def flush_input():
    try:
        import msvcrt
        while msvcrt.kbhit():
            msvcrt.getch()
    except ImportError:
        import sys, termios    #for linux/unix
        termios.tcflush(sys.stdin, termios.TCIOFLUSH)

abc = ["A)","B)","C)","D)","E)","F)","G)","H)","CH)"]

def Abc(qfull = ""):
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
    spravna = input("Zadejte správnou odpověď #%i: "%(cislo)).replace('\n', '').replace('\r', '')

    try:
        file = open("ABC.txt","x")
    except:
        file = open("ABC.txt","a")
    
    file.write("<Otazka>\r\n")
    file.write("    <Typ>ABC</Typ>\r\n") ##Vložení výchozího textu
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

def Serazeni(qfull):
    answer = 0
    qstart = 0
    bodu = re.search(r"\d bod",qfull).group()[0] ##Zjištění proměných

    zdroj = qfull[qfull.rfind("("):qfull.rfind(")")+1]
    qfull = qfull[:qfull.rfind("(")]
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

    print("Zadejte správné odpovědi: (Zkopírovat z výsledků) #%i"%(cislo)) #Zadání a zpracování správného pořadí
    odpovedi = sys.stdin.read()
    flush_input()
    odpovedisor = ""
    for i in range(len(moznosti)):
        c = re.search(r"\d\.\d",odpovedi).span()[1]
        odpovedisor += odpovedi[c+1]
        odpovedi = odpovedi[c+1:]

    try:
        file = open("Serazeni.txt","x")
    except:
        file = open("Serazeni.txt","a")
    
    file.write("<Otazka>\r\n")
    file.write("    <Typ>Serazeni</Typ>\r\n") ##Vložení výchozího textu
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
    file.write(odpovedisor) #Zapsání správného pořadí do souboru
    file.write("</Spravna>\r\n")
    if(zdroj):
        file.write("    <Zdroj>")
        file.write(zdroj)
        file.write("</Zdroj>\r\n")
    file.write("</Otazka>\r\n")
    file.flush()
    file.close()

def AnoNe(qfull):
    try:
        file = open("AnoNe.txt","x")
    except:
        file = open("AnoNe.txt","a")
    print("Ano/Ne otázka (N=další typ)")

    bodu = re.search(r"\d bod",qfull).group()[0]
    qstart = 0
    if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
        qstart=qfull.find("bod")+6
        cislo = int(qfull[slice(qstart-2,qstart)].replace(r"[a-zA-Z]",""))
    else:
        qstart=qfull.find("body")+7
        cislo = int(qfull[slice(qstart-3,qstart)].replace(r"[a-zA-Z]",""))
    ukol = qfull[qstart:qfull.find("\nA N")]
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
    file.write("    <Typ>AnoNe</Typ>\r\n")
    file.write("    <Bodu>")
    file.write(bodu)
    file.write("</Bodu>\r\n")
    file.write("    <Ukol>") ##Text otázky ( úvod k dalším otázkám)
    file.write(ukol)
    file.write("</Ukol>\n")
    file.write("    <Moznosti>\r\n")
    for index,podukol in enumerate(podukoly):
        file.write("        <Moznost") ##V tomto případě znamená možnost jednu podotázku
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


def Prirazeni(qfull):
    answer = 0
    try:
        file = open("Prirazeni.txt","x")
    except:
        file = open("Prirazeni.txt","a")
    
    bodu = re.search(r"\d bod",qfull).group()[0]
    qstart = 0
    if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
        qstart=qfull.find("bod")+6
        cislo = int(qfull[slice(qstart-2,qstart)].replace(r"[a-zA-Z]",""))
    else:
        qstart=qfull.find("body")+7
        cislo = int(qfull[slice(qstart-3,qstart)].replace(r"[a-zA-Z]",""))
    ukol = qfull[qstart:re.search(r"\d\.\d ",qfull).span()[0]]
    podukoly = []
    l = True
    while(l):
        sqnumindex = re.search(r"\d\.\d ",qfull).span()[1] ##Číslo podotázky
        qfull = qfull[sqnumindex:] ##zkrácení o první číslo
        try:
            nextsq = re.search(r"\d\.\d ",qfull).span()
        except:
            nextsq = None
            l = False
        if(nextsq != None):
            podukoly.append(qfull[:nextsq[0]-1]) ##Text podotázky
        else:
            podukoly.append(qfull[:qfull.find("A)")])

    qfull = qfull[qfull.find("A)"):]
    moznosti = []
    while(qfull.find(abc[answer+1]) != -1):

        moznosti.append(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
        qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
        answer += 1
    ##Poslední odpověď 
    moznosti.append(qfull[qfull.find(abc[answer]):])
    qfull = ""
    
    print("Zadejte správné odpovědi: (Zkopírovat z výsledků) #%i"%(cislo))
    odpovedi = sys.stdin.read()
    flush_input()
    odpovedisor = ""
    for i in range(len(podukoly)):
        print(i)
        c = re.search(r"\d\.\d",odpovedi).span()[1]
        odpovedisor += odpovedi[c+1]
        odpovedi = odpovedi[c+1:]

    ##ZAPISOVÁNÍ
    file.write("<Otazka>\r\n")
    file.write("    <Typ>Serazeni</Typ>\r\n")
    file.write("    <Bodu>")
    file.write(bodu)
    file.write("</Bodu>\r\n")
    file.write("    <Ukol>") ##Text otázky ( úvod k dalším otázkám)
    file.write(ukol)
    file.write("</Ukol>\n")
    file.write("    <Podukoly>\r\n")
    for index,podukol in enumerate(podukoly):
        file.write("        <Podukol") ##V tomto případě znamená podukol jednu podotázku ke teré se přiřadí jedna možnost
        file.write(str(index))
        file.write(">\r\n")
        file.write(podukol)
        file.write("</Podukol")
        file.write(str(index))
        file.write(">\r\n")
    file.write("    </Podukoly>\r\n")
    file.write("    <Moznosti>\r\n")
    for index,moznost in enumerate(moznosti):
        file.write("        <Moznost") ##V tomto případě znamená možnost jednu podotázku
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