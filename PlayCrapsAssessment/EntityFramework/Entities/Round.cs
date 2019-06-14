using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCrapsAssessment
{
    class Round
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoundId { get; set; }
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public RoundOutcome Outcome { get; set; }
        public List<Rolls> Rolls { get; set; }
        public Game Game { get; set; }
    }
}
