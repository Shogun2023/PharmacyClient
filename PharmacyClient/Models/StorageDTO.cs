using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Models
{
    public class StorageDTO
    {
        public int ST_ID { get; set; }
        public string ST_DR_HEADING { get; set; }
        public System.DateTime ST_DATE { get; set; }
        public int ST_AMOUNT { get; set; }
        public StorageDTO() { }
        public StorageDTO(int st_id, string st_dr_heading, DateTime st_date, int st_amount)
        {
            this.ST_ID = st_id;
            this.ST_DR_HEADING = st_dr_heading;
            this.ST_DATE = st_date;
            this.ST_AMOUNT = st_amount;
        }
    }
}
