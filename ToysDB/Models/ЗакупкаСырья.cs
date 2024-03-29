﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class ЗакупкаСырья
    {
        public byte Id { get; set; }
        public byte? Сырьё { get; set; }
        public short? Количество { get; set; }
        public decimal? Сумма { get; set; }
        public DateTime Дата { get; set; }
        public int Сотрудник { get; set; }

        public virtual Сотрудники СотрудникNavigation { get; set; }
        public virtual Сырьё СырьёNavigation { get; set; }
    }
}
