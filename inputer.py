import string
import re 
import sys  

filepath = "otazky.txt"
index = 0
def Menu():
    try:
        file = open(filepath,"r")
    except:
        file = open(filepath,"wr+")
    if(file.read()!=""):
        index = int(file.read()[file.read().find("Otazka",-1)+6])
    else:
        index = 0


    typ = input("Jaký typ otázky budeš zadávat ? (1-Práce s textem, 2-normální, cokoliv jiného = konec)")
    if(typ=="1"):
        Que_text()
    elif(typ=="2"):
        Question()
    else:
        if file.closed == False:
            file.close()
        exit()

abc = ["A)","B)","C)","D)","E)","F)"]


def Que_text(): ##Vložení otázky práce s textem
    qfull = ""

    
    file = open(filepath,"w")
        
    print("Zadejte výchoží text k otázkám")
    text = sys.stdin.read()
    answer = 0
    while(True):
        print("Otázka formátu A)B)C) (N=další)") #ABC
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull)<5):
            break
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("type=\"text\",\n") ##Vložení výchozího textu
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\",\n")
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        file.write("question=\"")
        file.write(question)
        file.write("\",\n")
        while(qfull.find(abc[answer+1]) != -1):
            file.write(abc[answer][0])
            file.write("=")
            file.write("\"")
            file.write(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            file.write("\",\n")
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
        file.write(abc[answer]) ##Poslední odpověď 
        file.write("=")
        file.write("\"")
        file.write(qfull[qfull.find(abc[answer]):])
        file.write("\",\n")
        qfull = ""
        ##Vložení správné odpovědi
        file.write("correct=\"")
        file.write(input("Zadejte správnou odpověď: ").replace('\n', ' ').replace('\r', ''))
        file.write("\",\n")
        file.write("}")
        index +=1
        
    while(True): #VKládání otázky s otevřenoou odpovědí
        print("Otevřená otázka (N=pokračovat na další)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
        file.write("type=\"otevrena\",\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\",\n")
        
        qstart = 0
        
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("question=\"")
        file.write(qfull[qstart:])
        file.write("\",\n")
        
        file.write("correct=\"")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("\",\n")
        file.write("}")
        index+=1
        
    while(True): ##Otázka Ano ne
        print("Ano/Ne otázka (N=další typ)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
        
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\",\n")
          
        file.write("type=\"ANO/NE\",\n")
          
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("question=\"") ##Text otázky ( úvod k dalším otázkám)
        file.write(qfull[qstart:qfull.find("A N",-1)])
        file.write("\",\n")
        l = True
        while(l):
            file.write("subquestion")
            sqnumindex = re.search(r"\d.\d",qfull).span()[1] ##Číslo podotázky
            file.write(qfull[sqnumindex]) 
            file.write("=\"")
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
            file.write("\",\n")

        
        file.write("correct=\"")
        file.write(re.sub(r"[^A-Za-z]","",input("Zadejte správé odpovědi (ANANA) : ")))
        file.write("\",\n")
        file.write("}")
        file.close()
        
    

    Menu()

def Question():
    file = open(filepath,"w")

    while(True):
        print("Otázka formátu A)B)C) (N=další)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("type=\"ABC\",\n")

        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\"\n")
        qstart = 0
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        question = qfull[slice(qstart,qfull.find("A)"))] ##Text otázky
        qfull = qfull[qfull.find("A)"):]
        file.write("question=\"")
        file.write(question)
        file.write("\",\n")
        while(qfull.find(abc[answer+1]) != -1):
            file.write(abc[answer])
            file.write("=")
            file.write("\"")
            file.write(qfull[qfull.find(abc[answer]):qfull.find(abc[answer+1])]) ##Text odpovědi
            file.write("\",\n")
            qfull = qfull[qfull.find(abc[answer+1]):] ## Odstranění již zpracované části
            answer += 1
        file.write(abc[answer]) ##Poslední odpověď 
        file.write("=")
        file.write("\"")
        file.write(qfull[qfull.find(abc[answer]):])
        file.write("\",\n")
        qfull = ""
        ##Vložení správné odpovědi
        file.write("correct=\"")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("\"\n")
        file.write("}")
        index +=1


    while(True): #VKládání otázky s otevřenoou odpovědí
        print("Otázka s otevřenou odpovědí (N=další)")
        qfull = sys.stdin.read()
        if(qfull.upper().replace("\n","") == "N" or qfull == "" or len(qfull) < 5):
            break
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
        file.write("type=\"otevrena\",\n")
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\",\n")
        
        qstart = 0
        
        if qfull.find("bod")>qfull.find("body"): ##začátek textu otázky
            qstart=qfull.find("bod")+6
        else:
            qstart=qfull.find("body")+7
        
        file.write("question=\"")
        file.write(qfull[qstart:])
        file.write("\",\n")
        
        file.write("correct=\"")
        file.write(input("Zadejte správnou odpověď: "))
        file.write("\"\n")
        file.write("}")
        index+=1
   
   
Menu()


        







        




