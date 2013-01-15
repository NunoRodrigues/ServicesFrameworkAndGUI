using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Uqbar.Services.Framework.Providers
{
    public interface IProvider
    {
        MemoryStream GetStream(string source);
        HtmlDocument GetHtml(MemoryStream stream, string Encoding);
        HtmlDocument GetHtml(string source, string Encoding);
        MemoryStream GetFile(string source, out string filename);
    }
}
