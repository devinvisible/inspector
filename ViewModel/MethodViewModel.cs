using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;

namespace Inspector.ViewModel
{
    class MethodViewModel : ObservableObject
    {
        private MethodDef _method;

        public MethodViewModel(MethodDef method)
        {
            _method = method;
        }

        public string FullName => _method.FullName;
    }
}
