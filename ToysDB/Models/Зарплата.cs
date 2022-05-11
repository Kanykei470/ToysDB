namespace ToysDB.Models
{
    public class Зарплата
    {
        public int Id { get; set; }
        public string Год { get; set; }
        public string Месяц { get; set; }
        public int Сотрудники { get; set; }
        public int Закуп { get; set; }
        public int Продажа { get; set; }
        public int Производство { get; set; }
        public int Всего { get; set; }
        public decimal Оклад { get; set; }
        public decimal Бонус { get; set; }
        public decimal ОбщаяСуммаКВыдаче { get; set; }
        public bool Статус { get; set; }

        public virtual Сотрудники СотрудникиNavigation { get; set; }
    }


}
