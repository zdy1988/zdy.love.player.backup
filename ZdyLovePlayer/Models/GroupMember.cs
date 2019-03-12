using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Models
{
    public class GroupMember : IEntity
    {
        public int ID { get; set; }

        public int GroupID { get; set; }

        public int MediaID { get; set; }

        public Boolean? IsTop { get; set; }
    }
}
