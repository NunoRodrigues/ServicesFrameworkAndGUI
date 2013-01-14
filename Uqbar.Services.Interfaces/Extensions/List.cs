using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Uqbar.Services.Framework
{
    public static partial class Extensions
    {
        public static List<SelectListItem> ToList(this Enum enumObj, string emptyValue, string emptyName)
        {
            var values = from object e in Enum.GetValues(enumObj.GetType())
                         select new SelectListItem() { Value = ((int)e).ToString(), Text = e.ToString() };

            List<SelectListItem> list = new List<SelectListItem>();

            if (string.IsNullOrEmpty(emptyValue) == false || string.IsNullOrEmpty(emptyName) == false)
            {
                list.Add(new SelectListItem() { Value = emptyValue, Text = emptyName });
            }

            list.AddRange(values.ToArray());

            return list;
        }

        public static List<SelectListItem> ToList(this Enum enumObj)
        {
            return enumObj.ToList(null, null);
        }

        public static SelectList ToSelectList(this Enum enumObj, string emptyValue, string emptyName)
        {
            List<SelectListItem> values = enumObj.ToList(emptyValue, emptyName);

            return new SelectList(values, "Value", "Text", enumObj);
        }

        public static SelectList ToSelectList(this Enum enumObj)
        {
            return enumObj.ToSelectList(null, null);
        }
    }
}
