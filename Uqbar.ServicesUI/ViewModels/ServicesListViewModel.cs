using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uqbar.ServicesUI.Data;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.ServicesUI.ViewModels
{
    public class ServicesListViewModel : ISettings
    {
        public List<Categoria> Categorias = new List<Categoria>();

        public int IdCategoria { get; set; }

        public ServicoInfo.EstadosEnum IdEstado { get; set; }
    }
}