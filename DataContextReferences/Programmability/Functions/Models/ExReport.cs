using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.Functions.Models
{
    public class ExReport
    {
        [Key]
        [Column(Order = 0)]
        public String StateCode {get; set; }

        [Key]
        [Column(Order = 1)]
        public Int32 ActiveCount { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int32 PendingCount { get; set; }
                
        [Column(Order = 3)]
        public Int32 TerminatedCount { get; set; }
    }
}