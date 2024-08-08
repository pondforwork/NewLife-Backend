using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class DonationChannel
    {

        [Key]
        [Column("donation_channel_id")]
        public int donationChannelId { get; set; }

        [Column("image_url")]
        public string? imageUrl { get; set; }

        [Column("bank_name")]
        public string bankName { get; set; }

        [Column("account_name")]
        public string accountName { get; set; }

        [Column("account_number")]
        public int accountNumber { get; set; }

        [Column("date_added")]
        public DateTime dateAdded { get; set; }














    }
}
