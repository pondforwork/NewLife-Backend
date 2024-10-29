namespace NewLife_Web_api.Model
{
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Tel { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Career { get; set; }
        public int? NumOfFamMembers { get; set; }
        public bool? IsHaveExperience { get; set; }
        public string? SizeOfResidence { get; set; }
        public string? TypeOfResidence { get; set; }
        public int? FreeTimePerDay { get; set; }
        public int? MonthlyIncome { get; set; }
    }
}
