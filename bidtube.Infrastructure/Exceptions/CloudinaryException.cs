using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Exceptions
{
    public class CloudinaryException : Exception
    {
        public CloudinaryException(string message) : base(message)
        {
        }
    }
}
