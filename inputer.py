import string
def Menu():
    typ = int(input("Jaký typ otázky budeš zadávat ? (1-Práce s textem, 2-normální)"))
    if(typ==1):
        Que_text()

abc = ["A)","B)","C)","D)","E)","F)"]
index = 0
file = open("otazky.txt","w")

def Que_text(): ##Vložení otázky práce s textem
    qfull = ""
    text = input("zadejte výchozí text/y k otázkám")
    answer = 0
    while(input("pokračovat ? (Y/N)")!="N"):
        file.write("Otazka")
        file.write(str(index))
        file.write("{\n")
        file.write("type=\"text\",")
        file.write("text=\"")
        file.write(text)
        file.write("\",")
        qfull=input("Zadejte text otázky formátu A)B)C)D)")
        question = qfull[slice(0,qfull.find("A)"))]
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
        file.write("}")
    file.close()
Menu()



        





