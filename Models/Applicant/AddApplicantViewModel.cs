using JobBoard.Data.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Models.Applicant;

public class AddApplicantViewModel
{
    public int ApplicantId { get; set; }
    [Display(Name = "Full Name")]
    [Required(ErrorMessage = "This field is required")]
    public string FullName { get; set; } = default!;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "This field is required")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = default!;

    [Display(Name = "Phone Number")]
    [Required(ErrorMessage = "This field is required")]
    public string PhoneNumber { get; set; } = default!;

    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime DOB { get; set; } = default!;

    [Display(Name = "Gender")]
    [Required(ErrorMessage = "Please select a gender")]
    public Gender Gender { get; set; } = default!;

    [Display(Name = "States")]
    [Required(ErrorMessage = "Please select a state")]
    public int StateId { get; set; }
    public List<SelectListItem> States { get; set; } = default!;
    public IFormFile CV { get; set; } = default!;
    public IFormFile CoverLetter { get; set; } = default!;
    public string? CVPath { get; set; }
    public string? CoverLetterPath { get; set; }
}