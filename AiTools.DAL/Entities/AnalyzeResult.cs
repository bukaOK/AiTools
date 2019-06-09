using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AiTools.DAL.Entities
{
    public class AnalyzeResult : GuidEntity
    {
        [Required]
        public string Name { get; set; }
        public string DataFilePath { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
