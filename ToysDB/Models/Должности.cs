using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Должности
    {
        public Должности()
        {
            Сотрудникиs = new HashSet<Сотрудники>();
        }

        public byte Id { get; set; }
        public string Должность { get; set; }

        public virtual ICollection<Сотрудники> Сотрудникиs { get; set; }
    }
}
