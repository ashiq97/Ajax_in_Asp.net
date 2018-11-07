using jQueryAjaxInAsp.netMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxInAsp.netMvc.Controllers
{
    public class EmployeeController : Controller
    {

        
        // GET: Employee
        public ActionResult Index()
        {

            return View();
        }

        // in viewAll we want to return all of the employee ( a employee list)
        public ActionResult ViewAll()
        {
            return View(GetAllEmployee());
        }

        IEnumerable<Employee> GetAllEmployee()
        {
            using (DBModel db = new DBModel())
            {
                return db.Employees.ToList<Employee>();
            }

            //DBModel db = new DBModel();

            //List<Employee> empList = db.Employees.ToList();

            //return empList;
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Employee emp = new Employee();
            if(id!=0)
            {
                using (DBModel db = new DBModel())
                {
                   // emp = db.Employees.SingleOrDefault( x => x.EmployeeID == id);
                    emp = db.Employees.Where( x => x.EmployeeID == id).FirstOrDefault<Employee>();
                 //   return emp;
                }
            }
            return View(emp);
        }

        [HttpPost]

        public ActionResult AddOrEdit(Employee emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);

                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    emp.ImagePath = "~/Appfiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Appfiles/Images/"), fileName));
                }
                using (DBModel db = new DBModel())
                {
                    if(emp.EmployeeID == 0)
                    {
                        db.Employees.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                //return RedirectToAction("ViewAll");

                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this,"ViewAll",GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);

              //  throw;
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                        Employee emp;
                    emp = db.Employees.SingleOrDefault(x => x.EmployeeID == id);
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}