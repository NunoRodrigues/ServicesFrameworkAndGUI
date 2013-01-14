using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using Uqbar.ServicesUI.Data;
using Uqbar.ServicesUI.ViewModels;

namespace Uqbar.ServicesUI.Controllers
{
    public class ServicesListController : Controller
    {
        //
        // GET: /ServiceList/

        public ActionResult List()
        {
            ServicesListViewModel model = new ServicesListViewModel();
            model.Categorias = Servicos.Instance.Categorias;
            return View(model);
        }

        public ActionResult Item(string url)
        {
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);

            if (server != null)
            {
                server.RefreshInfo();

                return PartialView(server);
            }

            return null;
        }
    }
}
