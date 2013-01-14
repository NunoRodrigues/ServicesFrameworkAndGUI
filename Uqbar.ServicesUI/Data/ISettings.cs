using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uqbar.ServicesUI.Data
{
    public interface ISettings
    {
        
    }

    public static class Extensions
    {
        public static string GetFormatDate(this ISettings model)
        {
            return Properties.Settings.Default.FormatDate;
        }

        public static string GetFormatDateTime(this ISettings model)
        {
            return Properties.Settings.Default.FormatDateTime;
        }
    }
}