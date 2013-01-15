using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.ServicesUI.Data
{
    public class Categoria
    {
        public static Categoria Zero = new Categoria() { Id = 1, Nome = "Data Scrapping", Servicos = new List<ServicoExtraInfo>() { new ServicoExtraInfo() { Name = "Sample", Estado = ServicoInfo.EstadosEnum.Parado, URL = "http://localhost:8181" } } };

        public int Id { get; set; }

        public string Nome { get; set; }

        public List<ServicoExtraInfo> Servicos = new List<ServicoExtraInfo>();
    }
}