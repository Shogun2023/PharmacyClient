//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PharmacyClient.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Users
    {
        public byte U_ID { get; set; }
        public string U_NAME { get; set; }
        public string U_SNAME { get; set; }
        public string U_LOGIN { get; set; }
        public string U_PASS { get; set; }
        public byte U_ROLE { get; set; }
    
        public virtual UserRoles UserRoles { get; set; }
    }
}
