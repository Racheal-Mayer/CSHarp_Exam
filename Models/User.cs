using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CExam.Models
{        public class User
        {
            [Key]
            public int UserId { get; set; }

            [Required]
            [MinLength(2, ErrorMessage="Must be atleast 2 characters!")]
            [Display(Name="First Name")]
            public string FirstName {get;set;}

            [Required]
            [MinLength(2, ErrorMessage="Must be atleast 2 characters!")]
            [Display(Name="Last Name")]
            public string LastName {get;set;}

            [EmailAddress(ErrorMessage="Must enter valid email!")]
            [Required]
            public string Email {get;set;}

            [Required]
            [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
            [DataType("Password")]
            [StrongPassword]
            public string Password {get;set;}

            [NotMapped]
            [Compare("Password")]
            [DataType(DataType.Password)]
            public string Confirm {get;set;}

            public List <Association> MyJoins {get; set;}
            //how user is connected to activity
            public List <MeetUp> PlannedActivities {get; set;}
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;

        }
    }