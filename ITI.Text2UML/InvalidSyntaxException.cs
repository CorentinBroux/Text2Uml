using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class InvalidSyntaxException :  Exception
    {
        public InvalidSyntaxException(string message):base(message)
        {
        }
    }
}
