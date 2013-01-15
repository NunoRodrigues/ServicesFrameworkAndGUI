using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using Uqbar.Services.WinSrv.CVM.Providers;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.OCR;

namespace Uqbar.Services.WinSrv.CVM.Scrappers
{
    public class FIIDadosCadastrais : IScrapper
    {
        public string URL { get; set; }

        public IProvider DataProvider { get; set; }
        
        public void Perform()
        {
            HtmlDocument doc = GetPagina(this.URL);

            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection tLista = doc.DocumentNode.SelectNodes(@"//div[@id='done']//table//tr//a");

                foreach (HtmlNode row in tLista)
                {
                    HtmlAttribute attr = row.Attributes["href"];
                    if (attr != null)
                    {
                        string title = row.InnerText;
                        string link = attr.Value;

                        GetFicha(link);

                        System.Diagnostics.Debug.WriteLine(title + " ::: " + link);
                    }
                }
            }
        }

        public HtmlDocument GetPagina(string url)
        {
            return GetPagina(url, 0);
        }

        public HtmlDocument GetPagina(string url, int trys)
        {
            MemoryStream stream = DataProvider.GetStream(url);

            HtmlDocument doc = DataProvider.GetHtml(stream);

            if (trys < 30)
            {
                HtmlNode captcha = doc.DocumentNode.SelectSingleNode("//input[@name='strCAPTCHA']");

                if (captcha != null)
                {
                    Uri baseUrl = new Uri(url);

                    HtmlNode imgNode = captcha.ParentNode.SelectSingleNode("img");

                    Uri imgUrl = new Uri(baseUrl, imgNode.Attributes["src"].Value);

                    MemoryStream img = DataProvider.GetStream(imgUrl.AbsoluteUri);

                    string result = FixCAPTCHAKnownIssues(GOCR.Resolve(img));

                    if (result != null && result.IndexOf('_') < 0)
                    {
                        // Try
                        string param = "strCAPTCHA";
                        int start = url.IndexOf(param);
                        if (start > 0)
                        {
                            int end = url.IndexOf("&", start) + 1;
                            url = url.Substring(0, start) + url.Substring(end, url.Length - end);
                        }

                        url = url.Replace("?", "?" + param + "=" + result + "&");
                    }

                    return GetPagina(url, trys + 1);
                }
            }

            return doc;
        }

        public void GetFicha(string url)
        {
            // TODO REMOVE
            url = "ResultBuscaDocsFdo01.htm";

            HtmlDocument doc = GetPagina(url);

            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection tLinhas = doc.DocumentNode.SelectNodes(@"//table[@id='TbMain']//tr[count(td) > 1]");

                foreach (HtmlNode row in tLinhas)
                {
                    HtmlNode labelNode = row.SelectSingleNode("td[1]");
                    HtmlNode valueTextNode = row.SelectSingleNode("td[2]");

                    string label = labelNode.InnerText;
                    string value = valueTextNode.InnerText;

                    System.Diagnostics.Debug.WriteLine("--- " + label + " ::: " + value);
                }
            }
        }

        private string FixCAPTCHAKnownIssues(string input)
        {
            if (input != null)
            {
                string output = input.Replace("B", "8");
                output = output.Replace("O", "0");
                output = output.Replace("g", "9");

                output = output.Replace("\r", "");                
                output = output.Replace("\n", "");

                return output.Trim();
            }

            return input;
        }
    }
}
