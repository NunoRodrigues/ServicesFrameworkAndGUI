using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Uqbar.Services.WinSrv.CVM.Providers;

namespace Uqbar.Services.WinSrv.CVM.Scrappers
{
    public class FIIDadosCadastrais : IScrapper
    {
        public string URL { get; set; }

        public IProvider DataProvider { get; set; }
        
        public void Perform()
        {
            HtmlDocument doc = DataProvider.GetHtml(this.URL);

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

        public void GetFicha(string url)
        {
            // TODO REMOVE
            url = "ResultBuscaDocsFdo01.htm";

            HtmlDocument doc = DataProvider.GetHtml(url);

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
    }
}
