using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lorei.CScode.Interfaces
{
    public interface ApiProvider
    {
        /**
         * Reflective method to get a list of public methods in an ApiProvider class
         */
        List<System.Reflection.MethodInfo> GetMethods();
    }
}
