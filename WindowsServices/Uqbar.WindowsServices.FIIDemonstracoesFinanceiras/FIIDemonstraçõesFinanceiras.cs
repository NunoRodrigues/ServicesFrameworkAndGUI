using System;
using HtmlAgilityPack;
using System.Xml;
using System.IO;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.Providers;
using Uqbar.Services.WinSrv.CVM.Output;

namespace Uqbar.Services.WinSrv.CVM.Scrappers
{
    public class FIIDemonstraçõesFinanceiras : IMessagemProvider
    {

        public Uri URL { get; set; }
        
        public IProvider DataProvider { get; set; }

        public event Mensagem.MensagemDelegate NewMessage;

        private XML output = null;

        private readonly string _enconding = "ISO-8859-1";

        public void Perform()
        {
            this.NewMessage.Raise(Mensagem.MessageType.Info, "Started");

            HtmlDocument doc = DataProvider.GetHtml(this.URL.AbsoluteUri, _enconding);

            if (doc.DocumentNode != null)
            {
                output = new XML();
                string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";

                HtmlNodeCollection tLista = doc.DocumentNode.SelectNodes(@"(//table//tr/td/a)");

                if (tLista != null)
                {
                    this.NewMessage.Raise(Mensagem.MessageType.Info, "Found " + tLista.Count + " item(s)");

                    foreach (HtmlNode row in tLista)
                    {
                        HtmlAttribute attr = row.Attributes["href"];
                        if (attr != null)
                        {
                            string title = row.InnerText;
                            string link = attr.Value;

                            GetFicha(link);
                        }

                        output.Save(filename);
                    }
                }

                this.NewMessage.Raise(Mensagem.MessageType.Info, "Saved : " + filename);
            }
        }

        public void GetFicha(string link)
        {
            Uri completeUrl = new Uri(this.URL, link);

            // Output
            XmlElement fichaNode = output.AddElement("Ficha", null, completeUrl.AbsoluteUri);

            HtmlDocument doc = DataProvider.GetHtml(completeUrl.AbsoluteUri, _enconding);

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
                                    GetCell(fichaNode, cols[i], values[i]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetCell(XmlElement outputParent, HtmlNode nodeLabel, HtmlNode nodeValue)
        {
            HtmlNode label = nodeLabel.SelectSingleNode("b");

            HtmlNode value = nodeValue.SelectSingleNode("span");
            if (value == null) value = nodeValue.SelectSingleNode("select/option[@selected]");
            if (value == null) value = nodeLabel.SelectSingleNode("a");

            if (label != null && value != null)
            {
                string nome = label.InnerText;
                string valor = null;
                if (value.Name == "option")
                {
                    if (string.IsNullOrEmpty(value.InnerText) == false)
                        valor = value.InnerText;
                    else
                        valor = value.Attributes["value"].Value;
                }
                else if (value.Name == "a")
                {
                    string link = value.Attributes["href"].Value;

                    if (link.Contains("__doPostBack"))
                    {
                        string filename;
                        MemoryStream file = DataProvider.GetFile(link, out filename);

                        nome = filename;
                        valor = Convert.ToBase64String(file.ToArray());
                    }
                }
                else
                {
                    valor = value.InnerText;
                }

                XmlElement fichaNode = output.AddElement(outputParent, "Campo", nome, valor);

                System.Diagnostics.Debug.WriteLine("--- " + nome + " ::: " + ((valor != null) ? valor : ""));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- SHITE!");
            }
        }
    }
}
