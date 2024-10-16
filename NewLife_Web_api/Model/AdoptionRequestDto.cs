namespace NewLife_Web_api.Model
{
    public class AdoptionRequestDto
    {
        public int UserId { get; set; }
        public int AdoptionPostId { get; set; }
        public string ReasonForAdoption { get; set; }
    }
}
