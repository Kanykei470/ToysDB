using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class ЕдиницыИзмерения
    {
        public ЕдиницыИзмерения()
        {
            ГотоваяПродукцияs = new HashSet<ГотоваяПродукция>();
            Сырьёs = new HashSet<Сырьё>();
        }

        public byte Id { get; set; }
        public string Наименование { get; set; }

        public virtual ICollection<ГотоваяПродукция> ГотоваяПродукцияs { get; set; }
        public virtual ICollection<Сырьё> Сырьёs { get; set; }
    }
}
