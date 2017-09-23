using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    [Serializable]
    public class SerivcesException : ApplicationException
    {
        public SerivcesException() { }  
        public SerivcesException(string message)  
            : base(message) { }
        public SerivcesException(string message, Exception inner)  
            : base(message, inner) { }  
    }
}
