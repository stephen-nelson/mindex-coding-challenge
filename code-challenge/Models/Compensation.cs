using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class Compensation
    {
        [Key]
        public string Employee { get; set; }
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
