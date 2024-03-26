using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Lab3.Pages.DataClasses
{
    public class User
    {
        [BindProperty]
        [Required(ErrorMessage = "Must Enter a Username")]
        public string? Username { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Must Enter a Password")]
        public string? Password { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Must Enter a User's First Name")]
        public string? FirstName { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Must Enter a User's Last Name")]
        public string? LastName { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? Phone { get; set; }
        [BindProperty] public string? Address { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Must Enter the User's Type")]
        public string? UserType { get; set; }
    }
}
