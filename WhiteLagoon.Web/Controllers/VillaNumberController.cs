using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        //private readonly ApplicationDbContext _db;

        private readonly IUnitOfWork _unitOfWork;

        /*     public VillaNumberController(ApplicationDbContext db)               
             {
                 _db = db;
             }*/

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //var villaNumbers = _db.VillaNumbers.ToList();
            //var villaNumbers = _unitOfWork.VillaNumbers.Include(u => u.Villa).ToList();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {

            //IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});


            //ViewData["VillaList"] = list;
            // OR 
            //ViewBag.VillaList = list;
            // return View();

            VillaNumberVM villaNumber = new VillaNumberVM()
            {
                //VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                //{
                //    Text = u.Name,
                //    Value = u.Id.ToString()
                //})

                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            }; 

            return View(villaNumber);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {

            // now we have used ValidateNever in model itself,
            // so we don't need ModelState.Remove("Villa") to remove it from validation
            // thus commenting the same as below
            //ModelState.Remove("Villa");

            //bool isVillaNumberExists = _db.VillaNumbers.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            bool isVillaNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            if (ModelState.IsValid && !isVillaNumberExists) {
                //_db.VillaNumbers.Add(obj.VillaNumber);
                //_db.SaveChanges();

                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();


                TempData["success"] = "The villa number has been created successfully.";
                // mentioning controller name is not necessary here, since we are already inside villa controller..
                //return RedirectToAction(nameof(Index), "Villa");
                return RedirectToAction(nameof(Index));
            }
            if (isVillaNumberExists)
            {
                TempData["error"] = "Villa Number Already Exists!!";
            } else
            {
                TempData["error"] = "Failed to create the villa number.";
            }


            //obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
             obj.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
             {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }


        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                //VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                //VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };

            if(villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM obj)
        {
           
            if (ModelState.IsValid)
            {
                //_db.VillaNumbers.Update(obj.VillaNumber);
                //_db.SaveChanges();

                _unitOfWork.VillaNumber.Update(obj.VillaNumber);
                _unitOfWork.Save();

                TempData["success"] = "The villa number has been updated successfully.";
                // mentioning controller name is not necessary here, since we are already inside villa controller..
                //return RedirectToAction(nameof(Index), "Villa");
                return RedirectToAction(nameof(Index));
            } else
            {
                TempData["error"] = "Failed to create the villa number.";
            }

            //obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            obj.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }



        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                //VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                //VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };

            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(villaNumberVM);
        }



        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            //VillaNumber? objFromDB = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            VillaNumber? objFromDB = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
            if (objFromDB is not null)
            {
                //_db.VillaNumbers.Remove(objFromDB);
                //_db.SaveChanges();

                _unitOfWork.VillaNumber.Remove(objFromDB);
                _unitOfWork.Save();


                TempData["success"] = "The villa number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to delete the villa number.";
            return View();
        }



    }
}
