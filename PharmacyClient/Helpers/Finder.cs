using PharmacyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.Helpers
{
    internal class Finder
    {
        readonly int id;
        public Finder(int id)
        {
            this.id = id;
        }
        public bool DrugPredicate(Drugs drug)
        {
            return drug.DR_ID == id;
        }
        public bool SupplierPredicate(Suppliers supplier)
        {
            return supplier.SUP_ID == id;
        }
        public bool UserRolePredicate(UserRoles userRole)
        {
            return userRole.UR_ID == id;
        }
    }
}
