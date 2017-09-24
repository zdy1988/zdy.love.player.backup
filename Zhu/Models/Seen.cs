using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Models
{
    public class Seen : IEntity
    {
        public int ID { get; set; }
        public DateTime SeenDate { get; set; }
        public string Title { get; set; }
        public string MediaSource { get; set; }
        public int MediaID { get; set; }
    }
}
