using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AiTools.DAL.Entities
{
    public class ForecastResult : GuidEntity
    {
        [Required]
        public string Name { get; set; }
        public string ModelPath { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
