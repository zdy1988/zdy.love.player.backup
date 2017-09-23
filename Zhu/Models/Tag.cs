using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Models
{
    public class Tag : IEntity
    {
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string Type { get; set; }
        public int TypeSourceID { get; set; }
    }
}
