using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;

namespace ToysDB.Controllers
{
    public class БюджетController : Controller
    {
        private readonly ToysContext _context;

        public БюджетController(ToysContext context)
        {
            _context = context;
        }

        // GET: Бюджет
        public async Task<IActionResult> Index()
        {
            var budget = await _context.Бюджетs.FromSqlRaw("dbo.Get_Budget").ToListAsync();
            return View(budget);
        }


        // GET: Бюджет/Details/5
        public async Task<IActionResult> Details()
        {

            var budget = await _context.Бюджетs.FromSqlRaw("dbo.Get_Budget").ToListAsync();
            return View(budget);
        }

        // GET: Бюджет/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Бюджет/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Сумма,Процент,Бонус")] Бюджет бюджет)
        {
            if (ModelState.IsValid)
            {
                _context.Add(бюджет);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(бюджет);
        }

        // GET: Бюджет/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var budget = await _context.Бюджетs.FromSqlRaw("dbo.Get_Budget").ToListAsync();
            //var budget = await _context.Budgets.FindAsync(id);
            if (budget.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(budget.FirstOrDefault());
        }

        // POST: Бюджет/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Сумма,Процент,Бонус")] Бюджет бюджет)
        {
            if (id != бюджет.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", бюджет.Id);
                    SqlParameter SumOfBudget = new SqlParameter("@SumOfBudget", бюджет.Сумма);
                    SqlParameter PercentageOfPremium = new SqlParameter("@PercentageOfPremium", бюджет.Процент);
                    SqlParameter Bonus = new SqlParameter("@Bonus", бюджет.Бонус);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Budget @Id, @SumOfBudget, @PercentageOfPremium, @Bonus", Id, SumOfBudget, PercentageOfPremium, Bonus);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!БюджетExists(бюджет.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(бюджет);
        }

        // GET: Бюджет/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var бюджет = await _context.Бюджетs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (бюджет == null)
            {
                return NotFound();
            }

            return View(бюджет);
        }

        // POST: Бюджет/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var бюджет = await _context.Бюджетs.FindAsync(id);
            _context.Бюджетs.Remove(бюджет);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool БюджетExists(int id)
        {
            return _context.Бюджетs.Any(e => e.Id == id);
        }
    }
}
