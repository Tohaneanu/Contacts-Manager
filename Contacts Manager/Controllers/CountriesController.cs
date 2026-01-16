using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace Contacts_Manager.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }


        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file!";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. Please select an xlsx file!";
                return View();
            }
            int insertedCountries = await _countriesService.UploadCountriesFromExcelFile(excelFile);
            ViewBag.Message = $"{insertedCountries} Countries Uploaded";
            return View();
        }
    }
}
