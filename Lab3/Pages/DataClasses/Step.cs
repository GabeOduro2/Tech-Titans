namespace Lab3.Pages.DataClasses
{
    public class Step
    {
        public int StepID { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Information { get; set; }
        public Plan Plan { get; set; } // FK
        public int PlanID { get; set; }

    }
}
