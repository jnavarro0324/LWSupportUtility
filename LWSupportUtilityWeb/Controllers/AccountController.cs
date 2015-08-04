using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LWSupportUtilityWeb.Models;

namespace LWSupportUtilityWeb.Controllers
{
    public class AccountController : Controller
    {
        
        private LWSupportUtility model = new LWSupportUtility();

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index_Post(string cUsername ,string cPassword)
        {
            
            int iUserID;

            if (string.IsNullOrEmpty(cUsername) || string.IsNullOrEmpty(cPassword))
            {
 
            }
            else
            {
                    iUserID = ValidateLogin(cUsername, cPassword);

                    if (iUserID != 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
            }
            
            return View();
        }

        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create_Get(int? id)
        {
            tblUser user = GetUser(id);

            return View(user);
        }

        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create_Post(int? id,string cFirstname,string cLastname,string cUsername, string cPassword,string cEmail)
        {
            tblUser user = GetUser(id);

            
            user.cFirstname = cFirstname;
            user.cLastname = cLastname;
            user.cUsername = cUsername;
            user.cPassword = cPassword;
            user.cEmail = cEmail;
            user.iSecretQuestionID = 0;
            user.cSecretAnswer = "Test Data";
            user.dTimestamp = DateTime.Now;

            if (!id.HasValue)
            {

                model.AddTotblUsers(user);

            }
            model.SaveChanges();
            return RedirectToAction("Create");
        }

        private tblUser GetUser(int? id)
        {
            return id.HasValue ? model.tblUsers.Where(x => x.iUserID == id).First() : new tblUser() { iUserID = -1 };
        }

        private int ValidateLogin(string cUsername, string cPassword)
        {
            var iUserID = (from Userlist in model.tblUsers where Userlist.cUsername.Equals(cUsername) && Userlist.cPassword.Equals(cPassword) select Userlist.iUserID).FirstOrDefault();
            
            return iUserID;
        }
    }
}
