using GPLX.Database.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GPLX.Database
{
    public static class DbInitializer
    {
        public static void Initialize(Context context)
        {
            try
            {
                if (!context.Database.EnsureCreated())
                    return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
