using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using Microsoft.AspNetCore.Http;

namespace ToysDB.Controllers
{
    public class ПродажаПродукцииController : Controller
    {
        private readonly ToysContext _context;


        public ПродажаПродукцииController(ToysContext context)
        {
            _context = context;
        }

        // GET: ПродажаПродукции
        public async Task<IActionResult> Index()
        {
            var toysContext = _context.ПродажаПродукцииs.Include(п => п.СотрудникNavigation).Include(п => п.ПродукцияNavigation);
            return View(await toysContext.ToListAsync());
        }

        // GET: ПродажаПродукции/Details/5
        public async Task<IActionResult> Details(byte? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var продажаПродукции = await _context.ПродажаПродукцииs
                .Include(п => п.ПродукцияNavigation)
                .Include(п => п.СотрудникNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (продажаПродукции == null)
            {
                return NotFound();
            }

            return View(продажаПродукции);
        }

        // GET: ПродажаПродукции/Create
        public IActionResult Create()
        {
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио");
            return View();
        }

        // POST: ПродажаПродукции/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Продукция,Количество,Сумма,Дата,Сотрудник")] ПродажаПродукции продажаПродукции)
        {
            var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
            var prod = _context.ГотоваяПродукцияs.Where(u => u.Id == продажаПродукции.Продукция).FirstOrDefault();
            var sum = prod.Сумма / Convert.ToDecimal(prod.Количество) * Convert.ToDecimal(продажаПродукции.Количество);

            var sumcheck = sum + sum / 100 * budget.Процент;
            if (продажаПродукции.Дата == null)
            {
                продажаПродукции.Дата = DateTime.Now;
            }
                if (prod.Количество < продажаПродукции.Количество || продажаПродукции.Количество == null)
                {
                    ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
                }
                else
                {
                    budget.Сумма += продажаПродукции.Сумма;
                    prod.Сумма -= sum;
                    float количество = (float)продажаПродукции.Количество;
                    prod.Количество -= (short)количество;

                    продажаПродукции.Сумма = sumcheck;
                    _context.Add(продажаПродукции);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            //}
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
            return View(продажаПродукции);

        }

        // GET: ПродажаПродукции/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var продажаПродукции = await _context.ПродажаПродукцииs.FindAsync(id);
            if (продажаПродукции == null)
            {
                return NotFound();
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
            return View(продажаПродукции);
        }

        // POST: ПродажаПродукции/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Продукция,Количество,Сумма,Дата,Сотрудник")] ПродажаПродукции продажаПродукции)
        {
            {
                if (id != продажаПродукции.Id)
                {
                    return NotFound();
                }

                var deleted = _context.ПродажаПродукцииs.Where(u => u.Id == продажаПродукции.Id).FirstOrDefault();

                var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
                var prod = _context.ГотоваяПродукцияs.Where(u => u.Id == продажаПродукции.Продукция).FirstOrDefault();
                decimal sum, sum1;

                if (prod.Количество == 0)
                {
                    sum = ((decimal)(продажаПродукции.Сумма / (100 + budget.Процент) * 100));

                    sum1 = ((decimal)(deleted.Сумма / (100 + budget.Процент) * 100));
                }
                else
                {
                    sum = ((decimal)(prod.Сумма / prod.Количество * (decimal)продажаПродукции.Количество));

                    sum1 = ((decimal)(prod.Сумма / prod.Количество * (decimal)deleted.Количество));
                }

                var sumforonedel = sum1 / (decimal)deleted.Количество;  
                var sumcheck = sum + sum / 100 * budget.Процент;

                    if (продажаПродукции.Количество > deleted.Количество + prod.Количество)
                    {
                        ModelState.AddModelError("Количество", "Недостаточно готовой продукции");
                    }
                    else
                    {
                        budget.Сумма -= deleted.Сумма;
                        budget.Сумма += продажаПродукции.Сумма;
                        prod.Количество += (short)deleted.Количество;
                        prod.Количество -= (short)продажаПродукции.Количество;
                        prod.Сумма += sum1;
                        prod.Сумма -= sum;

                        if (prod.Количество != 0 && prod.Сумма == 0)
                        {
                            prod.Сумма = prod.Количество * sumforonedel;
                        }
                        _context.Remove(deleted);
                        _context.Update(продажаПродукции);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
                ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
                ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
                return View(продажаПродукции);
            
        }
        // GET: ПродажаПродукции/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var продажаПродукции = await _context.ПродажаПродукцииs
                .Include(п => п.ПродукцияNavigation)
                .Include(п => п.СотрудникNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (продажаПродукции == null)
            {
                return NotFound();
            }

            return View(продажаПродукции);
        }

        // POST: ПродажаПродукции/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {

            var продажаПродукции = await _context.ПродажаПродукцииs.FindAsync(id);


            var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
            var prod = _context.ГотоваяПродукцияs.Where(u => u.Id == продажаПродукции.Продукция).FirstOrDefault();
            decimal sum;
            if (prod.Количество == 0)
            {
                sum = ((decimal)(продажаПродукции.Сумма / (100 + budget.Процент) * 100));
            }
            else
            {
                sum = ((decimal)(prod.Сумма / prod.Количество * (decimal)продажаПродукции.Количество));
            }
            budget.Сумма -= (short)продажаПродукции.Сумма;
            prod.Количество += (short)продажаПродукции.Количество;
            prod.Сумма += sum;

            _context.ПродажаПродукцииs.Remove(продажаПродукции);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ПродажаПродукцииExists(byte id)
        {
            return _context.ПродажаПродукцииs.Any(e => e.Id == id);
        }

    }
}
