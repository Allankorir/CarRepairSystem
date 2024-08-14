using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairManagementSystem.Models
{
    public class Repair
    {
        public int ID { get; set; }
        public int InventoryID { get; set; }
        public string RepairDetails { get; set; }
    }
}
