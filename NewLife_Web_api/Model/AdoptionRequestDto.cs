namespace NewLife_Web_api.Model
{
    public class AdoptionRequestDto
    {
        public int UserId { get; set; }
        public int AdoptionPostId { get; set; }
        public string ReasonForAdoption { get; set; }
        public bool UpdateUserInfo { get; set; } // เพิ่มฟิลด์นี้เพื่อระบุว่าต้องการอัปเดตข้อมูลผู้ใช้หรือไม่
        public UserUpdateDto UserUpdate { get; set; }
    }
}
