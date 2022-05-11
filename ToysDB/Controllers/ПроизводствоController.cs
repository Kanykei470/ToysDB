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
            
            ToysContext db = new ToysContext();
            List<Ингредиенты> ingredients = new List<Ингредиенты>();
            List<Сырьё> rawMaterials = new List<Сырьё>();

            ingredients = ((from col in _context.Ингредиентыs
                            where col.Продукция == производство.Продукция
                            select col).ToList());

            foreach (var item in ingredients)
            {
                rawMaterials.Add((from ing in _context.Сырьёs
                                  where ing.Id == item.Сырье
                                  select ing).First());
            }

            bool isNotEnogh = false;
            foreach (var rawM in rawMaterials)
            {
                foreach (var ingred in ingredients)
                {
                    if (rawM.Id == ingred.Сырье)
                    {
                        if (rawM.Количество < (ingred.Количество * производство.Количество))
                        {
                            isNotEnogh = true;
                            break;
                        }
                    }
                }
            }

            decimal averageSum, needSum, totalSum = 0;
            decimal needAmount;

            if (isNotEnogh)
            {
                ModelState.AddModelError("Количество", "Недостаточно материала для производства.");
            }
            else if (!isNotEnogh)
            {
                foreach (var rawM in rawMaterials)
                {
                    foreach (var ingred in ingredients)
                    {
                        if (rawM.Id == ingred.Сырье)
                        {
                            averageSum = Convert.ToDecimal(rawM.Сумма) / Convert.ToDecimal(rawM.Количество);
                            needAmount = Convert.ToDecimal(ingred.Количество) * Convert.ToDecimal(производство.Количество);
                            needSum = averageSum * Convert.ToDecimal(needAmount);

                            var productAmountt = db.Сырьёs
                                .Where(r => r.Id == ingred.Сырье)
                                .FirstOrDefault();
                            productAmountt.Количество = (short)(productAmountt.Количество - needAmount);
                            productAmountt.Сумма = productAmountt.Сумма - needSum;

                            totalSum += needSum;
                        }

                    }
                }

                var finProducts = db.ГотоваяПродукцияs
                       .Where(r => r.Id == производство.Продукция)
                       .FirstOrDefault();
                finProducts.Количество += (short)производство.Количество;
                finProducts.Сумма += totalSum;



                _context.Add(производство);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            var deleted = _context.Производствоs.Where(a => a.Id == производство.Id).FirstOrDefault();

            List<Ингредиенты> ingredients = new List<Ингредиенты>();
            List<Сырьё> rawMaterials = new List<Сырьё>();

            ingredients = ((from col in _context.Ингредиентыs
                            where col.Продукция == deleted.Продукция
                            select col).ToList());

            foreach (var item in ingredients)
            {
                rawMaterials.Add((from ing in _context.Сырьёs
                                  where ing.Id == item.Сырье
                                  select ing).First());
            }

            decimal averageSum, needSum, totalSum = 0;
            decimal needAmount;

            foreach (var rawM in rawMaterials)
            {
                foreach (var ingred in ingredients)
                {
                    if (rawM.Id == ingred.Сырье)
                    {
                        averageSum = Convert.ToDecimal(rawM.Сумма) / Convert.ToDecimal(rawM.Количество);
                        needAmount = Convert.ToDecimal(ingred.Количество) * Convert.ToDecimal(deleted.Количество);
                        needSum = averageSum * Convert.ToDecimal(needAmount);

                        var productAmountt = _context.Сырьёs
                            .Where(r => r.Id == ingred.Сырье)
                            .FirstOrDefault();
                        productAmountt.Количество += (short)needAmount;
                        productAmountt.Сумма += needSum;

                        totalSum += needSum;
                    }

                }
            }

            List<Ингредиенты> ingredients1 = new List<Ингредиенты>();
            List<Сырьё> rawMaterials1 = new List<Сырьё>();

            ingredients1 = ((from col in _context.Ингредиентыs
                             where col.Продукция == производство.Продукция
                             select col).ToList());

            foreach (var item in ingredients1)
            {
                rawMaterials1.Add((from ing in _context.Сырьёs
                                   where ing.Id == item.Сырье
                                   select ing).First());
            }

            decimal averageSum1, needSum1, totalSum1 = 0;
            decimal needAmount1;

            foreach (var rawM in rawMaterials1)
            {
                foreach (var ingred in ingredients1)
                {
                    if (rawM.Id == ingred.Сырье)
                    {
                        averageSum1 = Convert.ToDecimal(rawM.Сумма) / Convert.ToDecimal(rawM.Количество);
                        needAmount1 = Convert.ToDecimal(ingred.Количество) * Convert.ToDecimal(производство.Количество);
                        needSum1 = averageSum1 * Convert.ToDecimal(needAmount1);

                        var productAmountt1 = _context.Сырьёs
                            .Where(r => r.Id == ingred.Сырье)
                            .FirstOrDefault();
                        productAmountt1.Количество -= (short)needAmount1;
                        productAmountt1.Сумма -= needSum1;

                        totalSum1 += needSum1;
                    }

                }
            }

            var prod = _context.ГотоваяПродукцияs.Where(u => u.Id == производство.Продукция).FirstOrDefault();
            prod.Количество -= (short)deleted.Количество;
            prod.Сумма -= totalSum;
            prod.Количество += (short)производство.Количество;
            prod.Сумма += totalSum1;


            _context.Remove(deleted);
            _context.Update(производство);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));





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


            ToysContext db = new ToysContext();
            List<Ингредиенты> ingredients = new List<Ингредиенты>();
            List<Сырьё> rawMaterials = new List<Сырьё>();

            ingredients = ((from col in _context.Ингредиентыs
                            where col.Продукция == производство.Продукция
                            select col).ToList());

            foreach (var item in ingredients)
            {
                rawMaterials.Add((from ing in _context.Сырьёs
                                  where ing.Id == item.Сырье
                                  select ing).First());
            }


            decimal averageSum, needSum, totalSum = 0;
            decimal needAmount;


            foreach (var rawM in rawMaterials)
            {
                foreach (var ingred in ingredients)
                {
                    if (rawM.Id == ingred.Сырье)
                    {
                        averageSum = Convert.ToDecimal(rawM.Сумма) / Convert.ToDecimal(rawM.Количество);
                        needAmount = Convert.ToDecimal(ingred.Количество) * Convert.ToDecimal(производство.Количество);
                        needSum = averageSum * Convert.ToDecimal(needAmount);

                        var productAmountt = db.Сырьёs
                            .Where(r => r.Id == ingred.Сырье)
                            .FirstOrDefault();
                        productAmountt.Количество += (short)needAmount;
                        productAmountt.Сумма += needSum;

                        totalSum += needSum;
                    }

                }
            }

            var finProducts = db.ГотоваяПродукцияs
                   .Where(r => r.Id == производство.Продукция)
                   .FirstOrDefault();
            finProducts.Количество -= (short)производство.Количество;
            finProducts.Сумма -= totalSum;

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
