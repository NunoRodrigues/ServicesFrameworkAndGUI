using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Uqbar.Services.WinSrv.CVM.Output
{
    public class XML
    {
        private XmlDocument _xml = new XmlDocument();

        public XML()
        {
            XmlElement root = _xml.CreateElement("Fichas", null);
            _xml.AppendChild(root);
        }

        public XmlElement AddElement(string name)
        {
            return AddElement(_xml.FirstChild as XmlElement, name, null, null);
        }

        public XmlElement AddElement(string name, string nome, string valor)
        {
            return AddElement(_xml.FirstChild as XmlElement, name, nome, valor);
        }

        public XmlElement AddElement(XmlElement parent, string name, string nome, string valor)
        {
            XmlElement element = _xml.CreateElement(name, null);
            if (string.IsNullOrEmpty(nome) == false) { element.SetAttribute("Nome", nome); }
            if (string.IsNullOrEmpty(valor) == false) { element.SetAttribute("Valor", valor); }

            parent.AppendChild(element);

            return element;
        }

        public void Save(string filename)
        {
            _xml.Save(filename);
        }
    }
}
