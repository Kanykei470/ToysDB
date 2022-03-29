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
            var toysContext = _context.Производствоs.Include(п => п.ПродукцияNavigation).Include(п => п.СотрудникNavigation);
            return View(await toysContext.ToListAsync());
        }

        // GET: Производство/Details/5
        public async Task<IActionResult> Details(string id)
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
            //ProductionOfCG_Labs_PPO3Context db = new ProductionOfCG_Labs_PPO3Context();
            //List<Ингредиенты> ингредиенты = new List<Ингредиенты>();
            //List<Сырьё> сырьё = new List<Сырьё>();

            //ингредиенты = ((from col in _context.Ингредиентыs
            //                where col.Продукция == production. 
            //                select col).ToList());

            //foreach (var item in ингредиенты)
            //{
            //    Сырьё.Add((from ing in _context.Сырьёs
            //                      where ing.Id == item.Сырье
            //                      select ing).First());
            //}

            //bool isNotEnogh = false;
            //foreach (var rawM in сырьё)
            //{
            //    foreach (var ingred in ингредиенты)
            //    {
            //        if (rawM.Id == ingred.Сырье)
            //        {
            //            if (rawM.Количество < (ingred.Количество * production.Количество))
            //            {
            //                isNotEnogh = true;
            //                break;
            //            }
            //        }
            //    }
            //}

            //decimal averageSum, needSum, totalSum = 0;
            //decimal needAmount;

            //if (isNotEnogh)
            //{
            //    ModelState.AddModelError("Amount", "Недостаточно материала для производства.");
            //}
            //else if (!isNotEnogh)
            //{
            //    foreach (var rawM in Сырьё)
            //    {
            //        foreach (var ingred in Ингредиенты)
            //        {
            //            if (rawM.Id == ingred.RawMaterial)
            //            {
            //                averageSum = Convert.ToDecimal(rawM.Sum) / Convert.ToDecimal(rawM.Amount);
            //                needAmount = Convert.ToDecimal(ingred.Amount) * Convert.ToDecimal(production.Amount);
            //                needSum = averageSum * Convert.ToDecimal(needAmount);

            //                var productAmountt = db.Сырьё
            //                    .Where(r => r.Id == ingred.RawMaterial)
            //                    .FirstOrDefault();
            //                productAmountt.Amount = Convert.ToDouble(Convert.ToDecimal(productAmountt.Amount) - Convert.ToDecimal(needAmount));
            //                productAmountt.Sum = (productAmountt.Sum - needSum);
            //                db.SaveChanges();

            //                totalSum += needSum;
            //            }

            //        }
            //    }

            //    var finProducts = db.FinishedProducts
            //           .Where(r => r.Id == production.Production1)
            //           .FirstOrDefault();
            //    finProducts.Amount = (finProducts.Amount + production.Amount);
            //    finProducts.Sum = finProducts.Sum + totalSum;
            //    db.SaveChanges();


            //    _context.Add(production);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //if (ModelState.IsValid)
            //{
            //    _context.Add(производство);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", производство.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", производство.Сотрудник);
            return View(производство);
        }

        // GET: Производство/Edit/5
        public async Task<IActionResult> Edit(string id)
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Продукция,Количество,Дата,Сотрудник")] Производство производство)
        {
            if (id != производство.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(производство);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ПроизводствоExists(производство.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", производство.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", производство.Сотрудник);
            return View(производство);
        }

        // GET: Производство/Delete/5
        public async Task<IActionResult> Delete(string id)
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
            var производство = await _context.Производствоs.FindAsync(id);
            _context.Производствоs.Remove(производство);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ПроизводствоExists(string id)
        {
            return _context.Производствоs.Any(e => e.Id == id);
        }
    }
}
