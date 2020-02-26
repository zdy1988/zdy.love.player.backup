using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDY.LovePlayer.Untils
{
    public class AssemblyHelper
    {
        private static readonly string NameSpaceStr = typeof(AssemblyHelper).Assembly.GetName().Name;

        public static object CreateInternalInstance(string className)
        {
            try
            {
                var type = Type.GetType($"{NameSpaceStr}.{className}");
                return type == null ? null : Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }
    }
}
