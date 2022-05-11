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
    public class СырьёController : Controller
    {
        private readonly ToysContext _context;

        public СырьёController(ToysContext context)
        {
            _context = context;
        }

        // GET: Сырьё
        public async Task<IActionResult> Index()
        {
            var raw_Materials = await _context.Должностиs.FromSqlRaw("dbo.Get_Raw_Materials").ToListAsync();
            return View(raw_Materials);
        }

        // GET: Сырьё/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сырьё = await _context.Сырьёs
                .Include(с => с.ЕдиницаИзмеренияNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (сырьё == null)
            {
                return NotFound();
            }

            return View(сырьё);
        }

        // GET: Сырьё/Create
        public IActionResult Create()
        {
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View();
        }

        // POST: Сырьё/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] Сырьё сырьё)
        {
            if (ModelState.IsValid)
            {
                _context.Add(сырьё);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", сырьё.ЕдиницаИзмерения);
            return View(сырьё);
        }

        // GET: Сырьё/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сырьё = await _context.Сырьёs.FindAsync(id);
            if (сырьё == null)
            {
                return NotFound();
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", сырьё.ЕдиницаИзмерения);
            return View(сырьё);
        }

        // POST: Сырьё/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] Сырьё сырьё)
        {
            if (id != сырьё.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(сырьё);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!СырьёExists(сырьё.Id))
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
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Id", сырьё.ЕдиницаИзмерения);
            return View(сырьё);
        }

        // GET: Сырьё/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var сырьё = await _context.Сырьёs
                .Include(с => с.ЕдиницаИзмеренияNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (сырьё == null)
            {
                return NotFound();
            }

            return View(сырьё);
        }

        // POST: Сырьё/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var сырьё = await _context.Сырьёs.FindAsync(id);
            _context.Сырьёs.Remove(сырьё);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool СырьёExists(byte id)
        {
            return _context.Сырьёs.Any(e => e.Id == id);
        }
    }
}
