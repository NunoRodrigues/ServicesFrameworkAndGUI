using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Uqbar.Services.Framework;

namespace Uqbar.Services.WinSrv.CVM.Providers
{
    public class Web : IProvider
    {
        private CookieContainer cookieJar = new CookieContainer();
        private HttpWebResponse _lastResponse = null;
        private HtmlDocument _lastDocument = null;
        private string _lastURL = null;

        public MemoryStream GetStream(string source)
        {
            if (_lastResponse != null)
            {
                _lastResponse.Close();
                _lastResponse.Dispose();
            }

            System.Diagnostics.Debug.WriteLine(source);

            HttpWebRequest request;
            if (source.Contains("__doPostBack"))
            {
                // .NET Form PostBack
                request = NewRequest(GetFormAction(_lastURL, _lastDocument), true);
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryfYX4SXa4fi5hfg9D";
                request.KeepAlive = true;

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    // Gets Parameters
                    Dictionary<string, string> values = GetFormInputs(_lastDocument);

                    // Updates Parameters
                    int lastIndex = 0;
                    values["__EVENTTARGET"] = getSingleParam(lastIndex, source, out lastIndex);
                    values["__EVENTARGUMENT"] = getSingleParam(lastIndex, source, out lastIndex);

                    writer.Write(getPostParameters(values));
                }

                System.Net.ServicePointManager.Expect100Continue = false;
            }
            else
            {
                _lastURL = source;

                // Vanila Web Request
                request = NewRequest(source, false);
                request.Method = "GET";
            }
            
            _lastResponse = (HttpWebResponse)request.GetResponse();
            return GetStream(_lastResponse.GetResponseStream());
        }

        public HtmlDocument GetHtml(MemoryStream stream)
        {
            stream.WriteFile(@"c:\ToDelete\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");

            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            _lastDocument = new HtmlDocument();
            _lastDocument.LoadHtml(text);
            return _lastDocument;
        }

        public HtmlDocument GetHtml(string source)
        {
            return GetHtml(GetStream(source));
        }

        public MemoryStream GetFile(string source, out string filename)
        {
            MemoryStream dataStream = GetStream(source);

            filename = getFilename(_lastResponse);

            dataStream.WriteFile(@"c:\ToDelete\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");

            dataStream.Close();

            return dataStream;
        }

        private HttpWebRequest NewRequest(string url, bool keepalive)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //request.Host = "cvmweb.cvm.gov.br";
            //request.Headers.Set("Origin", @"http://cvmweb.cvm.gov.br");
            string origin = url.Substring(0, url.IndexOf(request.Host) + request.Host.Length);
            request.Headers.Set("Origin", origin);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Referer = _lastURL;
            request.Headers.Set("Accept-Encoding", "gzip,deflate,sdch");
            request.Headers.Set("Accept-Language", "en-GB,en-US;q=0.8,en;q=0.6,pt-PT;q=0.4");
            request.Headers.Set("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            request.CookieContainer = cookieJar;
            request.Headers.Set("Cache-Control", "max-age=0");

            if (keepalive)
            {
                request.KeepAlive = true;
                // Força o Keep-Alive
                var sp = request.ServicePoint;
                var prop = sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic);
                prop.SetValue(sp, (byte) 0, null);
            }

            return request;
        }

        #region .NET Forms

        private string GetFormAction(string baseUrl, HtmlDocument doc)
        {
            HtmlNode form = doc.DocumentNode.SelectSingleNode("//form");
            string action = form.Attributes["action"].Value;
            return new Uri(new Uri(baseUrl), HttpUtility.HtmlDecode(action)).AbsoluteUri;
        }

        private Dictionary<string, string> GetFormInputs(HtmlDocument doc)
        {
            var inputs = doc.DocumentNode.SelectNodes("//input[not(contains(@type,'button') or contains(@type,'submit'))]|//select");

            if (inputs != null)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (HtmlNode element in inputs)
                {
                    string name = element.GetAttributeValue("name", "undefined");

                    string value;
                    if (element.Name.ToLower() == "select")
                    {
                        HtmlNode selectedValue = element.SelectSingleNode("option[@selected]");
                        value = selectedValue.GetAttributeValue("value", "");
                    }
                    else
                    {
                        value = element.GetAttributeValue("value", "");
                    }

                    if (!name.Equals("undefined"))
                        result.Add(name, value);
                }

                return result;
            }

            return null;
        }

        #endregion

        private string getPostParameters(Dictionary<string, string> col)
        {
            if (col != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> element in col)
                {
                    string value = element.Value;
                    sb.Append("------WebKitFormBoundaryfYX4SXa4fi5hfg9D\nContent-Disposition: form-data; name=\"");
                    sb.Append(element.Key);
                    sb.Append("\"\n\n");
                    sb.Append(value);
                    sb.Append("\n");
                }

                sb.Append("------WebKitFormBoundaryfYX4SXa4fi5hfg9D--");
                return sb.ToString();
            }

            return "";
        }

        private string getSingleParam(int startIndex, string source, out int lastIndex)
        {
            int firstIndex = source.IndexOf('\'', startIndex) + 1;
            lastIndex = source.IndexOf('\'', firstIndex);

            if (lastIndex - firstIndex > 1)
            {
                return source.Substring(firstIndex, lastIndex - firstIndex);
            }

            return "";
        }

        private string getFilename(HttpWebResponse response)
        {
            string raw = response.Headers["content-disposition"];
            string name = "filename=";
            if (string.IsNullOrEmpty(raw) == false && raw.Contains(name))
            {
                int startIndex = raw.IndexOf(name) + name.Length;
                int endIndex = raw.Length - raw.IndexOf("&", startIndex);
                if (endIndex >= raw.Length) endIndex = raw.Length - startIndex;

                string result = raw.Substring(startIndex, endIndex);
                return result;
            }

            return Guid.NewGuid().ToString();
        }

        private MemoryStream GetStream(Stream stream)
        {
            MemoryStream mem = new MemoryStream();

            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                mem.Write(buffer, 0, len);
            }

            mem.Position = 0;

            return mem;
        }
    }
}
