using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCrapsAssessment
{
    class Rolls
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RollId { get; set; }
        [ForeignKey("Round")]
        public int RoundId { get; set; } 
        [Required]
        public int RollValue { get; set; }
        public Round Round { get; set; }
    }
}
