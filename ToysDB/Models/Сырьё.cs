using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Сырьё
    {
        public Сырьё()
        {
            ЗакупкаСырьяs = new HashSet<ЗакупкаСырья>();
            Ингредиентыs = new HashSet<Ингредиенты>();
        }

        public byte Id { get; set; }
        public string Наименование { get; set; }
        public byte? ЕдиницаИзмерения { get; set; }
        public decimal? Сумма { get; set; }
        public short? Количество { get; set; }

        public virtual ЕдиницыИзмерения ЕдиницаИзмеренияNavigation { get; set; }
        public virtual ICollection<ЗакупкаСырья> ЗакупкаСырьяs { get; set; }
        public virtual ICollection<Ингредиенты> Ингредиентыs { get; set; }
    }
}
