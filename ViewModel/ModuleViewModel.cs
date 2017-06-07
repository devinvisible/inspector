using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;

namespace Inspector.ViewModel
{
    class ModuleViewModel : ObservableObject
    {
        private ModuleDefMD _module;
        public ObservableCollection<TypeViewModel> Types { get; }
        public ICollectionView TypesCV { get; }

        public ModuleViewModel(ModuleDefMD module)
        {
            _module = module;

            Types = new ObservableCollection<TypeViewModel>();
            foreach (var type in _module.GetTypes())
            {
                Types.Add(new TypeViewModel(type));
            }

            TypesCV = CollectionViewSource.GetDefaultView(Types);
            (TypesCV as ListCollectionView).CustomSort = Comparer<TypeViewModel>.Create((a, b) => { return a.FullName.CompareTo(b.FullName); });
        }

        public string FullName => _module.FullName;
    }
}
