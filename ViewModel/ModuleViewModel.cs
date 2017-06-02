using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using System.Collections.ObjectModel;

namespace Inspector.ViewModel
{
    class ModuleViewModel : ViewModelBase
    {
        private ModuleDefMD _module;
        public ObservableCollection<TypeViewModel> Types { get; }

        public ModuleViewModel(ModuleDefMD module)
        {
            _module = module;

            Types = new ObservableCollection<TypeViewModel>();
            foreach (var type in _module.GetTypes())
            {
                Types.Add(new TypeViewModel(type));
            }
        }

        public string FullName => _module.FullName;
    }
}
