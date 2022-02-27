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
    public class СотрудникиController : Controller
    {
        private readonly ToysContext _context;

        public СотрудникиController(ToysContext context)
        {
            _context = context;
        }

        // GET: Сотрудники
        public async Task<IActionResult> Index()
        {
            var toysContext = _context.Сотрудникиs.Include(с => с.ДолжностьNavigation);
            return View(await toysContext.ToListAsync());
        }

        // GET: Сотрудники/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сотрудники = await _context.Сотрудникиs
                .Include(с => с.ДолжностьNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (сотрудники == null)
            {
                return NotFound();
            }

            return View(сотрудники);
        }

        // GET: Сотрудники/Create
        public IActionResult Create()
        {
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Id");
            return View();
        }

        // POST: Сотрудники/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Фио,Должность,Оклад,Адрес,Телефон")] Сотрудники сотрудники)
        {
            if (ModelState.IsValid)
            {
                _context.Add(сотрудники);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность", сотрудники.Должность);
            return View(сотрудники);
        }

        // GET: Сотрудники/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сотрудники = await _context.Сотрудникиs.FindAsync(id);
            if (сотрудники == null)
            {
                return NotFound();
            }
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Id", сотрудники.Должность);
            return View(сотрудники);
        }

        // POST: Сотрудники/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Фио,Должность,Оклад,Адрес,Телефон")] Сотрудники сотрудники)
        {
            if (id != сотрудники.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(сотрудники);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!СотрудникиExists(сотрудники.Id))
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
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Id", сотрудники.Должность);
            return View(сотрудники);
        }

        // GET: Сотрудники/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сотрудники = await _context.Сотрудникиs
                .Include(с => с.ДолжностьNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (сотрудники == null)
            {
                return NotFound();
            }

            return View(сотрудники);
        }

        // POST: Сотрудники/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var сотрудники = await _context.Сотрудникиs.FindAsync(id);
            _context.Сотрудникиs.Remove(сотрудники);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool СотрудникиExists(byte id)
        {
            return _context.Сотрудникиs.Any(e => e.Id == id) ;
        }
    }
}
