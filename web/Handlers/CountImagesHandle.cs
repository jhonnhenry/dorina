using org.pdfclown.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using pdfclown = org.pdfclown.files;

namespace web.Handlers
{
    public static class CountImagesHandle
    {
        public static int GetTotal(string filePath)
        {
            int count = 0;
            using (pdfclown::File file = new pdfclown::File(filePath))
            {
                foreach (PdfIndirectObject indirectObject in file.IndirectObjects)
                {
                    // Get the data object associated to the indirect object!
                    PdfDataObject dataObject = indirectObject.DataObject;
                    // Is this data object a stream?
                    if (dataObject is PdfStream)
                    {
                        PdfDictionary header = ((PdfStream)dataObject).Header;
                        // Is this stream an image?
                        if (header.ContainsKey(PdfName.Type)
                          && header[PdfName.Type].Equals(PdfName.XObject)
                          && header[PdfName.Subtype].Equals(PdfName.Image))
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }
}
