using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.Exceptions
{
    public class ConcurrencyConflictException: Exception
    {
        public ConcurrencyConflictException(string message, Exception? innerException = null)
    : base(message, innerException)
        {
        }
    }
}
