using N14DCAT006.Common;
using N14DCAT006.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N14DCAT006.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        N14DCAT006DbContext data = new N14DCAT006DbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(FormCollection collection)
        {
            var taikhoan = collection["TaiKhoanAdmin"];
            var matkhau = collection["MatKhauAdmin"];
            matkhau = Encryptor.MD5Hash(matkhau);
            N14DCAT006.Models.Admin kh = data.Admins.SingleOrDefault(n => n.TaiKhoanAdmin == taikhoan && n.MatKhauAdmin == matkhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                Session["TaiKhoanAdmin"] = kh;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Thongbao1 = "Tên đăng nhập hoặc mật khẩu không đúng";
                return this.View();
            }
        }
    }
}