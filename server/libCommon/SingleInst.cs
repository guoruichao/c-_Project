using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libCommon
{
   public class SingleInst <T>
        where T : new()
    {
        private static T t;
        public static T GetInstance()
        {
            if (t == null)
            {
                t = new T();
            }
            return t;
        }
    }
}
