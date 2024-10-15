using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewLife_Web_api.Model
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Tel { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public IFormFile? ProfilePic { get; set; }
        public string? Address { get; set; }
        public string? Career { get; set; }
        public int? NumOfFamMembers { get; set; }
        public string? Experience { get; set; }
        public string? SizeOfResidence { get; set; }
        public string? TypeOfResidence { get; set; }
        public int? FreeTimePerDay { get; set; }
        public string? ReasonForAdoption { get; set; }
        public int? InterestId1 { get; set; }
        public int? InterestId2 { get; set; }
        public int? InterestId3 { get; set; }
        public int? InterestId4 { get; set; }
        public int? InterestId5 { get; set; }

        public List<int> InterestedBreedIds { get; set; }
    }
}
