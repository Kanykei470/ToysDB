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
            SqlParameter Id = new SqlParameter("@Id", id);
            var unit = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.GetID_Units @Id", Id).ToListAsync();

            //var unit = await _context.Units.FindAsync(id);
            if (unit.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(unit.FirstOrDefault());
        }

        // POST: ЕдиницыИзмерения/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование")] ЕдиницыИзмерения unit)
        {
            if (id != unit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", unit.Id);
                    SqlParameter Title = new SqlParameter("@Title", unit.Наименование);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Units @Id, @Title", Id, Title);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ЕдиницыИзмеренияExists(unit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unit);
        }

        // GET: ЕдиницыИзмерения/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var unit = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.GetID_Units @Id", Id).ToListAsync();
            if (unit.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(unit.FirstOrDefault());
        }

        // POST: ЕдиницыИзмерения/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Units @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool ЕдиницыИзмеренияExists(byte id)
        {
            return _context.ЕдиницыИзмеренияs.Any(e => e.Id == id);
        }
    }
}
