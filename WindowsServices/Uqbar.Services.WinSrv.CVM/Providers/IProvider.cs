using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Uqbar.Services.WinSrv.CVM.Providers
{
    public interface IProvider
    {
        MemoryStream GetStream(string source);
        HtmlDocument GetHtml(MemoryStream stream);
        HtmlDocument GetHtml(string source);
        MemoryStream GetFile(string source, out string filename);
    }
}
