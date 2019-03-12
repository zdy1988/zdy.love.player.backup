using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Models
{
    public class Group : IEntity
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
