using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Models
{
    public class ReceiptsDTO
    {
        public int R_ID { get; set; }
        public string R_DR_HEADING { get; set; }
        public string R_SUP_NAME { get; set; }
        public int R_AMOUNT { get; set; }
        public System.DateTime R_DATE { get; set; }
        public double R_PRICE { get; set; }
        public ReceiptsDTO() { }
        public ReceiptsDTO(int r_id, string r_dr_heading, string r_sup_name, int r_amount, DateTime r_date)
        {
            this.R_ID = r_id;
            this.R_DR_HEADING = r_dr_heading;
            this.R_SUP_NAME = r_sup_name;
            this.R_AMOUNT = r_amount;
            this.R_DATE = r_date;
        }
    }
}
