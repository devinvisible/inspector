using inspector.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace inspector.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        ObservableCollection<ProcessViewModel> _processes;
        public ObservableCollection<ProcessViewModel> Processes
        {
            get
            {
                if (_processes == null)
                {
                    _processes = new ObservableCollection<ProcessViewModel>();
                }
                return _processes;
            }
        }

        public ProcessViewModel SelectedProcess { get; set; }

        public MainWindowViewModel()
        {
            UpdateProcessList();
            SelectOurProcess();
        }

        private void UpdateProcessList()
        {
            Processes.Clear();
            foreach (Process p in Process.GetProcesses())
            {
                var process = new ProcessViewModel(p);
                if (process.IsManaged) // contains mscoree.dll
                    Processes.Add(process);
            }
            base.OnPropertyChanged("Processes");
        }

        private void SelectOurProcess()
        {
            var our_id = Process.GetCurrentProcess().Id;
            foreach (ProcessViewModel p in Processes)
            {
                if (p.Id == our_id)
                {
                    SelectedProcess = p;
                    return;
                }
            }
            base.OnPropertyChanged("SelectedProcess");
        }

        private ICommand _inspectProcessCommand;
        public ICommand InspectProcessCommand
        {
            get
            {
                if (_inspectProcessCommand == null)
                {
                    _inspectProcessCommand = new RelayCommand(_ => InspectProcess(), _ => CanInspectProcess());
                }
                return _inspectProcessCommand;
            }
        }

        public void InspectProcess()
        {
            Debug.WriteLine("InspectProcess executed...");

            var process = SelectedProcess;

            //trvModules.Items.Clear();
            //foreach (ProcessModule module in process.Modules)
            //{
            //    try
            //    {
            //        var assembly = Assembly.ReflectionOnlyLoadFrom(module.FileName);
            //        trvModules.Items.Add(assembly);
            //    }
            //    catch (BadImageFormatException) { }
            //    catch (FileLoadException) { }
            //}
        }

        public bool CanInspectProcess() => true;
    }
}