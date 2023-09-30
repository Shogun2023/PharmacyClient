using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Models
{
    public class DrugsDTO
    {
        public int DR_ID { get; set; }
        public string DR_HEADING { get; set; }
        public DrugsDTO() { }
        public DrugsDTO(int dr_id, string dr_heading)
        {
            DR_ID = dr_id;
            DR_HEADING = dr_heading;
        }
    }
}
