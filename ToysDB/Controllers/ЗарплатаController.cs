using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using AspNetCoreHero.ToastNotification;

namespace ToysDB.Controllers
{
    public class ЗарплатаController : Controller
        {
            private readonly ToysContext _context;

            public ЗарплатаController(ToysContext context)
            {
                _context = context;
            }

            // GET: Зарплата



            public void create(string yearString, string monthstring, [Bind("Id,Год,Месяц,Сотрудники,Закуп,Продажа,Производство,Всего,Оклад,Бонус,ОбщаяСуммаКВыдаче,Статус")] Зарплата zp, int sotrId)
            {
                var emp = _context.Сотрудникиs;
                var purchase = _context.ЗакупкаСырьяs;
                var sale = _context.ПродажаПродукцииs;
                var production = _context.Производствоs;
                var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
                int inPurchase, inSale, inProduction;


                var sotr = emp.Where(u => u.Id == sotrId).FirstOrDefault();
                inPurchase = 0;
                inSale = 0;
                inProduction = 0;


                foreach (var raw in purchase)
                {
                    if (raw.Сотрудник == sotr.Id && raw.Дата.Year.ToString().Equals(yearString) && raw.Дата.Month.ToString().Equals(monthstring))
                    {
                        inPurchase++;
                    }
                }
                foreach (var salo in sale)
                {
                    if (salo.Сотрудник == sotr.Id && salo.Дата.Year.ToString().Equals(yearString) && salo.Дата.Month.ToString().Equals(monthstring))
                    {
                        inSale++;
                    }
                }
                foreach (var prod in production)
                {
                    if (prod.Сотрудник == sotr.Id && prod.Дата.Year.ToString().Equals(yearString) && prod.Дата.Month.ToString().Equals(monthstring))
                    {
                        inProduction++;
                    }
                }

                zp.Сотрудники = (byte)sotr.Id;
                zp.Год = yearString;
                zp.Месяц = monthstring;
                zp.Продажа = inSale;
                zp.Производство = inProduction;
                zp.Закуп = inPurchase;
                zp.Всего = inProduction + inPurchase + inSale;
                zp.Оклад = (decimal)sotr.Оклад;
                zp.Бонус = (decimal)budget.Бонус * zp.Всего * (zp.Оклад / 100);
                zp.ОбщаяСуммаКВыдаче = zp.Оклад + zp.Бонус;
                zp.Статус = false;
                _context.Add(zp);
                _context.SaveChanges();
            }



            public async Task<IActionResult> paySalary(string yearString, string monthString)
            {
                var EmpList = from s in _context.Зарплатаs.Include(з => з.СотрудникиNavigation)
                              select s;
                EmpList = EmpList.Where(s => s.Год == yearString && s.Месяц == monthString);

                var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
                decimal totalSalary = 0;

                foreach (var emp in EmpList)
                {
                    if (emp.Статус == false)
                    {
                        totalSalary += emp.ОбщаяСуммаКВыдаче;
                    }
                }

                if (totalSalary > budget.Сумма)
                {
                    return NotFound("Не хватает денег!");
                }
                else
                {
                    foreach (var emp in EmpList)
                    {
                        if (emp.Статус == false)
                        {
                            emp.Статус = true;
                        }
                    }

                    budget.Сумма -= totalSalary;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

        public IActionResult Index(string yearString, string monthstring)
        {
            ViewBag.years = new SelectList(
                    new List<SelectListItem>
                    {
            new SelectListItem {Text = "2020", Value = "2020"},
            new SelectListItem {Text = "2021", Value = "2021"},
            new SelectListItem {Text = "2022", Value = "2022"},
            new SelectListItem {Text = "2023", Value = "2023"},
            new SelectListItem {Text = "2024", Value = "2024"},
            new SelectListItem {Text = "2025", Value = "2025"},
                    }, "Value", "Text");

            ViewBag.months = new SelectList(
        new List<SelectListItem>
        {
            new SelectListItem {Text = "Январь", Value = "1"},
            new SelectListItem {Text = "Февраль", Value = "2"},
            new SelectListItem {Text = "Март", Value = "3"},
            new SelectListItem {Text = "Апрель", Value = "4"},
            new SelectListItem {Text = "Май", Value = "5"},
            new SelectListItem {Text = "Июнь", Value = "6"},
            new SelectListItem {Text = "Июль", Value = "7"},
            new SelectListItem {Text = "Август", Value = "8"},
            new SelectListItem {Text = "Сентябрь", Value = "9"},
            new SelectListItem {Text = "Октябрь", Value = "10"},
            new SelectListItem {Text = "Ноябрь", Value = "11"},
            new SelectListItem {Text = "Декабрь", Value = "12"}
        }, "Value", "Text");



            var EmpList = from s in _context.Зарплатаs.Include(з => з.СотрудникиNavigation)
                          select s;

            if (!String.IsNullOrEmpty(yearString) && !String.IsNullOrEmpty(monthstring))
            {
                EmpList = EmpList.Where(s => s.Год == yearString && s.Месяц == monthstring);
                int countOfEmp = 0;
                foreach (var item in EmpList)
                {
                    countOfEmp++;
                    if (countOfEmp == 1) break;
                }

                if (countOfEmp == 0)
                {
                    var emp = _context.Сотрудникиs;
                    List<byte> EmpIdList = new List<byte>();

                    foreach (var sotr in emp)
                    {
                        EmpIdList.Add((byte)sotr.Id);

                    }
                    for (int i = 0; i < EmpIdList.Count(); i++)
                    {
                        int sotrId = EmpIdList[i];
                        Зарплата зарплата = new Зарплата();
                        create(yearString, monthstring, зарплата, sotrId);
                    }
                }
                return View(EmpList.ToList());
            }
            else
            {

                return View(EmpList.ToList());
            }
        }
    }
    }