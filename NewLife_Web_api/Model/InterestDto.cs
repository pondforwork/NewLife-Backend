using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewLife_Web_api.Model
{
    public class InterestDto
    {
        public int UserId { get; set; }
        public int? InterestId1 { get; set; }
        public int? InterestId2 { get; set; }
        public int? InterestId3 { get; set; }
        public int? InterestId4 { get; set; }
        public int? InterestId5 { get; set; }
    }
}
