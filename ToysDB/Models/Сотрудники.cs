using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class Сотрудники
    {
        public Сотрудники()
        {
            ЗакупкаСырьяs = new HashSet<ЗакупкаСырья>();
            ПродажаПродукцииs = new HashSet<ПродажаПродукции>();
            Производствоs = new HashSet<Производство>();
        }

        public int Id { get; set; }
        public string Фио { get; set; }
        public byte? Должность { get; set; }
        public decimal? Оклад { get; set; }
        public string Адрес { get; set; }
        public string Телефон { get; set; }

        public virtual Должности ДолжностьNavigation { get; set; }
        public virtual ICollection<ЗакупкаСырья> ЗакупкаСырьяs { get; set; }
        public virtual ICollection<ПродажаПродукции> ПродажаПродукцииs { get; set; }
        public virtual ICollection<Производство> Производствоs { get; set; }
        public virtual ICollection <Зарплата> Зарплатаs { get; set; }
    }
}
