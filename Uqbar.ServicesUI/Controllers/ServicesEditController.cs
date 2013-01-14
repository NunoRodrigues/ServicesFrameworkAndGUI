using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Uqbar.ServicesUI.Data;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.ServicesUI.ViewModels;

namespace Uqbar.ServicesUI.Controllers
{
    public class ServicesEditController : Controller
    {

        public ActionResult Edit(string url)
        {
            // Servico
            Categoria cat;
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url, out cat);
            server.RefreshInfo();

            ServicesEditViewModel model = new ServicesEditViewModel();
            model.Data = server;
            model.IdCategoria = cat.Id;
            model.Categorias = Servicos.Instance.Categorias;

            return View(model);
        }

        public ActionResult SaveInfo(string url, int idCategoria, string name)
        {
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);

            // Info
            server.Name = name;

            Categoria catOld = Servicos.Instance.Categorias.FirstOrDefault(c => c.Servicos.FirstOrDefault(s => s.URL == server.URL) != null);

            if (catOld != null && catOld.Id != idCategoria)
            {
                // Categoria Antiga
                catOld.Servicos.Remove(server);

                // Categoria Nova
                Categoria catNew = Servicos.Instance.Categorias.FirstOrDefault(c => c.Id == idCategoria);
                if (catNew != null)
                {
                    catNew.Servicos.Add(server);
                }
            }

            Servicos.Instance.Save();

            return RedirectToAction("List", new {controller = "ServicesList"});
        }

        public ActionResult Agenda(string url)
        {
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);

            return PartialView(server);
        }

        public ActionResult AgendaAdd(string url, int tipo, DateTime? data, int horas, int minutos, int dia, int[] diaSemana)
        {
            AgendaItem newAgenda = AgendaItem.New();

            // Tipo
            newAgenda.Tipo = (AgendaItem.Periodicidade) tipo;

            // Data
            if (newAgenda.Tipo == AgendaItem.Periodicidade.Pontual)
            {
                newAgenda.Data = data;
            }

            // Horas
            newAgenda.Hora = horas;

            // Minutos
            newAgenda.Minutos = minutos;

            // Dia
            if (newAgenda.Tipo == AgendaItem.Periodicidade.Mensal)
            {
                newAgenda.Dia = dia;
            }

            // Dias da Semana
            if (newAgenda.Tipo == AgendaItem.Periodicidade.Semanal && diaSemana != null)
            {
                newAgenda.DiasSemana  =new List<DayOfWeek>();

                foreach (int item in diaSemana)
                {
                    newAgenda.DiasSemana.Add((DayOfWeek) item);
                }
            }

            // Save
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);
            Agenda agenda = server.GetAgenda();
            agenda.List.Add(newAgenda);
            if (server.SetAgenda(agenda))
            {
                // Info (Refresh)
                server.RefreshInfo();                
            }

            return RedirectToAction("Agenda", new { url = url });
        }

        public ActionResult AgendaRemove(string url, Guid Id)
        {
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);

            Agenda agenda = server.GetAgenda();
            AgendaItem item = agenda.List.FirstOrDefault(a => a.Id == Id);
            if (item != null)
            {
                agenda.List.Remove(item);
                if (server.SetAgenda(agenda))
                {
                    // Info (Refresh)
                    server.RefreshInfo();
                }
            }
            return RedirectToAction("Agenda", new {url = url});
        }

        public ActionResult Mensagens(string url, int start, int length)
        {
            ServicoExtraInfo server = Servicos.Instance.GetByURL(url);

            return PartialView(server.GetMensagens(start, length));
        }
    }
}