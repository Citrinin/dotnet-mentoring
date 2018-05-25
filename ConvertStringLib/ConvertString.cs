using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertStringLib
{
    public class ConvertString
    {
        public int ConvertToInt(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(source, "String must be not null and contain numbers");
            }
            return Convert.ToInt32(source);
        }
    }
}
