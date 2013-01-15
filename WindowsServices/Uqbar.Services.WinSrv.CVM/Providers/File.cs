using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Uqbar.Services.WinSrv.CVM.Providers
{
    public class File : IProvider
    {
        public MemoryStream GetStream(string source)
        {
            /*
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\DataFiles\", source);

            StreamReader streamReader = new StreamReader(path);
            return new MemoryStream(streamReader.BaseStream.);
            */

            return null;
        }

        public HtmlDocument GetHtml(string source)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(GetStream(source).ToString());
            return doc;
        }

        public MemoryStream GetFile(string source, out string filename)
        {
            /*
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\DataFiles\", source);

            StreamReader streamReader = new StreamReader(path);
            string html = streamReader.ReadToEnd();
            streamReader.Close();

            filename = "";
             
            return html;
            */

            filename = "";
            return null;
            
        }

        public HtmlDocument GetHtml(MemoryStream stream)
        {
            return null;
        }
    }
}
