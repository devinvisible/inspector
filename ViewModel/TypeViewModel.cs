using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Inspector.ViewModel
{
    class TypeViewModel : ObservableObject
    {
        private TypeDef _type;
        public ObservableCollection<MethodViewModel> Methods { get; }
        public ICollectionView MethodsCV { get; }

        public TypeViewModel(TypeDef type)
        {
            _type = type;

            Methods = new ObservableCollection<MethodViewModel>();
            foreach (var method in _type.Methods)
            {
                Methods.Add(new MethodViewModel(method));
            }

            MethodsCV = CollectionViewSource.GetDefaultView(Methods);
            (MethodsCV as ListCollectionView).CustomSort = Comparer<MethodViewModel>.Create((a, b) => { return a.FullName.CompareTo(b.FullName); });
        }

        public String FullName => _type.FullName;
    }
}
