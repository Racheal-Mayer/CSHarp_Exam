using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CExam.Models
{        public class MeetUp
        {
            [Key]
            public int MeetUpId {get; set;}
            [Required]
            public string Title {get; set;}
            [Required]
            public string Description {get; set;}
            [FutureDate]
            public DateTime Date {get; set;}
            public string Duration {get; set;}
            public int DurationTime {get; set;}
            public List <Association> Participants {get; set;}
            public int UserId {get; set;}
            //foreign key
            public User Coordinator {get; set;}
            //one to many nav property
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
        }
}

