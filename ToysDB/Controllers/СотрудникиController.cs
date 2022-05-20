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
            var сотрудникиrs = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();
            var positions = await _context.Должностиs.FromSqlRaw("dbo.Get_Positions").ToListAsync();
            foreach (var e in сотрудникиrs)
            {
                foreach (var p in positions)
                {
                    if (e.Должность == p.Id)
                    {
                        e.ДолжностьNavigation.Должность = p.Должность;
                    }
                }
                
            }
            return View(сотрудникиrs);
        }

        // GET: Сотрудники/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            //Должность
            SqlParameter ID = new SqlParameter("@Id", id);
            var сотрудникиrs = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", ID).ToListAsync();
            SqlParameter pos = new SqlParameter("@Id", сотрудникиrs.FirstOrDefault().Должность);
            var posID = await _context.Должностиs.FromSqlRaw("dbo.GetID_Positions @id", pos).ToListAsync();

            if (сотрудникиrs.FirstOrDefault().Должность == posID.FirstOrDefault().Id)
            {
                сотрудникиrs.FirstOrDefault().ДолжностьNavigation.Должность = posID.FirstOrDefault().Должность;
            }

            if (ID == null)
            {
                return NotFound();
            }
            if (сотрудникиrs == null)
            {
                return NotFound();
            }
            return View(сотрудникиrs.FirstOrDefault());
        }

        // GET: Сотрудники/Create
        public IActionResult Create()
        {
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность");
            return View();
        }

        // POST: Сотрудники/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Фио,Должность,Оклад,Адрес,Телефон")] Сотрудники сотрудники)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Names = new SqlParameter("@Names", сотрудники.Фио);
                    SqlParameter Position = new SqlParameter("@Position", сотрудники.Должность);
                    SqlParameter Salary = new SqlParameter("@Salary", сотрудники.Оклад);
                    SqlParameter Address = new SqlParameter("@Address", сотрудники.Адрес);
                    SqlParameter Phone = new SqlParameter("@Phone", сотрудники.Телефон);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Employeers @Names, @Position, @Salary, @Address, @Phone", Names, Position, Salary, Address, Phone);

                    return RedirectToAction(nameof(Index));
                }
               
                ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность", сотрудники.Должность);
                return View(сотрудники);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Сотрудники/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);

            var employeers = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", ID).ToListAsync();
            if (employeers == null)
            {
                return NotFound();
            }
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность", employeers.FirstOrDefault().Должность);
            return View(employeers.FirstOrDefault());
           
        }

        // POST: Сотрудники/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Фио,Должность,Оклад,Адрес,Телефон")] Сотрудники сотрудники)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", сотрудники.Id); ; ;
                    SqlParameter Names = new SqlParameter("@Names", сотрудники.Фио);
                    SqlParameter Position = new SqlParameter("@Position", сотрудники.Должность);
                    SqlParameter Salary = new SqlParameter("@Salary", сотрудники.Оклад);
                    SqlParameter Address = new SqlParameter("@Address", сотрудники.Адрес);
                    SqlParameter Phone = new SqlParameter("@Phone", сотрудники.Телефон);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Employeers @Id, @Names, @Position, @Salary, @Address, @Phone", Id, Names, Position, Salary, Address, Phone);

                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!СотрудникиExists((byte)сотрудники.Id))
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
            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность", сотрудники.Должность);
            return View(сотрудники);
        }

        // GET: Сотрудники/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var employee = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @Id", Id).ToListAsync();

            ViewData["Должность"] = new SelectList(_context.Должностиs, "Id", "Должность", employee.FirstOrDefault().Должность);


            if (employee.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(employee.FirstOrDefault());
        }

        // POST: Сотрудники/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Employeers @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool СотрудникиExists(byte id)
        {
            return _context.Сотрудникиs.Any(e => e.Id == id) ;
        }
    }
}
