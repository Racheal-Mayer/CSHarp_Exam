using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CExam.Models
{        public class Association
        {
            [Key]
            public int AssociationId {get; set;}
            public int UserId {get; set;}
            public int MeetUpId {get; set;}
            public User Tagalong {get; set;}
            public MeetUp Joined {get; set;}
        }
    }
