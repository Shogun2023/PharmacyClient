using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Models
{
    public class UsersDTO
    {
        public byte U_ID { get; set; }
        public string U_NAME { get; set; }
        public string U_SNAME { get; set; }
        public string U_LOGIN { get; set; }
        public string U_PASS { get; set; }
        public string U_ROLE_NAME { get; set; }
        public UsersDTO() { }
        public UsersDTO(byte u_id, string u_name, string u_sname, string u_login, string u_pass, string u_role_name)
        {
            this.U_ID = u_id;
            this.U_NAME = u_name;
            this.U_SNAME = u_sname;
            this.U_LOGIN = u_login;
            this.U_PASS = u_pass;
            this.U_ROLE_NAME = u_role_name;
        }
    }
}
