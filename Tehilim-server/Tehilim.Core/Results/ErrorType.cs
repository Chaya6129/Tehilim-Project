using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.Results
{
    public enum ErrorType
    {
        None,
        NotFound,
        Unauthorized,
        Forbidden,
        Conflict,
        Validation
    }
}

