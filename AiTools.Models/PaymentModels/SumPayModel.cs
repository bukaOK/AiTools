using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AiTools.Models.PaymentModels
{
    public class SumPayModel
    {
        [Required]
        public double Sum { get; set; }
    }
}
