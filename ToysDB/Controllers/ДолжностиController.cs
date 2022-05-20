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
    public class ДолжностиController : Controller
    {
        private readonly ToysContext _context;

        public ДолжностиController(ToysContext context)
        {
            _context = context;
        }

        // GET: Должности
        public async Task<IActionResult> Index()
        {
            var positions = await _context.Должностиs.FromSqlRaw("dbo.Get_Positions").ToListAsync();
            return View(positions);
        }

        // GET: Должности/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);
            var positions = await _context.Должностиs.FromSqlRaw("dbo.GetID_Positions @id", ID).ToListAsync();
            if (id == null)
            {
                return NotFound();
            }

            return View(positions.FirstOrDefault());
        }

        // GET: Должности/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Должности/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Должность")] Должности должности)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Position = new SqlParameter("@Должность", должности.Должность);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Positions @Должность", Position);
                    return RedirectToAction(nameof(Index));
                }
                return View(должности);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Должности/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var position = await _context.Должностиs.FromSqlRaw("dbo.GetID_Positions @Id", Id).ToListAsync();

            //var post = await _context.Posts.FindAsync(id);
            if (position.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(position.FirstOrDefault());
        }

        // POST: Должности/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Должность")] Должности должности)
        {
            if (id != должности.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", должности.Id);
                    SqlParameter Position = new SqlParameter("@Должность", должности.Должность);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Position @Id, @Должность", Id, Position);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ДолжностиExists(должности.Id))
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
            return View(должности);
        }

        // GET: Должности/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlParameter Id = new SqlParameter("@Id", id);
            var post = await _context.Должностиs.FromSqlRaw("dbo.Delete_Positions @Id", Id).ToListAsync();

            if (post.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(post.FirstOrDefault());
        }

        // POST: Должности/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Positions @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool ДолжностиExists(byte id)
        {
            return _context.Должностиs.Any(e => e.Id == id);
        }
    }
}
