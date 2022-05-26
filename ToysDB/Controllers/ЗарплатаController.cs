﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using AspNetCoreHero.ToastNotification;
using Microsoft.Data.SqlClient;

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

        public async Task<IActionResult> Index(string yearString, string monthString)
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

            var months = new List<string>()
            {
                "Январь", "Февраль",
                "Март","Апрель","Май","Июнь","Июль","Август",
                "Сентябрь","Октябрь","Ноябрь","Декабрь"
            };
            List<Зарплата> SalaryList;
            if (!String.IsNullOrEmpty(yearString) && !String.IsNullOrEmpty(monthString))
            {
                SqlParameter year = new SqlParameter("@y", Convert.ToInt32(yearString));
                SqlParameter month = new SqlParameter("@m", Convert.ToInt32(monthString));
                SalaryList = await _context.Зарплатаs.FromSqlRaw("exec dbo.[SP_Salary]  @y,@m", year, month).ToListAsync();
            }
            else
            {
                SalaryList = await _context.Зарплатаs.FromSqlRaw("dbo.Get_Salary").ToListAsync();
            }
            var workerList = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();


            foreach (var Зарплата in SalaryList)
            {
                foreach (var worker in workerList)
                {
                    if (Зарплата.Сотрудники == worker.Id)
                    {
                        Зарплата.СотрудникиNavigation.Фио = worker.Фио;
                    }
                }
            }
            var total = 0.0m;
            foreach (var item in SalaryList)
            {
                total += item.ОбщаяСуммаКВыдаче;
            };
            ViewBag.totalSumAll = total;
            foreach (var item in SalaryList)
            {
                item.Месяц = months[Convert.ToInt32(item.Месяц) - 1];
            }
            return View(SalaryList);
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Зарплата = await _context.Зарплатаs
                .Include(s => s.СотрудникиNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Зарплата == null)
            {
                return NotFound();
            }

            return View(Зарплата);
        }

        public async Task<IActionResult> payЗарплата(string yearString, string monthString)
        {
            var parameterReturn = new SqlParameter
            {
                ParameterName = "p",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };


            SqlParameter year = new SqlParameter("@y", Convert.ToInt32(yearString));
            SqlParameter month = new SqlParameter("@m", Convert.ToInt32(monthString));
            _context.Database.ExecuteSqlRaw("exec  @p =  dbo.[SP_Pay_Зарплата]  @y,@m", year, month, parameterReturn);
            int returnValue = (int)parameterReturn.Value;

            if (returnValue == 1)
            {
                return NotFound("Don't have enough money in budget");
            }
            else
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { yearString, monthString });
            }
        }

        
    }
}    