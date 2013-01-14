using System;
using HtmlAgilityPack;
using Uqbar.Services.WinSrv.CVM.Providers;

namespace Uqbar.Services.WinSrv.CVM.Scrappers
{
    public class FIIDemonstraçõesFinanceiras : IScrapper
    {
        
        public string URL { get; set; }
        
        public IProvider DataProvider { get; set; }

        public void Perform()
        {
            HtmlDocument doc = DataProvider.GetHtml(this.URL);
            Uri baseAddress = new Uri(this.URL);

            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection tLista = doc.DocumentNode.SelectNodes(@"//table//tr/td/a");

                foreach (HtmlNode row in tLista)
                {
                    HtmlAttribute attr = row.Attributes["href"];
                    if (attr != null)
                    {
                        string title = row.InnerText;
                        string link = attr.Value;

                        GetFicha(baseAddress, link);

                        System.Diagnostics.Debug.WriteLine(title + " ::: " + attr.Value);
                    }
                }
            }
        }

        public void GetFicha(Uri baseAddress, string link)
        {
            Uri url = new Uri(baseAddress, link);
            HtmlDocument doc = DataProvider.GetHtml(url.AbsoluteUri);

            if (doc.DocumentNode != null)
            {
                // Fields
                HtmlNodeCollection rows = doc.DocumentNode.SelectNodes(@"//table//tr");

                if (rows != null)
                {
                    foreach (HtmlNode row in rows)
                    {
                        HtmlNodeCollection cols = row.SelectNodes("td[b]");
                        HtmlNodeCollection values = row.SelectNodes("td[span|select|a]");

                        if (cols != null && values != null)
                        {
                            if (cols.Count == values.Count)
                            {
                                for (int i = 0; i < values.Count; i++)
                                {
                                    GetCell(cols[i], values[i]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetCell(HtmlNode nodeLabel, HtmlNode nodeValue)
        {
            HtmlNode label = nodeLabel.SelectSingleNode("b");

            HtmlNode value = nodeValue.SelectSingleNode("span");
            if (value == null) value = nodeValue.SelectSingleNode("select/option[@selected]");
            if (value == null) value = nodeLabel.SelectSingleNode("a");

            if (label != null && value != null)
            {
                if (value.Name == "option")
                {
                    if (string.IsNullOrEmpty(value.InnerText) == false)
                    {
                        System.Diagnostics.Debug.WriteLine("--- " + label.InnerText + " ::: " + value.InnerText);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("--- " + label.InnerText + " ::: " + value.Attributes["value"].Value);
                    }
                }
                else if (value.Name == "a")
                {
                    string link = value.Attributes["href"].Value;

                    if (link.Contains("__doPostBack"))
                    {
                        string filename;
                        DataProvider.GetFile(link, out filename);
                        System.Diagnostics.Debug.WriteLine("--- " + label.InnerText + " ::: " + link);
                    }

                    System.Diagnostics.Debug.WriteLine("--- " + label.InnerText + " ::: " + link);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("--- " + label.InnerText + " ::: " + value.InnerText);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- SHITE!");
            }
        }
    }
}
