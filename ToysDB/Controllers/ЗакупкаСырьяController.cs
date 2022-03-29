using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;

namespace ToysDB.Controllers
{
    public class ЗакупкаСырьяController : Controller
    {
        private readonly ToysContext _context;
        private readonly INotyfService _notyf;

        public ЗакупкаСырьяController(ToysContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: ЗакупкаСырья
        public async Task<IActionResult> Index()
        {
            var toysContext = _context.ЗакупкаСырьяs.Include(з => з.СотрудникNavigation).Include(з => з.СырьёNavigation);
            return View(await toysContext.ToListAsync());
        }

        // GET: ЗакупкаСырья/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var закупкаСырья = await _context.ЗакупкаСырьяs
                .Include(з => з.СотрудникNavigation)
                .Include(з => з.СырьёNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (закупкаСырья == null)
            {
                return NotFound();
            }

            return View(закупкаСырья);
        }

        // GET: ЗакупкаСырья/Create
        public IActionResult Create()
        {
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио");
            ViewData["Сырьё"] = new SelectList(_context.Сырьёs, "Id", "Наименование");
            return View();
        }

        // POST: ЗакупкаСырья/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Сырьё,Количество,Сумма,Дата,Сотрудник")] ЗакупкаСырья закупкаСырья)
        {
            var buget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
            var raw = _context.Сырьёs.Where(u => u.Id == закупкаСырья.Сырьё).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (buget.Сумма < закупкаСырья.Сумма)
                {
                    ModelState.AddModelError("Сумма", "У вас недостаточно бюджета!");
                    
                }
                else 
                {
                    buget.Сумма = buget.Сумма - закупкаСырья.Сумма;
                    raw.Количество = (short?)(raw.Количество + закупкаСырья.Количество);
                    raw.Сумма = raw.Сумма + закупкаСырья.Сумма;
                    _context.Add(закупкаСырья);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
               
            }
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", закупкаСырья.Сотрудник);
            ViewData["Сырьё"] = new SelectList(_context.Сырьёs, "Id", "Наименование", закупкаСырья.Сырьё);
            return View(закупкаСырья);
            _notyf.Success("Запись успешно создана.");
        }

        // GET: ЗакупкаСырья/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var закупкаСырья = await _context.ЗакупкаСырьяs.FindAsync(id);
            if (закупкаСырья == null)
            {
                return NotFound();
            }
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", закупкаСырья.Сотрудник);
            ViewData["Сырьё"] = new SelectList(_context.Сырьёs, "Id", "Наименование", закупкаСырья.Сырьё);
            return View(закупкаСырья);

        }

        // POST: ЗакупкаСырья/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Сырьё,Количество,Сумма,Дата,Сотрудник")] ЗакупкаСырья закупкаСырья)
        {
            if (id != закупкаСырья.Id)
            {
                return NotFound();
            }

            var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();
            var raw = _context.Сырьёs.Where(u => u.Id == закупкаСырья.Сырьё).FirstOrDefault();
            var deleted = _context.ЗакупкаСырьяs.Where(u => u.Id == закупкаСырья.Id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (budget.Сумма + deleted.Сумма < закупкаСырья.Сумма)
                {

                    ModelState.AddModelError("Сумма", "У вас недостаточно бюджета!");
                }
                else
                {
                    budget.Сумма += deleted.Сумма;
                    budget.Сумма -= закупкаСырья.Сумма;
                    raw.Количество -= deleted.Количество;
                    raw.Количество += закупкаСырья.Количество;
                    raw.Сумма -= deleted.Сумма;
                    raw.Сумма += закупкаСырья.Сумма;

                    try
                    {
                        _context.Remove(deleted);
                        _context.Update(закупкаСырья);
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ЗакупкаСырьяExists(закупкаСырья.Id))
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
            }
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", закупкаСырья.Сотрудник);
            ViewData["Сырьё"] = new SelectList(_context.Сырьёs, "Id", "Наименование", закупкаСырья.Сырьё);
            return View(закупкаСырья);
        }


        // GET: ЗакупкаСырья/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var закупкаСырья = await _context.ЗакупкаСырьяs
                .Include(з => з.СотрудникNavigation)
                .Include(з => з.СырьёNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

           

            if (закупкаСырья == null)
            {
                return NotFound();
            }

            return View(закупкаСырья);
        }

        // POST: ЗакупкаСырья/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var budget = _context.Бюджетs.Where(u => u.Id == 1).FirstOrDefault();

            var закупкаСырья = await _context.ЗакупкаСырьяs.FindAsync(id);

            var raw = _context.Сырьёs.Where(u => u.Id == закупкаСырья.Сырьё).FirstOrDefault();
            budget.Сумма += закупкаСырья.Сумма;
            raw.Количество -= закупкаСырья.Количество;
            raw.Сумма -= закупкаСырья.Сумма;

            _context.ЗакупкаСырьяs.Remove(закупкаСырья);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ЗакупкаСырьяExists(byte id)
        {
            return _context.ЗакупкаСырьяs.Any(e => e.Id == id);
        }
    }
}
