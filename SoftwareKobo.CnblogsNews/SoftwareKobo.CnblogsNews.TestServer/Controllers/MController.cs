using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftwareKobo.CnblogsNews.TestServer.Controllers
{
    public class MController : Controller
    {
        public class News
        {
            public int Id
            {
                get;
                set;
            }

            public string Title
            {
                get;
                set;
            }

            public DateTime PublishTime
            {
                get;
                set;
            }

            public int CommentCount
            {
                get;
                set;
            }
        }

        // GET: M
        public ActionResult Index(int page = 1)
        {
            if (page < 1 || page > 100)
            {
                return View("Error");
            }
            var rand = new Random();
            var newsList = new List<News>();
            int topId = rand.Next(20, int.MaxValue);
            for (int i = 0; i < 15; i++)
            {
                newsList.Add(new News()
                {
                    Id = topId,
                    PublishTime = DateTime.Now,
                    Title = topId + ": " + DateTime.Now,
                    CommentCount = rand.Next(10)
                });
                topId--;
            }
            ViewData.Model = newsList;
            return View();
        }

        public ActionResult N(int id)
        {
            return View();
        }
    }
}