using JobBoard.Data;
using JobBoard.Data.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Models.Applicant;

public class ApplicationStatusViewModel
{
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = default!;
    public int JobId { get; internal set; }
    public string JobName { get; internal set; } = default!;
}
