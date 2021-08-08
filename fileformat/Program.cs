using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FormatFile
{
    class Program
    {
        static void Main(string[] args)
        {


            DirectoryInfo d = new DirectoryInfo(@"C:\Users\denizhankalkan\Yeni");

            var DirectoryList = d.GetDirectories();
            foreach (var directory in DirectoryList)
            {
                var fileList = directory.GetFiles("*.pdf");
                foreach (var file in fileList.OrderByDescending(m => m.Name))
                {
                    int i;
                    string text = string.Empty; //""
                    var data = file.Name.Split(' ');
                    if (data.Length == 6)
                    {
                        text = data[0] + " " + data[1];
                        i = 2;
                    }
                    else
                    {
                        i = 1;
                        text = data[0];
                    }
                    var pdfSite = data[i + 3].Split('.');
                    var externalSite = pdfSite[0].Split('-');

                    if (externalSite[1].IndexOf('(') > 0)
                    {
                        externalSite[1] = externalSite[1].Substring(0, externalSite[1].IndexOf('('));
                        var fileName_2 = text + " " + data[i] + " " + data[i + 2] + " " + externalSite[0] + "-" + externalSite[1] + "_" + "File" + "." + pdfSite[1];
                        var fullPath_2 = "c:\\FileExample_20210604\\" + file.Directory.Name + "\\" + fileName_2;
                        var fileName_3 = text + " " + data[i] + " " + data[i + 2] + " " + externalSite[0] + "-" + externalSite[1] + "_M" + "File" + "." + pdfSite[1];
                        var fullPath_3 = "c:\\FileExample__20210604\\" + file.Directory.Name + "\\" + fileName_3;
                        if (File.Exists(fullPath_2))
                        {
                            File.Move(fullPath_2, fullPath_3);
                            MergePDF(file.FullName, fullPath_3, fullPath_2);

                            File.Delete(fullPath_3);
                        }

                    }
                    else
                    {
                        var fileName = text + " " + data[i] + " " + data[i + 2] + " " + externalSite[0] + "-" + externalSite[1] + "_" + "File" + "." + pdfSite[1];

                        var fullPath = "c:\\TarafEkleme_20210604\\" + file.Directory.Name + "\\" + fileName;


                        File.Copy(file.FullName, fullPath);
                    }
                }

            }

            Console.ReadKey();

        }
        private static void MergePDF(string File1, string File2, string outputPdfPath)
        {
            string[] fileArray = new string[3];
            fileArray[0] = File1;
            fileArray[1] = File2;

            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;


            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

            //output file Open  
            sourceDocument.Open();


            //files list wise Loop  
            for (int f = 0; f < fileArray.Length - 1; f++)
            {
                int pages = TotalPageCount(fileArray[f]);

                reader = new PdfReader(fileArray[f]);
                //Add pages in new file  
                for (int i = 1; i <= pages; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }

                reader.Close();
            }
            //save the output file  
            sourceDocument.Close();
        }
        private static int TotalPageCount(string file) //page count
        {
            using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(file)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count == 0 ? 1 : matches.Count;
            }
        }
    }
}
