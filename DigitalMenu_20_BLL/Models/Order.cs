using DigitalMenu_20_BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public string Note { get; set; }
        public int TotalAmount { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
