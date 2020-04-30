using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notebooks.Models;

namespace Notebooks.Controllers
{

    [Authorize(Roles ="Admin,Contributor,Viewer")]
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
            User model = new User { Id = int.Parse(User.Identity.GetUserId()) };
            return View("Index",  model);
        }
        [Authorize(Roles ="Contributor,Admin")]
        public IActionResult Upsert(int? id)
        {
            Notebook = new Notebook();
            if (id == null)
            {
                return View(Notebook);
            }
            Notebook = _db.Notebooks.FirstOrDefault(u => u.Id == id);
            if (Notebook == null)
            {
                return NotFound();
            }
            int user_id = int.Parse(User.Identity.GetUserId());
            if (Notebook.UserID != user_id && !User.IsInRole("Admin"))
            {
                return View("AccessDenied");
            }
            return View(Notebook);
        }

        [Authorize(Roles = "Contributor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                int user_id = int.Parse(User.Identity.GetUserId());
                if (Notebook.Id == 0)
                {
                    Notebook.UserID = user_id;
                    _db.Notebooks.Add(Notebook);
                }
                else
                {
                    _db.Notebooks.Update(Notebook);
                }
                _db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("UserList", new { id = Notebook.UserID });
                }
                return RedirectToAction("Index");
            }
            return View(Notebook);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (User.IsInRole("Viewer") || User.IsInRole("Admin"))
            {
                return Json(new { data = await _db.Notebooks.ToListAsync() });
            }
            else
            {
                int id = int.Parse(User.Identity.GetUserId());
                return Json(new { data = await _db.Notebooks.Where(u => u.UserID == id).ToListAsync() });
            }
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> GetByUser(string id)
        {
            return Json(new { data = await _db.Notebooks.Where(u => u.UserID == int.Parse(id)).ToListAsync() });
        }

        [Authorize(Roles="Admin")]
        public IActionResult UserList(string id)
        {
            User model = new User{ Id = int.Parse(id) } ;
            return View("UserList", model);
        }


        [Authorize(Roles = "Contributor,Admin")]
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
    }

}