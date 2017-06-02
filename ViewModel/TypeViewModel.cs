using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using System.Collections.ObjectModel;

namespace Inspector.ViewModel
{
    class TypeViewModel : ViewModelBase
    {
        private TypeDef _type;
        public ObservableCollection<MethodViewModel> Methods { get; }

        public TypeViewModel(TypeDef type)
        {
            _type = type;

            Methods = new ObservableCollection<MethodViewModel>();
            foreach (var method in _type.Methods)
            {
                Methods.Add(new MethodViewModel(method));
            }
        }

        public String FullName => _type.FullName;
    }
}
