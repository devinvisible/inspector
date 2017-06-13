using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace Inspector.ViewModel
{
    public enum HouseType
    {
        Cabin,
        Shack,
        Mansion
    }

    interface foo
    {
        Object thing { get; }
    }

    class ProcessViewModel : ObservableObject
    {
        private Process _process;
        public ObservableCollection<ModuleViewModel> Modules { get; }
        public ICollectionView ModulesCV { get; }
        public int TotalModules { get; set; }

        public ProcessViewModel(Process process)
        {
            _process = process;
            
            Modules = new ObservableCollection<ModuleViewModel>();
            ModulesCV = CollectionViewSource.GetDefaultView(Modules);
            (ModulesCV as ListCollectionView).CustomSort = Comparer<ModuleViewModel>.Create((a, b) => { return a.FullName.CompareTo(b.FullName); });
        }

        public int Id => _process.Id;
        public string Name => _process.ProcessName;

        public bool IsManaged
        {
            get
            {
                try
                {
                    var modules = _process.Modules;
                    foreach (ProcessModule module in modules)
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

        public void ShowModules()
        {
            if (Modules.Count == 0)
            {
                TotalModules = 0;
                foreach (ProcessModule m in _process.Modules)
                {
                    try
                    {
                        ModuleDefMD module = ModuleDefMD.Load(m.FileName);
                        Modules.Add(new ModuleViewModel(module));
                    }
                    catch (BadImageFormatException) { }
                    
                    TotalModules++;
                }
            }
            base.OnPropertyChanged(nameof(Modules));
            base.OnPropertyChanged(nameof(TotalModules));
        }
    }
}