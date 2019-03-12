using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Models
{
    public class Actor : IEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FormerName { get; set; }
        public string Summary { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DebutDay { get; set; }
        public string Introduce { get; set; }
    }
}
