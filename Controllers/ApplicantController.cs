using AspNetCoreHero.ToastNotification.Abstractions;
using JobBoard.Context;
using JobBoard.Data;
using JobBoard.Data.Enum;
using JobBoard.Models.Applicant;
using JobBoard.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ApplicantController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    INotyfService notyf,
    JobBoardDbContext jobBoardDbContext,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment webHostEnvironment) : Controller
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly INotyfService _notyfService = notyf;
    private readonly JobBoardDbContext _jobBoardDbContext = jobBoardDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    [HttpGet]
    public async Task<IActionResult> ListAllApplicants()
    {
        var applicants = await _jobBoardDbContext.Applicants.ToListAsync();
        return View(applicants);
    }

    public async Task<IActionResult> ApplicantDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var applicant = await _jobBoardDbContext.Applicants
            .FirstOrDefaultAsync(m => m.UserId == user!.Id);


        if (applicant != null)
        {
            var applicantDetailViewModel = new ApplicantDetailViewModel
            {
                FullName = applicant.FullName,
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber,
                Gender = applicant.Gender,
            };

            return View(applicantDetailViewModel);
        }
        else
        {
            _notyfService.Error("Applicant details not found");
            return RedirectToAction("ApplicantDetail", "Applicant");
        }
    }
    // public async Task<IActionResult> AddApplicantAsync()
    // {
    //     ViewData["Genders"] = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
    //     ViewData["States"] = await _jobBoardDbContext.States.Select(s => new { Id = s.Id, Name = s.Name }).ToListAsync();
    //     return View();
    // }

    // [HttpPost]
    // public async Task<IActionResult> AddApplicant(Applicant applicant, IFormFile cvFile, IFormFile coverLetterFile)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         _jobBoardDbContext.Add(applicant);
    //         await _jobBoardDbContext.SaveChangesAsync();
    //         return RedirectToAction(nameof(Index));
    //     }
    //     if (ModelState.IsValid)
    // {
    //     if (cvFile != null && cvFile.Length > 0)
    //     {
    //         string cvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", cvFile.FileName);
    //         using (var stream = new FileStream(cvPath, FileMode.Create))
    //         {
    //             await cvFile.CopyToAsync(stream);
    //         }
    //         applicant.CVPath = cvPath;
    //     }

    //     if (coverLetterFile != null && coverLetterFile.Length > 0)
    //     {
    //         string coverLetterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", coverLetterFile.FileName);
    //         using (var stream = new FileStream(coverLetterPath, FileMode.Create))
    //         {
    //             await coverLetterFile.CopyToAsync(stream);
    //         }
    //         applicant.CoverLetterPath = coverLetterPath;
    //     }

    //     _jobBoardDbContext.Add(applicant);
    //     await _jobBoardDbContext.SaveChangesAsync();
    //     return RedirectToAction(nameof(Index));
    // }

    // ViewData["States"] = await _jobBoardDbContext.States.Select(s => new { Id = s.Id, Name = s.Name }).ToListAsync();
    // ViewData["Genders"] = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
    // return View(applicant);
    // }

    public IActionResult AddApplicant()
    {
        var states = _jobBoardDbContext.States.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();

        var viewModel = new AddApplicantViewModel()
        {
            States = states,
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> ApplicantDetail(AddApplicantViewModel model)
    {
        var applicantExist = await _jobBoardDbContext.Applicants.AnyAsync(x => x.FullName == model.FullName || x.Email == model.Email);
        var userDetail = await Helper.GetCurrentUserIdAsync(_httpContextAccessor, _userManager);

        if (applicantExist)
        {
            _notyfService.Warning("Applicant already exist");
            return View(model);
        }

        var applicant = new Applicant
        {
            UserId = userDetail.userId,
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            StateId = model.StateId
        };

        if (model.CV != null && model.CV.Length > 0)
        {
            var cvFileName = Path.GetFileNameWithoutExtension(model.CV.FileName) + "_" + Path.GetRandomFileName() + Path.GetExtension(model.CV.FileName);
            var cvFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", cvFileName);
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "uploads"));

            using (var stream = new FileStream(cvFilePath, FileMode.Create))
            {
                await model.CV.CopyToAsync(stream);
            }

            applicant.CVPath = cvFilePath;
        }

        if (model.CoverLetter != null && model.CoverLetter.Length > 0)
        {
            var coverLetterFileName = Path.GetFileNameWithoutExtension(model.CoverLetter.FileName) + "_" + Path.GetRandomFileName() + Path.GetExtension(model.CoverLetter.FileName);
            var coverLetterFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", coverLetterFileName);
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "uploads"));

            using (var stream = new FileStream(coverLetterFilePath, FileMode.Create))
            {
                await model.CoverLetter.CopyToAsync(stream);
            }

            applicant.CoverLetterPath = coverLetterFilePath;
        }

        await _jobBoardDbContext.AddAsync(applicant);
        var result = await _jobBoardDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Details submitted successfully");
            return RedirectToAction("ListAllApplicants", "Applicant");
        }

        _notyfService.Error("An error occurred during application");
        return View(model);
    }
}