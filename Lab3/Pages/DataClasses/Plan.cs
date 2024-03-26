using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Lab3.Pages.DataClasses
{
    public class Plan
    {
        public int PlanID { get; set; }
        [BindProperty, Required(ErrorMessage = "Must Enter Plan's Name")] public string Name { get; set; }
        [BindProperty] public List<Step> Steps { get; set; }

        public Plan()
        {
            Steps = new List<Step>();
        }

        public Plan(int planID, string name, List<Step> steps)
        {
            PlanID = planID;
            Name = name;
            Steps = steps;
        }
    }
}
