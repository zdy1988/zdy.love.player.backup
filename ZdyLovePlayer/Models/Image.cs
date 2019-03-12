using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Models
{
    public class Image : IEntity
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public int TypeSourceID { get; set; }
        public string Path { get; set; }
    }
}
