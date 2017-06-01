using System;
using System.ComponentModel;
using System.Diagnostics;

namespace inspector.ViewModel
{
    public class ProcessViewModel
    {
        private Process _process;

        public ProcessViewModel(Process process)
        {
            _process = process;
        }

        public int Id => _process.Id;
        public string Name => _process.ProcessName;

        public bool IsManaged
        {
            get
            {
                try
                {
                    foreach (ProcessModule module in _process.Modules)
                    {
                        if (module.ModuleName.ToLower() == "mscoree.dll")
                            return true;
                    }
                }
                catch (Win32Exception) { } // 'Access is denied.'
                catch (InvalidOperationException) { } // 'Cannot process request because the process has exited.'

                return false;
            }
        }
    }
}