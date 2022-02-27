using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class ГотоваяПродукция
    {
        public ГотоваяПродукция()
        {
            Ингредиентыs = new HashSet<Ингредиенты>();
            ПродажаПродукцииs = new HashSet<ПродажаПродукции>();
            Производствоs = new HashSet<Производство>();
        }

        public byte Id { get; set; }
        public string Наименование { get; set; }
        public byte? ЕдиницаИзмерения { get; set; }
        public decimal? Сумма { get; set; }
        public short? Количество { get; set; }

        public virtual ЕдиницыИзмерения ЕдиницаИзмеренияNavigation { get; set; }
        public virtual ICollection<Ингредиенты> Ингредиентыs { get; set; }
        public virtual ICollection<ПродажаПродукции> ПродажаПродукцииs { get; set; }
        public virtual ICollection<Производство> Производствоs { get; set; }
    }
}
