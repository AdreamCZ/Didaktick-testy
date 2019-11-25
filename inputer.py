import string
import re 
import sys  

filepath = "otazky.txt"
abcpath = "abc.txt"
anonepath = "anone.txt"
otevrenapath="otevrena.txt"
textabcpath = "textabc.txt"
textanonepath = "textanone.txt"
textotevrenapath = "textotevrena.txt"
textserazeni = "textserazeni.txt"

index = 0
def Menu():
    try:
        file = open(filepath,"r")
    except:
        file = open(filepath,"wr+")
    if(file.read()!=""):
        si = 0
        _index = 0
        while(file.read().find("Otazka",si) != -1):
            if(_index != -1):
                index = _index
                si = _index
            _index = int(file.read()[file.read().find("Otazka",si)+6])
    else:
        index = 0


    typ = input("Jaký typ otázky budeš zadávat ? (1-Práce s textem, 2-normální, cokoliv jiného = konec) (Konec zadávání Ctrl+Z)")
    if(typ=="1"):
        Que_text()
    elif(typ=="2"):
        Question()
    else:
        if file.closed == False:
            file.close()
        if(input("Opravdu ukončit ? (y/n)").capitalize=="Y"):
            exit()

abc = ["A)","B)","C)","D)","E)","F)"]


def Que_text(): ##Vložení otázky práce s textem
    qfull = ""

    

        
    print("Zadejte výchoží text k otázkám")
    text = sys.stdin.read()
    answer = 0
    while(True):
        file = open(textabcpath,"w")
        print("Otázka formátu A)B)C) (N=další)") #ABC
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull)<5):
            break
        file.write("<otazka>\n")
        file.write("    <typ>txtABC</typ>\n") ##Vložení výchozího textu
        file.write("    <text>\n")
        file.write(text)
        file.write("    </text>\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>\n")
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        file.write("    <ukol>")
        file.write(question)
        file.write("    </ukol>\n")
        file.write("    <moznosti>\n")
        while(qfull.find(abc[answer+1]) != -1):
            file.write("        <moznost>")
            file.write(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            file.write("        </moznost>\n")
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
        ##Poslední odpověď 
        file.write("        <moznost>")
        file.write(qfull[qfull.find(abc[answer]):])
        file.write("        </moznost>\n")
        file.write("    </moznosti>")
        qfull = ""
        ##Vložení správné odpovědi
        file.write("    <spravna>")
        file.write(input("Zadejte správnou odpověď: ").replace('\n', ' ').replace('\r', ''))
        file.write("    </spravna>\n")
        file.write("</otazka>")
        index +=1
        file.close()
        
    while(True): #VKládání otázky s otevřenoou odpovědí
        file = open(textotevrenapath,"w")
        print("Otevřená otázka (N=pokračovat na další)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("<otazka>\n")
        file.write("    <typ>txtOtevrena</typ>\n")
        file.write("    <text>\n")
        file.write(text)
        file.write("    </text>")


        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>\n")
        
        qstart = 0
        
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("    <ukol>")
        file.write(qfull[qstart:])
        file.write("    </ukol>\n")
        
        file.write("    <spravna>")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("    </spravna>\n")
        file.write("</otazka>\n")
        index+=1
        file.close()
        
    while(True): ##Otázka Ano ne
        file = open(textanonepath,"w")
        print("Ano/Ne otázka (N=další typ)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("<otazka>")
        file.write("    <typ>txtANONE</typ>\n")
        file.write("    <text>\n")
        file.write(text)
        file.write("    </text>\n")
        
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>")
          

          
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("    <ukol>") ##Text otázky ( úvod k dalším otázkám)
        file.write(qfull[qstart:qfull.find("A N",-1)])
        file.write("    </ukol>\n")
        l = True
        while(l):
            file.write("        <podukol>")
            sqnumindex = re.search(r"\d.\d",qfull).span()[1] ##Číslo podotázky
            file.write(qfull[sqnumindex]) 
            qfull = qfull[sqnumindex+1:] ##zkrácení o úvod první číslo
            try:
                nextsq = re.search(r"\d.\d",qfull).span()
            except:
                nextsq = None
                l = False
            if(nextsq != None):
                file.write(qfull[sqnumindex+1:nextsq[0]-1]) ##Text podotázky
            else:
                file.write(qfull[sqnumindex+1:])
            file.write("        </podukol>,\n")

        
        file.write("    <spravna>")
        odpovedi = input("Zadejte správné odpovědi: ")
        odpovedisor = ""
        for i in range(answer):
            c = re.search(r"\d.\d",odpovedi).span()[1]
            odpovedisor += odpovedi[c+1]
            odpovedi = odpovedi[c+1:]
        file.write(odpovedisor)
        file.write("    </spravna>\n")
        file.write("</otazka>")
        file.close()
        
        
    while(True): #Otázka s přiřazováním možností (seřazení)
        file = open(textserazeni,"w")
        answer = 0
        print("Otázka formátu Přiřazování ukázek (N=další)") 
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull)<5):
            break
        file.write("<otazka>\n")
        file.write("    <typ>serad</typ>\n")
        file.write("    <text>")
        file.write(text)
        file.write("    </text>\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>\n")
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        file.write("    <ukol>")
        file.write(question)
        file.write("    </ukol>\n")
        file.write("    <moznosti>\n")
        while(qfull.find(abc[answer+1]) != -1):
            file.write("        <moznost>")
            file.write(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            file.write("</moznost>\n")
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
         ##Poslední odpověď 
        file.write(qfull[qfull.find(abc[answer]):])
        file.write("    <moznosti>\n")
        qfull = ""
        ##Vložení správné odpovědi
        file.write("    <spravna>")
        odpovedi = input("Zadejte správné pořadí: ")
        odpovedisor = ""
        for i in range(answer):
            c = re.search(r"\d.\d",odpovedi).span()[1]
            odpovedisor += odpovedi[c+1]
            odpovedi = odpovedi[c+1:]
        file.write(odpovedisor)
        file.write("    </spravna>\n")
        file.write("</otazka>")
        index +=1
        file.close()
    

    Menu()

def Question():
    file = open(abcpath,"w")
    global index
    while(True):
        print("Otázka formátu A)B)C) (N=další)")
        answer = 0
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("<otazka>")
        file.write("    <typ>ABC</typ>\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>\n")
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        file.write("    <ukol>")
        file.write(question)
        file.write("    </ukol>\n")
        file.write("    <moznosti>")
        while(qfull.find(abc[answer+1]) != -1):
            file.write("        <moznost>")
            file.write(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            file.write("        </moznost>\n")
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
        ##Poslední odpověď 
        file.write("        <moznost>")
        file.write(qfull[qfull.find(abc[answer]):])
        file.write("        </moznost>")
        file.write("    </Moznosti>\n")
        qfull = ""
        ##Vložení správné odpovědi
        file.write("    <spravna>")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("    </spravna>\n")
        file.write("</otazka>")
        index +=1
        file.close()


    while(True): #VKládání otázky s otevřenoou odpovědí
        file=open(otevrenapath,"w")
        print("Otázka s otevřenou odpovědí (N=další)")
        answer = 0
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("<otazka>")
        file.write("    <typ>otevrena</n>\n")
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("    <body>")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("    </body>\n")
        
        qstart = 0
        
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("    <ukol>")
        file.write(qfull[qstart:])
        file.write("    </ukol>\n")
        
        file.write("    <spravna>")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("    </spravna>\n")
        file.write("</otazka>")
        index+=1
        file.close()
   
   
Menu()


        







        




