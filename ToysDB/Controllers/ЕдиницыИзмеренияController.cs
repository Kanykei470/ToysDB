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
    public class ЕдиницыИзмеренияController : Controller
    {
        private readonly ToysContext _context;

        public ЕдиницыИзмеренияController(ToysContext context)
        {
            _context = context;
        }

        // GET: ЕдиницыИзмерения
        public async Task<IActionResult> Index()
        {
            var units = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.Get_Units").ToListAsync();
            return View(units);
        }

        // GET: ЕдиницыИзмерения/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);
            var Units = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.GetID_Units @id", ID).ToListAsync();

            if (ID == null)
            {
                return NotFound();
            }

            if (Units == null)
            {
                return NotFound();
            }
            return View(Units.FirstOrDefault());
        }

        // GET: ЕдиницыИзмерения/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ЕдиницыИзмерения/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Наименование")] ЕдиницыИзмерения единицыИзмерения)
        {
            if (ModelState.IsValid)
            {
                _context.Add(единицыИзмерения);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(единицыИзмерения);
        }

        // GET: ЕдиницыИзмерения/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var единицыИзмерения = await _context.ЕдиницыИзмеренияs.FindAsync(id);
            if (единицыИзмерения == null)
            {
                return NotFound();
            }
            return View(единицыИзмерения);
        }

        // POST: ЕдиницыИзмерения/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование")] ЕдиницыИзмерения единицыИзмерения)
        {
            if (id != единицыИзмерения.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(единицыИзмерения);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ЕдиницыИзмеренияExists(единицыИзмерения.Id))
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
            return View(единицыИзмерения);
        }

        // GET: ЕдиницыИзмерения/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var единицыИзмерения = await _context.ЕдиницыИзмеренияs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (единицыИзмерения == null)
            {
                return NotFound();
            }

            return View(единицыИзмерения);
        }

        // POST: ЕдиницыИзмерения/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var единицыИзмерения = await _context.ЕдиницыИзмеренияs.FindAsync(id);
            _context.ЕдиницыИзмеренияs.Remove(единицыИзмерения);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ЕдиницыИзмеренияExists(byte id)
        {
            return _context.ЕдиницыИзмеренияs.Any(e => e.Id == id);
        }
    }
}
