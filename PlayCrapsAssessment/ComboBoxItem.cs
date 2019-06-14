using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCrapsAssessment
{
    public class ComboBoxItem
    {      
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
