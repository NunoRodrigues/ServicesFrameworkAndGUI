using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uqbar.Services.Framework;

namespace Uqbar.ServicesUI.Data
{
    public class Servicos
    {
        private static string _instanceKey = "Servicos";
        private static Servicos _instance = null;

        public static Servicos Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (HttpContext.Current.Application.AllKeys.Contains(_instanceKey) == false)
                    {
                        _instance = Configuration.Get<Servicos>();

                        if (_instance == null)
                        {
                            // First Time ever run
                            _instance = freshInstance();

                            _instance.Save();
                        }
                        else
                        {
                            // Get data from file
                            _instance.LastSet = Configuration.LastSet<Servicos>();

                            HttpContext.Current.Application.Add(_instanceKey, _instance);
                        }
                    }
                    else
                    {
                        // Get data from Application Instance
                        _instance = HttpContext.Current.Application[_instanceKey] as Servicos;
                    }
                }

                return _instance;
            }
        }

        private DateTime LastSet { get; set; }

        public List<Categoria> Categorias = null;

        public void Save()
        {
            _instance.LastSet = Configuration.Set(_instance);
        }

        public ServicoExtraInfo GetByURL(string url)
        {
            Categoria cat;

            return GetByURL(url, out cat);
        }

        public ServicoExtraInfo GetByURL(string url, out Categoria cat)
        {
            cat = this.Categorias.FirstOrDefault(c => c.Servicos != null && c.Servicos.FirstOrDefault(s => s.URL == url) != null);

            return cat.Servicos.FirstOrDefault(s => s.URL == url);
        }

        public ServicoExtraInfo GetBySafeId(string id)
        {
            Categoria cat = this.Categorias.FirstOrDefault(c => c.Servicos != null && c.Servicos.FirstOrDefault(s => s.SafeID == id) != null);

            return cat.Servicos.FirstOrDefault(s => s.SafeID == id);
        }

        private static Servicos freshInstance()
        {
            Servicos result = new Servicos();
            result.Categorias = new List<Categoria>();

            //result.Categorias.Add(Categoria.Zero, new List<ServicoInfo>() { new ServicoInfo() { Uid = Guid.Empty, Estado = ServicoInfo.EstadosEnum.Parado, Name = "Sample Service", URL = "http://localhost:4444/" } });
            result.Categorias.Add(Categoria.Zero);
            return result;
        }
    }
}