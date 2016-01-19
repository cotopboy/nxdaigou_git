using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.infrastructure.ExtensionMethods
{

    public static class ReleaseObjectExtensionMethods
    {
        public static void ReleaseComObject(this object target)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(target);
                target = null;
            }
            catch (Exception ex)
            {
                target = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
