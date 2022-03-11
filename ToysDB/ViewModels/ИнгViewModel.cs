using ToysDB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ToysDB.ViewModels
{
    public class ИнгViewModel
    {
        public IEnumerable<Ингредиенты> Ингредиенты { get; set; }
        public SelectList ГотовыйПродукт { get; set; }
        public int? ВыбранныйПродукт { get; set; }
        public string Наименование { get; set; }

    }
}
