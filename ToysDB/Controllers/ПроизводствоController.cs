using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;

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
            if (ModelState.IsValid)
            {
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
