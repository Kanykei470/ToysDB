using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Ингредиенты
    {
        public byte Id { get; set; }
        public byte? Продукция { get; set; }
        public byte? Сырье { get; set; }
        public short? Количество { get; set; }

        public virtual ГотоваяПродукция ПродукцияNavigation { get; set; }
        public virtual Сырьё СырьеNavigation { get; set; }
    }
}
