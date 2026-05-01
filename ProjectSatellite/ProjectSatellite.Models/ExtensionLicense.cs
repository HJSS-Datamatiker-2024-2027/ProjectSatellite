using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSatellite.Models
{
    public class ExtensionLicense
    {
        public Guid TenantId { get; set; }
        public string CustomerName { get; set; }
        public Guid ExtensionId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
    }
}
