using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        //private readonly ApplicationDbContext _db;

        /*        public VillaController(ApplicationDbContext db)
                {
                    _db = db;
                }*/

        //private readonly IVillaRepository _villaRepo;
        private readonly IUnitOfWork _unitOfWork;

        /*    public VillaController(IVillaRepository villaRepo)
            {
                _villaRepo = villaRepo;
            }*/

        private readonly IWebHostEnvironment _webHostEnvironment;

        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult Index()
        {
            //var villas = _db.Villas.ToList();
            //var villas = _villaRepo.GetAll();

            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("name", "The description cannot exactly match the Name.");
            }

            if (ModelState.IsValid) {
                //_db.Villas.Add(obj);
                //_db.SaveChanges();

                //_villaRepo.Add(obj);
                //_villaRepo.Save();

                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");
                    //using (var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create)) { obj.Image.CopyTo(filestream); }
                    using var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(filestream);

                    obj.ImageUrl = @"\images\VillaImage\" + fileName;


                } else
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }

                _unitOfWork.Villa.Add(obj);
                //_unitOfWork.Villa.Save();
                _unitOfWork.Save();

                TempData["success"] = "The villa has been created successfully.";
                // mentioning controller name is not necessary here, since we are already inside villa controller..
                //return RedirectToAction(nameof(Index), "Villa");
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to create the villa.";
            return View();
        }


        public IActionResult Update(int villaId)
        {
            //Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);
            //Villa? obj = _villaRepo.Get(u => u.Id == villaId);
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if (ModelState.IsValid && obj.Id > 0)
            {
                //_db.Villas.Update(obj);
                //_db.SaveChanges();

                //_villaRepo.Update(obj);
                //_villaRepo.Save();


                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");
                    //using (var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create)) { obj.Image.CopyTo(filestream); }

                    if(!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.Trim('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var filestream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(filestream);

                    obj.ImageUrl = @"\images\VillaImage\" + fileName;


                }
             


                _unitOfWork.Villa.Update(obj);
                //_unitOfWork.Villa.Save();
                _unitOfWork.Save();

                TempData["success"] = "The villa has been updated successfully.";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "Failed to update the villa.";
            return View();
        }



        public IActionResult Delete(int villaId)
        {
            //Villa? obj = _db.Villas.FirstOrDefault(u => u.Id == villaId);

            //Villa? obj = _villaRepo.Get(u => u.Id == villaId);

            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }



        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            //Villa? objFromDB = _db.Villas.FirstOrDefault(u => u.Id == obj.Id);
            //Villa? objFromDB = _villaRepo.Get(u => u.Id == obj.Id);

            Villa? objFromDB = _unitOfWork.Villa.Get(u => u.Id == obj.Id);
            if (objFromDB is not null)
            {
                //_db.Villas.Remove(objFromDB);
                //_db.SaveChanges();

                //_villaRepo.Remove(objFromDB);
                //_villaRepo.Save();

                if (!string.IsNullOrEmpty(objFromDB.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDB.ImageUrl.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Villa.Remove(objFromDB);
                //_unitOfWork.Villa.Save();
                _unitOfWork.Save();

                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to delete the villa.";
            return View();
        }



    }
}
