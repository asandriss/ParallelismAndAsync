using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebParallelProgrammingTutorial.Models;

namespace WebParallelProgrammingTutorial.Controllers
{
    public class HomeController : Controller
    {
        string[] sources = {
                               "http://async.azurewebsites.net/api/membervideos/1",
                               "http://async.azurewebsites.net/api/membervideos/2",
                               "http://async.azurewebsites.net/api/membervideos/3"
                           };
        public ActionResult Sync()
        {
            var sw = Stopwatch.StartNew();
            var data = GetVideos();
            sw.Stop();
            ViewBag.Elapsed = sw.ElapsedMilliseconds;
            return View("~/views/home/index.cshtml", data);
        }

        public ActionResult SyncP()
        {
            var sw = Stopwatch.StartNew();
            var data = GetVideosParallel();
            sw.Stop();
            ViewBag.Elapsed = sw.ElapsedMilliseconds;
            return View("~/views/home/index.cshtml", data);
        }

        private IEnumerable<IEnumerable<Video>> GetVideos()
        {
            var allVideos = new List<IEnumerable<Video>>();
            foreach (var url in sources)
            {
                allVideos.Add(DownloadData(url));
            }

            return allVideos;
        }

        private IEnumerable<IEnumerable<Video>> GetVideosParallel()
        {
            var allVideos = new List<IEnumerable<Video>>();
            
            Parallel.ForEach(sources, url =>
            {
                allVideos.Add(DownloadData(url));
            });

            return allVideos;
        }

        private IEnumerable<Video> DownloadData(string url)
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = httpClient.GetAsync(url).Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            return httpResponseMessage.Content.ReadAsAsync<IEnumerable<Video>>().Result;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}