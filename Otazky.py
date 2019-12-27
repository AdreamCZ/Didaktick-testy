import sys
import re
import os
from flush_input import *
abc = ["A)","B)","C)","D)","E)","F)"]

def Abc():
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
            file.write(index+1)
            file.write(">")
            file.write(moznost) ##Text odpovědi
            file.write("</Moznost")
            file.write(index+1)
            file.write(">\r\n")
        file.write("    </Moznosti>\r\n")
        ##Vložení správné odpovědi
        file.write("    <Spravna>")
        file.write(spravna)
        file.write("</Spravna>\r\n")
        file.write("</Otazka>\r\n")
        file.flush()
        file.close()
