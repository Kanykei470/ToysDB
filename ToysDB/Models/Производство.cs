using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Производство
    {
        public string Id { get; set; }
        public byte? Продукция { get; set; }
        public short? Количество { get; set; }
        public DateTime? Дата { get; set; }
        public byte? Сотрудник { get; set; }

        public virtual ГотоваяПродукция ПродукцияNavigation { get; set; }
        public virtual Сотрудники СотрудникNavigation { get; set; }
    }
}
