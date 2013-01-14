using System.Collections.Generic;
using Uqbar.ServicesUI.Data;

namespace Uqbar.ServicesUI.ViewModels
{
    public class ServicesEditViewModel : ISettings
    {
        public List<Categoria> Categorias = new List<Categoria>();

        public int IdCategoria = 0;

        public ServicoExtraInfo Data { get; set; }
    }
}