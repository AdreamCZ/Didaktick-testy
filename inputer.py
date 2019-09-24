import string
import re 

def Menu():
    typ = int(input("Jaký typ otázky budeš zadávat ? (1-Práce s textem, 2-normální)"))
    if(typ==1):
        Que_text()

abc = ["A)","B)","C)","D)","E)","F)"]
index = 0
file = open("otazky.txt","w+")
file.close()


def Que_text(): ##Vložení otázky práce s textem
    qfull = ""
    file = open("otazky.txt","r")
    if(file.read()!=""):
        index = file.read()[file.read().find("Otazka",-1)+6:]
    else:
        index = 0
    
    file = open("otazky.txt","w")
        
    text = input("zadejte výchozí text/y k otázkám")
    answer = 0
    while(input("pokračovat ? (Y/N)(A)B)C) ").upper()!="N"):
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("type=\"text\",\n") ##Vložení výchozího textu
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
        qfull=input("Zadejte text otázky formátu A)B)C)D)")
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
        
    while(input("pokračovat ? (Y/N)(Otevřená otázka)").upper()!="N"): #VKládání otázky s otevřenoou odpovědí
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
        file.write("type=\"otevrena\",\n")
        
        qfull=input("Zadejte text otázky")
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\"\n")
        
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
        
    while(input("pokračovat ? (Y/N)(Ano/Ne otázka)").upper() != "N"): ##Otázka Ano ne
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("text=\"")
        file.write(text)
        file.write("\",\n")
          
        qfull= input("zadejte text otázky (ANO/NE) (Body na začátku)")
        
        ##Počet bodů .. Nutné vložení na začátku textu
        file.write("bodu=\"")
        file.write(re.search(r"\d bod",qfull).group()[0])
        file.write("\"\n")
          
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
            nextsq = re.search(r"\d.\d",qfull).span()
            file.write(qfull[sqnumindex+1:nextsq[0]-1]) ##Text podotázky
            file.write("\",\n")
            if(nextsq == None): ##Pokud není další subotázka 
                l = False ##přestaň
        
        file.write("correct=\"")
        file.write(input("Zadejte správé odpovědi (ano,ne,ne) : "))
        file.write("\"\n")
        file.write("}")
        
    
    file.close()
    Menu()
   
Menu()


        







        




