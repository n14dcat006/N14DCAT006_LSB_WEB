using N14DCAT006.Common;
using N14DCAT006.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N14DCAT006.Controllers
{
    public class UserController : Controller
    {
        N14DCAT006DbContext data = new N14DCAT006DbContext(); 
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, User user)
        {
            var mk = collection["MatKhauUser"];
            var nhaplaimk = collection["NhapLaiMatKhau"];
            if (mk.CompareTo(nhaplaimk) == 0)
            {
                mk = Encryptor.MD5Hash(mk);
                user.TaiKhoanUser = collection["TaiKhoanUser"];
                user.HoTenUser = collection["HoTenUser"];
                user.MatKhauUser = mk;
                user.Email = collection["Email"];
                user.SoDienThoai = collection["SoDienThoai"];
                user.Status = true;
                data.Users.Add(user);
                data.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.Thongbao = "mật khẩu và Nhập lai mật khẩu phải giống nhau";
                return this.DangKy();
            }
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var taikhoan = collection["TaiKhoanUser"];
            var matkhau = collection["MatKhauUser"];
            matkhau = Encryptor.MD5Hash(matkhau);
            User kh = data.Users.SingleOrDefault(n => n.TaiKhoanUser == taikhoan && n.MatKhauUser == matkhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                Session["TaiKhoan"] = kh;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Thongbao1 = "Tên đăng nhập hoặc mật khẩu không đúng";
                return this.View();
            }
        }
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
        
    }
}