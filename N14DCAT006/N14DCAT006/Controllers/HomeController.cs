using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using N14DCAT006.Models;

namespace N14DCAT006.Controllers
{
    public class HomeController : Controller
    {
        N14DCAT006DbContext db = new N14DCAT006DbContext();
        
        
        public ActionResult Index()
        {
            ViewBag.file = "15.wav";   
            var all_sach = from a in db.BaiHats select a;
            ViewBag.nhacviettop = all_sach.Where(n => n.TheLoai.Contains("Nhạc Việt")).OrderByDescending(a => a.LuotXem).Take(5).ToList();
            ViewBag.nhacAuMytop = all_sach.Where(n => n.TheLoai.Contains("Nhạc Âu Mỹ")).OrderByDescending(a => a.LuotXem).Take(5).ToList();
            ViewBag.nhacChauAtop = all_sach.Where(n => n.TheLoai.Contains("Nhạc Châu Á")).OrderByDescending(a => a.LuotXem).Take(5).ToList();

            return View();
        }
        

    }
}