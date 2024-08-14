using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairManagementSystem.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public int VehicleID { get; set; }
        public int NumberOnHand { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
    }
}
