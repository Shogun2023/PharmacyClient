using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Models
{
    public class SalesDTO
    {
        public int SAL_ID { get; set; }
        public string SAL_DR_HEADING { get; set; }
        public int SAL_AMOUNT { get; set; }
        public System.DateTime SAL_DATE { get; set; }
        public double SAL_COST { get; set; }
        public SalesDTO() { }
        public SalesDTO(int sal_id, string sal_dr_heading, int sal_amount, DateTime sal_date, double sal_cost)
        {
            this.SAL_ID = sal_id;
            this.SAL_DR_HEADING = sal_dr_heading;
            this.SAL_AMOUNT = sal_amount;
            this.SAL_DATE = sal_date;
            this.SAL_COST = sal_cost;
        }
    }
}
