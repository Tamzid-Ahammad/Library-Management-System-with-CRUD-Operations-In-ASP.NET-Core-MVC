using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.ViewComponents
{
    public class BookList:ViewComponent
    {
        public IViewComponentResult Invoke(List<Book> data)
        {

            ViewBag.Count = data.Count;
            ViewBag.Total = data.Sum(i => i.RentPrice);

            return View(data);
        }
    }
}
