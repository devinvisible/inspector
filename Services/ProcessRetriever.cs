using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspector.Services
{
    class ProcessRetriever
    {
        public static async Task<Process[]> GetProcesses()
        {
            return await Task.Run(() =>
            {
                return Process.GetProcesses();
            });
        }
    }
}
