/************************/
--Migrations

dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add m1
dotnet ef migrations script
dotnet ef database update


Plugin de leitura do PDF
https://github.com/GowenGit/docnet
Documentação
http://cdn01.foxitsoftware.com/pub/foxit/manual/enu/FoxitPDF_SDK20_Guide.pdf
https://pdfium.googlesource.com/pdfium/+/refs/heads/main/docs/getting-started.md


Plugin de OCR
https://github.com/charlesw/tesseract

Traineddata Files 
https://tesseract-ocr.github.io/tessdoc/Data-Files.html
https://github.com/tesseract-ocr/tessdata_best



leptonica-1.80.0.dll é uma lib requerida no Azure
C:\Users\Jhonatas\.nuget\packages\tesseract\4.1.1\x86


Portable Document Format Reference Manual
https://web.archive.org/web/20150617123515/http://acroeng.adobe.com/PDFReference/PDF%20Reference%201.0.pdf



PDF Clawn
https://sourceforge.net/projects/clown/files/
https://pdfclown.files.wordpress.com/2015/02/userguide.pdf
http://clown.sourceforge.net/docs/api/