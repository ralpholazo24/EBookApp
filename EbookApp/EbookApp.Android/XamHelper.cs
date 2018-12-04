using EbookApp.Droid;
using iTextSharp.text.pdf;
using System.Text;
using GemBox.Document;

[assembly: Xamarin.Forms.Dependency(typeof(XamHelper))]

namespace EbookApp.Droid
{
    public class XamHelper : IXamHelper
    {
        public string PDTtoText(string fileName)
        {
            var reader = new PdfReader(fileName);

            StringBuilder sb = new StringBuilder();

            try
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    var cpage = reader.GetPageN(page);
                    var content = cpage.Get(PdfName.CONTENTS);

                    var ir = (PRIndirectReference)content;

                    var value = reader.GetPdfObject(ir.Number);

                    if (value.IsStream())
                    {
                        PRStream stream = (PRStream)value;

                        var streamBytes = PdfReader.GetStreamBytes(stream);

                        var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(streamBytes));

                        try
                        {
                            while (tokenizer.NextToken())
                            {
                                if (tokenizer.TokenType == PRTokeniser.TK_STRING)
                                {
                                    string str = tokenizer.StringValue;
                                    sb.Append(str.Replace("en-US", "\n"));
                                }
                            }
                        }
                        finally
                        {
                            tokenizer.Close();
                        }
                    }
                }
            }
            finally
            {
                reader.Close();
            }

            return sb.ToString();

        }
         
        public string WordToText(string filename)
        {
      
            // If using Professional version, put your serial key below.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            DocumentModel document = DocumentModel.Load(filename);

            var sb = new StringBuilder();

            // Get content from each paragraph
            foreach (Paragraph paragraph in document.GetChildElements(true, ElementType.Paragraph))
            {
                sb.AppendFormat("{0}", paragraph.Content.ToString());
                sb.AppendLine();
            }

            //// Get content from each bold run
            //foreach (Run run in document.GetChildElements(true, ElementType.Run))
            //{
            //    if (run.CharacterFormat.Bold)
            //    {
            //        sb.AppendFormat("{0}", run.Content.ToString());
            //        sb.AppendLine();
            //    }
            //}

            return sb.ToString();

        }






    }


}