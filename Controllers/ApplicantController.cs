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

    public async Task<IActionResult> ApplicantDetail(int id)
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
    public IActionResult AddApplicant()
    {
        var states = _jobBoardDbContext.States.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();
        var jobs = _jobBoardDbContext.Jobs.Select(x => new SelectListItem
        {
            Text = x.JobName,
            Value = x.Id.ToString()
        }).ToList();

        var viewModel = new AddApplicantViewModel()
        {
            States = states,
            Jobs = jobs
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> AddApplicant(AddApplicantViewModel model)
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
            StateId = model.StateId,
            JobId = model.JobId
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
    private async Task<Applicant?> GetCurrentApplicant()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return null;
        }

        return await _jobBoardDbContext.Applicants
            .FirstOrDefaultAsync(a => a.UserId == user.Id);
    }

    public async Task<IActionResult> EditApplicantDetailAsync()
    {
        var applicant = await GetCurrentApplicant();
        if (applicant == null)
        {
            return NotFound();
        }

        var states = _jobBoardDbContext.States.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        }).ToList();

        var applicantDetailViewModel = new ApplicantDetailViewModel
        {
            FullName = applicant.FullName,
            Email = applicant.Email,
            PhoneNumber = applicant.PhoneNumber,
            Gender = applicant.Gender,
            States = states,
            StateId = applicant.StateId,
            JobId = applicant.JobId
        };

        return View(applicantDetailViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditApplicantDetail(ApplicantDetailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var applicant = await GetCurrentApplicant();
            if (applicant == null)
            {
                return NotFound();
            }

            applicant.FullName = model.FullName;
            applicant.Email = model.Email;
            applicant.PhoneNumber = model.PhoneNumber;
            applicant.Gender = model.Gender;
            applicant.StateId = model.StateId;

            _jobBoardDbContext.Update(applicant);
            await _jobBoardDbContext.SaveChangesAsync();

            _notyfService.Success("Applicant details updated successfully");
            return RedirectToAction("ViewApplicantDetail", "Applicant");
        }

        _notyfService.Error("An error occurred while updating details");
        return View(model);
    }
    public async Task<IActionResult> DeleteApplicantDetail(int id)
    {
        var applicant = await _jobBoardDbContext.Applicants.FindAsync(id);
        if (applicant == null)
        {
            return NotFound();
        }

        return View(applicant);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var applicant = await _jobBoardDbContext.Applicants.FindAsync(id);
        if (applicant == null)
        {
            return NotFound();
        }

        _jobBoardDbContext.Applicants.Remove(applicant);
        await _jobBoardDbContext.SaveChangesAsync();

        return RedirectToAction("ListAllApplicants", "Applicant");
    }
    public async Task<IActionResult> ApplicationStatus()
    {
        var user = await _userManager.GetUserAsync(User);

        var applicant = await _jobBoardDbContext.Applicants
            .Include(a => a.Job)
            .FirstOrDefaultAsync(m => m.UserId == user!.Id);

        if (applicant == null)
        {
            return NotFound();
        }

        if (applicant.Job == null)
        {
            return NotFound("No applied job found."); 
        }

        var appliedJob = new
        {
            ApplicantName = applicant.FullName,
            Job = applicant.Job.JobName
        };

        return View(appliedJob);
    }
}