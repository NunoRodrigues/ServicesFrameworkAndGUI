using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using HtmlAgilityPack;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.OCR;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.Providers;
using Uqbar.Services.WinSrv.CVM.Output;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.WindowsServices.FIIDadosCadastrais
{
    public class FIIDadosCadastrais : IMessagemProvider
    {
        public event Mensagem.MensagemDelegate NewMessage;

        public Uri URL { get; set; }

        public IProvider DataProvider { get; set; }
        
        private XML output = null;

        private readonly string _encoding = "ISO-8859-1";
        
        public void Perform(ServicoInfo info)
        {
            HtmlDocument doc = GetPagina(this.URL.AbsoluteUri);

            if (doc.DocumentNode != null)
            {
                output = new XML();
                string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";

                HtmlNodeCollection tLista = doc.DocumentNode.SelectNodes(@"(//div[@id='done']//table//tr//a)"); //[position() < 3]

                if (tLista != null)
                {
                    this.NewMessage.Raise(Mensagem.MessageType.Info, "Found " + tLista.Count + " item(s)");

                    foreach (HtmlNode row in tLista)
                    {
                        info.ActionEnd = DateTime.Now; // Serve como indicador de progresso

                        HtmlAttribute attr = row.Attributes["href"];
                        if (attr != null)
                        {
                            string link = attr.Value;

                            GetFicha(link);
                        }

                        output.Save(filename);
                    }
                }
                
                this.NewMessage.Raise(Mensagem.MessageType.Info, "Saved : " + filename);
            }
            
        }

        public HtmlDocument GetPagina(string url)
        {
            return GetPagina(url, 0);
        }

        public HtmlDocument GetPagina(string url, int trys)
        {
            MemoryStream stream = DataProvider.GetStream(url);

            HtmlDocument doc = DataProvider.GetHtml(stream, _encoding);

            if (trys < 30)
            {
                HtmlNode captcha = doc.DocumentNode.SelectSingleNode("//input[@name='strCAPTCHA']");

                if (captcha != null)
                {
                    Uri baseUrl = new Uri(url);

                    HtmlNode imgNode = captcha.ParentNode.SelectSingleNode("img");

                    Uri imgUrl = new Uri(baseUrl, imgNode.Attributes["src"].Value);

                    MemoryStream img = DataProvider.GetStream(imgUrl.AbsoluteUri);

                    string captchaTry = GOCR.Resolve(img);
                    captchaTry = FixCAPTCHAKnownIssues(captchaTry);

                    if (captchaTry != null && captchaTry.IndexOf('_') < 0)
                    {
                        // Try
                        this.NewMessage.Raise(Mensagem.MessageType.Info, "CAPTCHA #" + trys + ": " + captchaTry);

                        string param = "strCAPTCHA";
                        int start = url.IndexOf(param);
                        if (start > 0)
                        {
                            int end = url.IndexOf("&", start) + 1;
                            url = url.Substring(0, start) + url.Substring(end, url.Length - end);
                        }

                        url = url.Replace("?", "?" + param + "=" + captchaTry + "&");
                    }
                    else
                    {
                        this.NewMessage.Raise(Mensagem.MessageType.Warning, "CAPTCHA #" + trys + ": MISSED");
                    }

                    return GetPagina(url, trys + 1);
                }
            }

            return doc;
        }

        public void GetFicha(string url)
        {
            Uri completeUrl = new Uri(this.URL, url);

            // Output
            XmlElement fichaNode = output.AddElement("Ficha", null, completeUrl.AbsoluteUri);

            HtmlDocument doc = GetPagina(completeUrl.AbsoluteUri);

            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection tLinhas = doc.DocumentNode.SelectNodes(@"//table[@id='TbMain']//tr[count(td) > 1]");

                if (tLinhas != null)
                {
                    foreach (HtmlNode row in tLinhas)
                    {
                        HtmlNode labelNode = row.SelectSingleNode("td[1]");
                        HtmlNode valueTextNode = row.SelectSingleNode("td[2]");

                        string label = labelNode.InnerText;
                        string value = valueTextNode.InnerText;

                        output.AddElement(fichaNode, "Campo", label, value);

                        System.Diagnostics.Debug.WriteLine("--- " + label + " ::: " + value);
                    }
                }
            }
        }

        private string FixCAPTCHAKnownIssues(string input)
        {
            if (input != null)
            {
                string output = input.Replace("\r", "");
                output = output.Replace("\n", "");

                output = output.Replace("O", "0");
                output = output.Replace("o", "0");
                output = output.Replace("a", "0");

                output = output.Replace("q", "1");

                output = output.Replace("B", "8");

                output = output.Replace("g", "9");

                return output.Trim();
            }

            return input;
        }

    }
}
