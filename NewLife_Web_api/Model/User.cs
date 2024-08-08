﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class User
    {
        [Key]
        [Column("user_id")] 
        public int userId { get; set; }

        [Column("profile_pic")]
        public string? profilePic { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("lastname")]
        public string lastName { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("role")]
        public string role { get; set; }

        [Column("address")]
        public string address { get; set; }

        [Column("tel")]
        public string tel{ get; set; }

        [Column("gender")]
        public string gender { get; set; }

        [Column("age")]
        public string age{ get; set; }

        [Column("career")]
        public string career{ get; set; }

        [Column("num_of_fam_members")]
        public string numOfFamMembers { get; set; }

        [Column("experience")]
        public string experience { get; set; }

        [Column("size_of_residence")]
        public string sizeOfResidence { get; set; }

        [Column("type_of_residence")]
        public string typeOfResidence { get; set; }

        [Column("free_time_per_day")]
        public string freeTimePerDay { get; set; }

        [Column("reason_for_adoption")]
        public string reasonForAdoption { get; set; }

        [Column("interest_id_1")]
        public string interestId1 { get; set; }

        [Column("interest_id_2")]
        public string interestId2 { get; set; }

        [Column("interest_id_3")]
        public string interestId3 { get; set; }

        [Column("interest_id_4")]
        public string interestId4 { get; set; }

        [Column("interest_id_5")]
        public string interestId5 { get; set; }
    }

}

