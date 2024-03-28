using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly LibraryDB _context;
        private readonly IWebHostEnvironment _enc;

        public StudentsController(LibraryDB context, IWebHostEnvironment enc)
        {
            _context = context;
            _enc = enc;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var data = await _context.Students.Include(i => i.Books).ThenInclude(p => p.Genre).ToListAsync();


            ViewBag.Count = data.Count;
            ViewBag.GrandTotal = data.Sum(i => i.Books.Sum(l => l.RentPrice));

            ViewBag.Average = data.Count > 0 ? data.Average(i => i.Books.Sum(l => l.RentPrice)) : 0;

            return View(data);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(i => i.Books).ThenInclude(j => j.Genre).FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View(new Student());
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,StudentName,Address,Email,ContactNo,ImagePath,ImageFile,Books")] Student student, string command = "")
        {
            if (student.ImageFile != null)
            {
                student.ImagePath = "\\Image\\" + student.ImageFile.FileName;


                string serverPath = _enc.WebRootPath + student.ImagePath;


                using FileStream stream = new FileStream(serverPath, FileMode.Create);


                await student.ImageFile.CopyToAsync(stream);

                TempData["Images"] = student.ImagePath;
            }
            else
            {
                student.ImagePath = TempData["Images"]?.ToString();
            }
            if (command == "Add")
            {
                student.Books.Add(new());
                return View(student);

            }
            else if (command.Contains("delete"))
            {
                int idx = int.Parse(command.Split('-')[1]);

                student.Books.RemoveAt(idx);
                ModelState.Clear();
                return View(student);
            }
            if (ModelState.IsValid)
            {
                var rows = await _context.Database.ExecuteSqlRawAsync("exec SpInsertStudent @p0,@p1,@p2,@p3,@p4", student.StudentName, student.Address, student.Email, student.ContactNo, student.ImagePath);
                if (rows > 0)
                {
                    student.StudentId = _context.Students.Max(x => x.StudentId);
                    foreach (var item in student.Books)
                    {
                        await _context.Database.ExecuteSqlRawAsync("exec SpInsertBook @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", item.BookTitle, item.AuthorName, item.RentPrice, item.IsAvailable, item.BookBorrowingDate, item.BookReturningDate, student.StudentId, item.GenreId);
                    }
                }



                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.Include(i => i.Books).ThenInclude(p => p.Genre).FirstOrDefaultAsync(x => x.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,StudentName,Address,Email,ContactNo,ImagePath,ImageFile,Books")] Student student, string command = "")
        {
            if (student.ImageFile != null)
            {

                student.ImagePath = "\\Image\\" + student.ImageFile.FileName;


                string serverPath = _enc.WebRootPath + student.ImagePath;


                using FileStream stream = new FileStream(serverPath, FileMode.Create);


                await student.ImageFile.CopyToAsync(stream);
                TempData["Images"] = student.ImagePath;

            }
            else
            {
                student.ImagePath = TempData["Images"]?.ToString();
            }
            if (command == "Add")
            {
                student.Books.Add(new());
                return View(student);
            }
            else if (command.Contains("delete"))
            {
                int idx = int.Parse(command.Split('-')[1]);


                student.Books.RemoveAt(idx);
                ModelState.Clear();
                return View(student);
            }
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var row = await _context.Database.ExecuteSqlRawAsync("exec SpUpdateStudent @p0, @p1, @p2, @p3, @p4,@p5", student.StudentId, student.StudentName, student.Address, student.Email, student.ContactNo, student.ImagePath);
                    foreach (var item in student.Books)
                    {
                        await _context.Database.ExecuteSqlRawAsync("exec SpInsertBook @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", item.BookTitle, item.AuthorName, item.RentPrice, item.IsAvailable, item.BookBorrowingDate, item.BookReturningDate, student.StudentId, item.GenreId);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.Include(i => i.Books).ThenInclude(p => p.Genre)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            await _context.Database.ExecuteSqlAsync($"exec SpDeleteStudent {id}");

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        [Route("~/DeleteAjax/{Id}")]
        public async Task<IActionResult> DeleteAjaxStudent(int id)
        {

            await _context.Database.ExecuteSqlAsync($"exec SpDeleteStudent {id}");



            return Ok();
        }
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}