using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lorei
{
    public interface ApiProvider
    {
        // Get list of methods provided by this api
        List<System.Reflection.MethodInfo> GetMethods();
    }
}
