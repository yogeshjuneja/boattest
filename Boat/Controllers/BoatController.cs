using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoatModels;
using BoatService;
using Microsoft.AspNetCore.Mvc;

namespace Boat.Controllers
{
    public class BoatController : Controller
    {
        IBoatService IBoatService;
        public BoatController(IBoatService objIBoatService)
        {
            IBoatService = objIBoatService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterBoat()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RegisterBoatSubmit(BoatRegisterModel objBoatRegisterModel)
        {
            string HTML = "";
            try
            {
                DynamicData objDynamicData = IBoatService.RegisterBoat(objBoatRegisterModel);
                int output = objDynamicData.info.Status;
                if (output > 0)
                    HTML = "Boat Added Boat Number is" + output.ToString();
                else
                    HTML = "Boat Already exist with name " + objBoatRegisterModel.BoatName;
            }
            catch (Exception ex)
            {
                HTML = ex.ToString();
            }
            TempData["SubmitOutput"] = HTML;
            return RedirectToAction("RegisterBoat");
        }

        public IActionResult RentBoat()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RentBoatSubmit(BoatRentedModel objBoatRentedModel)
        {
            string HTML = "";
            try
            {
                DynamicData objDynamicData = IBoatService.RentBoat(objBoatRentedModel);
                int output = objDynamicData.info.Status;
                switch(output)
                {
                    case 100:
                            HTML = "Boat Already Assigned to Someone else";
                            break;
                    case 404:
                        HTML = "Boat Number not exists. Number" + objBoatRentedModel.BoatNumber;
                        break;
                    default:
                           HTML = "Boat Assigned to " + objBoatRentedModel.CustomerName;
                        break;
                } 
            }
            catch (Exception ex)
            {
                HTML = ex.ToString();
            }
            TempData["SubmitOutput"] = HTML;
            return RedirectToAction("RentBoat");
        }
    }
}