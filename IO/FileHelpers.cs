using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirona.Utilities.IO
{
    public static class FileHelpers
    {
        public static Byte[] LoadFile(String filename)
        {
            // Open the file as a FileStream object.
            System.IO.FileStream infile = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            Byte[] document = new Byte[infile.Length];
            // Read the file to ensure it is readable.
            int count = infile.Read(document, 0, document.Length);
            if (count != document.Length)
            {
                infile.Close();
                Console.WriteLine("Test Failed: Unable to read data from file");
                return null;
            }
            infile.Close();
            return document;
        }

    }
}
