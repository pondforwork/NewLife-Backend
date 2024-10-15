using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewLife_Web_api.Model
{
    public class InterestDto
    {
        public int UserId { get; set; }
        public List<int> BreedIds { get; set; }
    }
}
