import PyPDF2
import re

##path = input("Zadej adresu/y pdf souboru s testem : ")
path = "jaro2018.pdf"
##Otevření PDF
input_path = True
while(input_path == True): 
    try:
        pdf_file = open(path, 'rb')
        input_path = False
    except:
        print("Adresa neexistuje")
        input_path = True

read_pdf = PyPDF2.PdfFileReader(pdf_file)
number_of_pages = read_pdf.getNumPages()

def GetQuestion(page_number,start_index):
    page = read_pdf.getPage(page_number)
    content = page.extractText() ##Získá text stránky
    find_question = True
     ##Pokud ještě jsou otázky hledej další
    question_start = content.find("bod",start_index)
    if(content.find("body",question_start+1)<content.find("bod",question_start+1)):
        question_end=content.find("bod",question_start+1)-1
    else:
        question_end=content.find("body",question_start+1)-1


    return(content[slice(question_start,question_end,1)])
    ##return(question_start,"|",question_end)

current_page = 1
current_question = 1
print(GetQuestion(current_page,0))


