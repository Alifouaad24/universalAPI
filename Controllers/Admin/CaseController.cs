//using CloudinaryDotNet;
//using CloudinaryDotNet.Actions;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Universal_server.Data;
//using Universal_server.Models;
//using static System.Net.Mime.MediaTypeNames;

//namespace Universal_server.Controllers.Admin
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CaseController : ControllerBase
//    {
//        private readonly UniversalDbContext dbContext;
//        private readonly IConfiguration configuration;
//        private Cloudinary cloudinary;
//        string cloudName;
//        string apiKey;
//        string apiSecret;


//        public CaseController(UniversalDbContext dbContext, IConfiguration configuration)
//        {
//            this.dbContext = dbContext;
//            this.configuration = configuration;

//            cloudName = configuration["Claoudinary:cloudName"];
//            apiKey = configuration["Claoudinary:apiKey"];
//            apiSecret = configuration["Claoudinary:apiSecret"];

//            cloudinary = new Cloudinary(new Account(
//                cloudName,
//                apiKey,
//                apiSecret
//            ));
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllCases()
//        {
//            var cases = await dbContext.Cases
//               .Include(c => c.Customer)
//               .Include(c => c.CarInfoTbl).ThenInclude(c => c.CarBrand)
//               .Include(c => c.CarInfoTbl).ThenInclude(c => c.CarModel)
//               .Include(c => c.caseImages)
//               .Include(c => c.Case_Services)
//                   .ThenInclude(cs => cs.Service)
//               .Include(c => c.Case_Services)
//                   .ThenInclude(cs => cs.Case_Service_Notes)

//               .ToListAsync();

//            return Ok(cases);
//        }


//        [HttpPost]
//        public async Task<IActionResult> AddCase(CaseRequests model)
//        {
//            int brandId = 0;
//            int ModelId = 0;
//            int YearId = 0;

//            if (!ModelState.IsValid) return BadRequest(ModelState);


//            var carBrandRec = await dbContext.CarBrands.Where(c => c.CarBrandName.ToLower().Contains(model.Brand.ToLower())).FirstOrDefaultAsync();
//            if (carBrandRec == null)
//            {
//                var newCarBrandRec = new CarBrand
//                {
//                    CarBrandName = model.Brand.ToLower().Trim(),
//                };

//                await dbContext.CarBrands.AddAsync(newCarBrandRec);
//                await dbContext.SaveChangesAsync();
//                brandId = newCarBrandRec.CarBrandId;
//            }
//            else
//            {
//                brandId = carBrandRec.CarBrandId;
//            }
//            var carYearRec = await dbContext.CarYears.Where(cy => cy.CarYearNumber.Trim() == model.Year.Trim()).FirstOrDefaultAsync();
//            if (carYearRec == null)
//            {
//                var newCarYearRec = new CarYear
//                {
//                    CarYearNumber = model.Year.Trim(),
//                };

//                await dbContext.CarYears.AddAsync(newCarYearRec);
//                await dbContext.SaveChangesAsync();

//                YearId = newCarYearRec.CarYearId;
//            }
//            else
//            {
//                YearId = carYearRec.CarYearId;
//            }

//            var carModelRec = await dbContext.CarModels.Where(cy => cy.CarModelName.Trim() == model.Model.Trim()).FirstOrDefaultAsync();
//            if (carModelRec == null)
//            {
//                var newCarModelRec = new CarModel
//                {
//                    CarModelName = model.Model.Trim(),
//                };

//                await dbContext.CarModels.AddAsync(newCarModelRec);
//                await dbContext.SaveChangesAsync();
//                ModelId = newCarModelRec.CarModelId;
//            }
//            else
//            {
//                ModelId = carModelRec.CarModelId;
//            }


           


//            var carInfo = new CarInfoTbl
//            {
//                VinNumber = model.VinNumber,
//                CarBrandId = brandId,
//                CarModelId = ModelId,
//                CarYearId = YearId,
//                GlobalCustomerId = model.CustomerId 
//            };



//            await dbContext.CarInfoTbls.AddAsync(carInfo);
//            await dbContext.SaveChangesAsync();


//            var Case = new Case
//            {
//                CustomerId = model.CustomerId,
//                CarInfoTblId = carInfo.CarInfoTblId,
//                Date = model.visitDate,
//            };

//            await dbContext.Cases.AddAsync(Case);
//            await dbContext.SaveChangesAsync();

//            return Ok(Case);
//        }

//        [HttpPut("BindImagesWithCase/{id}")]
//        public async Task<IActionResult> BindImagesWithCase(int id, [FromForm] ImagesCaseRequests model)
//        {
//            var caseImgs = new List<caseImage>();

//            var Case = await dbContext.Cases
//                .Include(c => c.CarInfoTbl)
//                .Include(c => c.caseImages)
//                .Where(c => c.CaseId == id).FirstOrDefaultAsync();

//            if (model.Images != null && model.Images.Count > 0)
//            {
//                foreach (var image in model.Images)
//                {
//                    if (image.Length <= 0)
//                        continue;

//                    using var stream = image.OpenReadStream();

//                    var uploadParams = new ImageUploadParams
//                    {
//                        File = new FileDescription(image.FileName, stream),
//                        Folder = "Cases"
//                    };
//                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

//                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
//                    {
//                        caseImgs.Add(new caseImage
//                        {
//                            CaseId = Case.CaseId,
//                            ImageUrl = uploadResult.SecureUrl.ToString(),
//                        });
//                    }
//                }
//                await dbContext.caseImages.AddRangeAsync(caseImgs);
//                await dbContext.SaveChangesAsync();
//            }

//            return Ok(Case);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCase(int id)
//        {
//            var Case = await dbContext.Cases.FindAsync(id);
//            if (Case == null) return NotFound();
//            dbContext.Cases.Remove(Case);
//            await dbContext.SaveChangesAsync();
//            return NoContent();
//        }



//        [HttpDelete("DeleteImageFromCase")]
//        public async Task<IActionResult> DeleteImageFromCase([FromQuery] string imageUrl)
//        {
//            var caseImage = await dbContext.caseImages
//                .FirstOrDefaultAsync(ci => ci.ImageUrl == imageUrl);

//            if (caseImage == null)
//                return NotFound("Image not found in database");

//            var publicId = ExtractPublicId(imageUrl);

//            if (string.IsNullOrEmpty(publicId))
//                return BadRequest("Invalid image url");

//            var deletionParams = new DeletionParams(publicId);
//            var result = await cloudinary.DestroyAsync(deletionParams);

//            if (result.Result != "ok")
//                return BadRequest("Failed to delete image from Cloudinary");

//            dbContext.caseImages.Remove(caseImage);
//            await dbContext.SaveChangesAsync();

//            return Ok(new { message = "Image deleted successfully" });
//        }


//        //[HttpPut("{id}")]
//        //public async Task<IActionResult> UpdateCase(int id, [FromForm] CaseRequests model)
//        //{

//        //    var Case = await dbContext.Cases
//        //        .Include(c => c.CarInfoTbl)
//        //        .Include(c => c.caseImages)
//        //        .Where(c => c.CaseId == id).FirstOrDefaultAsync();

//        //    if (!ModelState.IsValid) return BadRequest(ModelState);

//        //    Case.CustomerId = model.CustomerId;
//        //    Case.CarInfoTbl.VinNumber = model.VinNumber;
//        //    Case.CarInfoTbl.Brand = model.Brand;
//        //    Case.CarInfoTbl.Model = model.Model;
//        //    Case.CarInfoTbl.Year = model.Year;
//        //    Case.Date = model.visitDate;

//        //    await dbContext.SaveChangesAsync();


//        //    return Ok(Case);
//        //}

//        private string ExtractPublicId(string imageUrl)
//        {
//            var uri = new Uri(imageUrl);
//            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
//            var uploadIndex = Array.IndexOf(segments, "upload");
//            var publicIdSegments = segments.Skip(uploadIndex + 2);
//            var publicId = string.Join("/", publicIdSegments);
//            return Path.ChangeExtension(publicId, null);
//        }

//        [HttpPost("AddCaseServiceToCase")]
//        public async Task<IActionResult> AddCaseServiceToCase(int caseId, CaseServicRequest request)
//        {
//            var Case = await dbContext.Cases.FindAsync(caseId);
//            if (Case == null) return NotFound("Case not found");

//            var caseService = new Case_Service
//            {
//                CaseID = caseId,
//                Service_id = request.ServiceId ?? 0,
//                Resolved = request.Resolved,
//                Notes = request.Notes,
//                Cost = request.Cost,
//                Discount = request.Discount,
//                paid = request.paid
//            };

//            await dbContext.Case_Services.AddAsync(caseService);
//            await dbContext.SaveChangesAsync();
//            return Ok(caseService);
//        }

//        [HttpPut("EditCaseService")]
//        public async Task<IActionResult> EditCaseService(int caseServicId, CaseServicRequest request)
//        {
//            var CaseService = await dbContext.Case_Services.FindAsync(caseServicId);
//            if (CaseService == null) return NotFound("Case not found");


//            CaseService.Service_id = request.ServiceId ?? CaseService.Service_id;
//            CaseService.Resolved = request.Resolved ?? CaseService.Resolved;
//            CaseService.Notes = request.Notes ?? CaseService.Notes;
//            CaseService.Cost = request.Cost ?? CaseService.Cost;
//            CaseService.Discount = request.Discount ?? CaseService.Discount;
//            CaseService.paid = request.paid ?? CaseService.paid;

//            await dbContext.SaveChangesAsync();
//            return Ok(CaseService);
//        }

//        [HttpPut("changeStatusCaseService")]
//        public async Task<IActionResult> changeStatusCaseService(int caseServicId, CaseServiceStatusRequest request)
//        {
//            var CaseService = await dbContext.Case_Services.FindAsync(caseServicId);
//            if (CaseService == null) return NotFound("Case not found");


//            CaseService.Resolved = request.resulved;

//            await dbContext.SaveChangesAsync();
//            return Ok(CaseService);
//        }

//        [HttpDelete("DeletetCaseService")]
//        public async Task<IActionResult> DeletetCaseService(int caseServicId)
//        {
//            var CaseService = await dbContext.Case_Services.FindAsync(caseServicId);
//            if (CaseService == null) return NotFound("Case not found");

//            var notes = await dbContext.Case_Service_Notes.Where(n => n.Case_ServiceId == caseServicId).ToListAsync();

//            if (notes.Count > 0)
//            {
//                dbContext.Case_Service_Notes.RemoveRange(notes);
//            }

//            dbContext.Case_Services.Remove(CaseService);

//            await dbContext.SaveChangesAsync();
//            return Ok(new
//            {
//                msg = "Case service deleted successfully"
//            });
//        }

//        [HttpPost("AddServiceCaseNote")]
//        public async Task<IActionResult> AddServiceCaseNote(int caseServicId, CaseServiceNotesRequest request)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);
//            var CaseService = await dbContext.Case_Services.FindAsync(caseServicId);
//            if (CaseService == null) return NotFound("Case service not found");

//            foreach (var note in request.notes)
//            {
//                var caseNote = new Case_Service_Notes
//                {
//                    Case_ServiceId = caseServicId,
//                    Notes = note
//                };
//                await dbContext.Case_Service_Notes.AddAsync(caseNote);
//            }

//            await dbContext.SaveChangesAsync();
//            return Ok(new
//            {
//                msg = "Notes added successfully"
//            });
//        }

//        [HttpDelete("DeleteCaseServiceNote")]
//        public async Task<IActionResult> DeleteCaseServiceNote(int caseServicNoteId)
//        {

//            var CaseServiceNote = await dbContext.Case_Service_Notes.FindAsync(caseServicNoteId);
//            if (CaseServiceNote == null) return NotFound("Case service note not found");

//            dbContext.Case_Service_Notes.Remove(CaseServiceNote);

//            await dbContext.SaveChangesAsync();
//            return Ok(new
//            {
//                msg = "Notes added successfully"
//            });
//        }



  //  }
    public class CaseRequests
{
    public int CustomerId { get; set; }
    public string? VinNumber { get; set; }
    public string? Year { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public DateOnly? visitDate { get; set; }
}

public class CaseServicRequest
{
    public int? CaseId { get; set; }
    public int? ServiceId { get; set; }
    public bool? Resolved { get; set; }
    public string? Notes { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Discount { get; set; }
    public decimal? paid { get; set; }
}

public class ImagesCaseRequests
{
    public List<IFormFile>? Images { get; set; }
}

public class CaseServiceNotesRequest
{
    public List<string> notes { get; set; }
}

public class CaseServiceStatusRequest
{
    public bool resulved { get; set; }
}



