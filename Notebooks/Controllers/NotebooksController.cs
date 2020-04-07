using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notebooks.Models;

namespace Notebooks.Controllers
{

    [Authorize(Roles ="Admin,User")]
    public class NotebooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        public NotebooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public Notebook Notebook { get; set; }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles ="User")]
        public IActionResult Upsert(int? id)
        {
            Notebook = new Notebook();
            Notebook.UserID = GetUserId();
            if (id == null)
            {
                return View(Notebook);
            }
            Notebook = _db.Notebooks.FirstOrDefault(u => u.Id == id);
            if (Notebook == null)
            {
                return NotFound();
            }
            return View(Notebook);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                Notebook.UserID = GetUserId();
                if (Notebook.Id == 0)
                {
                    _db.Notebooks.Add(Notebook);
                }
                else
                {
                    _db.Notebooks.Update(Notebook);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Notebook);
        }

        public int GetUserId()
        {
            string em = User.Identity.Name;
            User user = _db.Users.FirstOrDefault(u => u.Email == em);
            return user.Id;
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (User.IsInRole("Admin"))
            {
                return Json(new { data = await _db.Notebooks.ToListAsync() });
            }
            else
            {
                int id = GetUserId();
                return Json(new { data = await _db.Notebooks.Where(u => u.UserID == id).ToListAsync() });
            }
        }

        [Authorize(Roles = "User")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var notebook = await _db.Notebooks.FirstOrDefaultAsync(u => u.Id == id);
            if (notebook == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _db.Notebooks.Remove(notebook);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful." });
        }
        #endregion
    }

}