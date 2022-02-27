using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class ПродажаПродукции
    {
        public byte Id { get; set; }
        public byte? Продукция { get; set; }
        public float? Количество { get; set; }
        public decimal? Сумма { get; set; }
        public DateTime? Дата { get; set; }
        public byte? Сотрудник { get; set; }

        public virtual ГотоваяПродукция ПродукцияNavigation { get; set; }
        public virtual Сотрудники СотрудникNavigation { get; set; }
    }
}
