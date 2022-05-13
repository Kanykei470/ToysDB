using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Data.SqlClient;

namespace ToysDB.Controllers
{
    public class ПродажаПродукцииController : Controller
    {
        private readonly ToysContext _context;
        private readonly INotyfService _notyf;

        public ПродажаПродукцииController(ToysContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: ПродажаПродукции
        public async Task<IActionResult> Index()
        {
            var employeers = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();
            var finishedproduction = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.Get_Finished_Production").ToListAsync();
            var saleOfProducts = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.Get_Sale_Of_Products").ToListAsync();

            foreach (var sp in saleOfProducts)
            {
                foreach (var emp in employeers)
                {
                    if (sp.Сотрудник == emp.Id)
                    {
                        sp.СотрудникNavigation.Фио = emp.Фио;
                    }
                }
                foreach (var fp in finishedproduction)
                {
                    if (sp.Продукция == fp.Id)
                    {
                        sp.ПродукцияNavigation.Наименование = fp.Наименование;
                    }
                }
            }

            return View(saleOfProducts);
        }

        // GET: ПродажаПродукции/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            //продукция и Сотрудник
            SqlParameter ID = new SqlParameter("@Id", id);
            var saleOfProducts = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.GetID_Sale_Of_Products @id", ID).ToListAsync();
            SqlParameter emp = new SqlParameter("@Id", saleOfProducts.FirstOrDefault().Сотрудник);
            var empID = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", emp).ToListAsync();
            SqlParameter production = new SqlParameter("@Id", saleOfProducts.FirstOrDefault().Продукция);
            var pID = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", production).ToListAsync();

            if ((saleOfProducts.FirstOrDefault().Сотрудник == empID.FirstOrDefault().Id) &&
                (saleOfProducts.FirstOrDefault().Продукция == pID.FirstOrDefault().Id))
            {
                saleOfProducts.FirstOrDefault().СотрудникNavigation.Фио = empID.FirstOrDefault().Фио;
                saleOfProducts.FirstOrDefault().ПродукцияNavigation.Наименование = pID.FirstOrDefault().Наименование;
            }
            if (ID == null)
            {
                return NotFound();
            }
            if (saleOfProducts == null)
            {
                return NotFound();
            }
            return View(saleOfProducts.FirstOrDefault());

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
            ToysContext db = new ToysContext();
            //List<Бюджет> budget = new List<Бюджет>();
            var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
            var prod = _context.ГотоваяПродукцияs.Where(u => u.Id == продажаПродукции.Продукция).FirstOrDefault();
            var sum = (prod.Сумма / prod.Количество * (decimal)продажаПродукции.Количество);
            var sumcheck = sum + sum / 100* budget.Процент;
            if (продажаПродукции.Дата == null)
            {
                продажаПродукции.Дата = DateTime.Now;
            }
                if (prod.Количество < продажаПродукции.Количество||продажаПродукции.Количество == null)
                {
                    ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
                }               
                else
                {
                продажаПродукции.Сумма = sumcheck-(budget.Процент*sum/100);
                budget.Сумма += sumcheck;
                prod.Сумма -= sum;
                prod.Количество -= (short)продажаПродукции.Количество;

                    _context.Add(продажаПродукции);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            
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
            var sumcheck = sum + sum / 100 * budget.Процент ;

            if (продажаПродукции.Количество > deleted.Количество + prod.Количество)
            {
                ModelState.AddModelError("Количество", "Недостаточно готовой продукции");
            }
            else
            {
                продажаПродукции.Сумма = sumcheck - (budget.Процент * sum / 100);
              
                budget.Сумма = budget.Сумма - (deleted.Сумма + ((deleted.Сумма/100)*budget.Процент)) +
                    (продажаПродукции.Сумма+((продажаПродукции.Сумма / 100) * budget.Процент));


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
            budget.Сумма -= (short)продажаПродукции.Сумма+((продажаПродукции.Сумма/100)*budget.Процент);
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
