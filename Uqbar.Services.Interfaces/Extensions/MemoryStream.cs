using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework
{
    public static partial class Extensions
    {
        public static void WriteFile(this MemoryStream stream, string filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.OpenOrCreate))
            {
                stream.CopyTo(file);
                file.Flush();
                file.Close();
            }

            stream.Position = 0;
        }
    }
}
