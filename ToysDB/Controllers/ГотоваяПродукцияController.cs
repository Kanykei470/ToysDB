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
    public class ГотоваяПродукцияController : Controller
    {
        private readonly ToysContext _context;

        public ГотоваяПродукцияController(ToysContext context)
        {
            _context = context;
        }

        // GET: ГотоваяПродукция
        public async Task<IActionResult> Index()
        {
            var units = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.Get_Units").ToListAsync();
            var finishedProduction = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.Get_Finished_Production").ToListAsync();
            foreach (var fp in finishedProduction)
            {
                foreach (var u in units)
                {
                    if (fp.ЕдиницаИзмерения == u.Id)
                    {
                        fp.ЕдиницаИзмеренияNavigation.Наименование = u.Наименование;
                    }
                }
               
            }

            return View(finishedProduction);
        }

        // GET: ГотоваяПродукция/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var готоваяПродукция = await _context.ГотоваяПродукцияs
                .Include(г => г.ЕдиницаИзмеренияNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (готоваяПродукция == null)
            {
                return NotFound();
            }

            return View(готоваяПродукция);
        }

        // GET: ГотоваяПродукция/Create
        public IActionResult Create()
        {
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View();
        }

        // POST: ГотоваяПродукция/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] ГотоваяПродукция готоваяПродукция)
        {
            if (ModelState.IsValid)
            {
                _context.Add(готоваяПродукция);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", готоваяПродукция.ЕдиницаИзмерения);
            return View(готоваяПродукция);
        }

        // GET: ГотоваяПродукция/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var готоваяПродукция = await _context.ГотоваяПродукцияs.FindAsync(id);
            if (готоваяПродукция == null)
            {
                return NotFound();
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", готоваяПродукция.ЕдиницаИзмерения);
            return View(готоваяПродукция);
        }

        // POST: ГотоваяПродукция/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] ГотоваяПродукция готоваяПродукция)
        {
            if (id != готоваяПродукция.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(готоваяПродукция);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ГотоваяПродукцияExists(готоваяПродукция.Id))
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
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", готоваяПродукция.ЕдиницаИзмерения);
            return View(готоваяПродукция);
        }

        // GET: ГотоваяПродукция/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var готоваяПродукция = await _context.ГотоваяПродукцияs
                .Include(г => г.ЕдиницаИзмеренияNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (готоваяПродукция == null)
            {
                return NotFound();
            }

            return View(готоваяПродукция);
        }

        // POST: ГотоваяПродукция/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var готоваяПродукция = await _context.ГотоваяПродукцияs.FindAsync(id);
            _context.ГотоваяПродукцияs.Remove(готоваяПродукция);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ГотоваяПродукцияExists(byte id)
        {
            return _context.ГотоваяПродукцияs.Any(e => e.Id == id);
        }
    }
}
