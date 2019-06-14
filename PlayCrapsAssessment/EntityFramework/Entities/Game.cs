using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCrapsAssessment
{
    class Game
    {        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }  
        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public List<Round> Rounds { get; set; }
        public Player Player { get; set; }
    }
}
