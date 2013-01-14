using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.WinSrv.CVM.Providers;

namespace Uqbar.Services.WinSrv.CVM.Scrappers
{
    public interface IScrapper
    {
        string URL { get; set; }

        IProvider DataProvider { get; set; }

        void Perform();
    }
}
