// using AspNetCoreHero.ToastNotification.Abstractions;
// using JobBoard.Context;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// public class JobApplicationController(
//     UserManager<IdentityUser> userManager,
//     SignInManager<IdentityUser> signInManager,
//     INotyfService notyf,
//     JobBoardDbContext jobBoardDbContext,
//     IHttpContextAccessor httpContextAccessor,
//     IWebHostEnvironment webHostEnvironment) : Controller
// {
//     private readonly UserManager<IdentityUser> _userManager = userManager;
//     private readonly SignInManager<IdentityUser> _signInManager = signInManager;
//     private readonly INotyfService _notyfService = notyf;
//     private readonly JobBoardDbContext _jobBoardDbContext = jobBoardDbContext;
//     private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
//     private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
//     public async Task<ActionResult> ViewApplicationStatusAsync(int jobId)
//     {
//         var user = await _userManager.GetUserAsync(User);
//         var applicantId = user!.Id;
//         var applicant = await _jobBoardDbContext.Applicants
//             .FirstOrDefaultAsync(m => m.UserId == user!.Id);

//         var application = new JobApplication
//         {
//             ApplicantId = applicantId,
//             JobId = jobId,
//             ApplicationStatus = ApplicationStatus.Submitted.ToString()
//         };
//         _jobBoardDbContext.JobApplications.Add(application);
//         _jobBoardDbContext.SaveChanges();

//         var applicantName = _jobBoardDbContext.Applicants.Find(applicantId)?.FullName;
//         var jobName = _jobBoardDbContext.Jobs.Find(jobId)?.JobName;

//         var applicationDetails = new ApplicationSubmissionViewModel
//         {
//             ApplicantName = applicantName!,
//             JobName = jobName!,
//             ApplicationStatus = application.ApplicationStatus.ToString()
//         };

//         return View("ApplicationSubmitted", applicationDetails);
//     }
// }

