using System;
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
    public class ПроизводствоController : Controller
    {
        private readonly ToysContext _context;

        public ПроизводствоController(ToysContext context)
        {
            _context = context;
        }

        // GET: Производство
        public async Task<IActionResult> Index()
        {
            var employeers = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();
            var finishedproduction = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.Get_Finished_Production").ToListAsync();
            var Production = await _context.Производствоs.FromSqlRaw("dbo.Get_Production").ToListAsync();

            foreach (var prod in Production)
            {
                foreach (var emp in employeers)
                {
                    if (prod.Сотрудник == emp.Id)
                    {
                        prod.СотрудникNavigation.Фио = emp.Фио;
                    }
                }
                foreach (var fp in finishedproduction)
                {
                    if (prod.Продукция == fp.Id)
                    {
                        prod.ПродукцияNavigation.Наименование = fp.Наименование;
                    }
                }
            }

            return View(Production);
        }

        // GET: Производство/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //Продукция и Сотрудник
            SqlParameter ID = new SqlParameter("@Id", id);
            var productionO = await _context.Производствоs.FromSqlRaw("dbo.GetID_Production @id", ID).ToListAsync();
            SqlParameter emp = new SqlParameter("@Id", productionO.FirstOrDefault().Сотрудник);
            var empID = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", emp).ToListAsync();
            SqlParameter production = new SqlParameter("@Id", productionO.FirstOrDefault().Продукция);
            var pID = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", production).ToListAsync();

            if ((productionO.FirstOrDefault().Сотрудник == empID.FirstOrDefault().Id) &&
                (productionO.FirstOrDefault().Продукция == pID.FirstOrDefault().Id))
            {
                productionO.FirstOrDefault().СотрудникNavigation.Фио = empID.FirstOrDefault().Фио;
                productionO.FirstOrDefault().ПродукцияNavigation.Наименование = pID.FirstOrDefault().Наименование;
            }
            if (ID == null)
            {
                return NotFound();
            }
            if (productionO == null)
            {
                return NotFound();
            }
            return View(productionO.FirstOrDefault());
        }

        // GET: Производство/Create
        public IActionResult Create()
        {
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио");
            return View();
        }

        // POST: Производство/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Продукция,Количество,Дата,Сотрудник")] Производство производство)
        {
            if (ModelState.IsValid)
            {
                SqlParameter Product = new SqlParameter("@product", производство.Продукция);
                SqlParameter Amount = new SqlParameter("@amount", производство.Количество);
                SqlParameter Date = new SqlParameter("@date", производство.Дата);
                SqlParameter Emp = new SqlParameter("@emp", производство.Сотрудник);
                var outParam = new SqlParameter
                {
                    ParameterName = "@r",
                    DbType = System.Data.DbType.Int32,
                    Size = 100,
                    Direction = System.Data.ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Production @product, @amount, @date, @emp, @r OUT", Product, Amount, Date, Emp, outParam);

                if (outParam.SqlValue.ToString() == "0")
                {
                    ModelState.AddModelError("Количество", "Недостаточно Сырья!");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", производство.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", производство.Сотрудник);
            return View(производство);
        }

        // GET: Производство/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var производство = await _context.Производствоs.FindAsync(id);
            if (производство == null)
            {
                return NotFound();
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", производство.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", производство.Сотрудник);
            return View(производство);
        }

        // POST: Производство/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Продукция,Количество,Дата,Сотрудник")] Производство производство)
        {
            SqlParameter Id = new SqlParameter("@id", производство.Id);
            SqlParameter Product = new SqlParameter("@product", производство.Продукция);
            SqlParameter Amount = new SqlParameter("@amount", производство.Количество);
            SqlParameter Date = new SqlParameter("@date", производство.Дата);
            SqlParameter Emp = new SqlParameter("@emp", производство.Сотрудник);
            var outParam = new SqlParameter
            {
                ParameterName = "@r",
                DbType = System.Data.DbType.Int32,
                Size = 100,
                Direction = System.Data.ParameterDirection.Output
            };
            var currentProduction = await _context.Производствоs.FromSqlRaw("dbo.GetID_Production @id", Id).ToListAsync();
            SqlParameter crntAmount = new SqlParameter("@crntamount", currentProduction.FirstOrDefault().Количество);

            await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Production @id, @product, @amount, @date, @emp, @crntamount, @r OUT", Id, Product, Amount, Date, Emp, crntAmount, outParam);

            if (outParam.SqlValue.ToString() == "0")
            {
                ModelState.AddModelError("Количество", "Недостаточно Сырья!");
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", производство.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", производство.Сотрудник);
            return View(производство);
        }

        // GET: Производство/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var производство = await _context.Производствоs
                .Include(п => п.ПродукцияNavigation)
                .Include(п => п.СотрудникNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (производство == null)
            {
                return NotFound();
            }

            return View(производство);
        }

        // POST: Производство/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ПроизводствоExists(int id)
        {
            return _context.Производствоs.Any(e => e.Id == id);
        }
    }
}
