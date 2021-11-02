using org.pdfclown.bytes;
using org.pdfclown.objects;


namespace web.Handlers
{
    public static class PdfClownHandle
    {
        public static int CountImages(string filePath)
        {
            return CountImages(FileStreamHandle.GetAsByteArray(filePath));
        }

        public static int CountImages(byte[] fileBytes)
        {
            int count = 0;
            IInputStream iInputStream = new org.pdfclown.bytes.Buffer(fileBytes);
            using (org.pdfclown.files.File file = new org.pdfclown.files.File(iInputStream))
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
