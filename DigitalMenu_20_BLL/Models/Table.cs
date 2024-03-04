using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Models
{
    public class Table
    {
        public int Id { get; set; }
        public long QRCode { get; set; }
        public List<Order> Orders { get; set; }
    }
}
