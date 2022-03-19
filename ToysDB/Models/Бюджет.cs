using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Бюджет
    {
        public byte Id { get; set; }
        public decimal? Сумма { get; set; }
        public short Процент { get; set; }
        public decimal? Бонус { get; set; }
    }
}
