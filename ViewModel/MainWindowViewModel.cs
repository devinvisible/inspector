using Inspector.Commands;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Inspector.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ProcessViewModel> Processes { get; }
        public ProcessViewModel SelectedProcess { get; set; }

        public ICommand InspectProcessCommand { get; }

        public MainWindowViewModel()
        {
            Processes = new ObservableCollection<ProcessViewModel>();

            InspectProcessCommand = new RelayCommand(_ => InspectProcess(), _ => CanInspectProcess());

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
            base.OnPropertyChanged(nameof(Processes));
        }

        private void SelectOurProcess()
        {
            var our_id = Process.GetCurrentProcess().Id;
            foreach (ProcessViewModel p in Processes)
            {
                if (p.Id == our_id)
                {
                    SelectedProcess = p;
                    break;
                }
            }
            base.OnPropertyChanged(nameof(SelectedProcess));
        }
        
        public void InspectProcess()
        {
            SelectedProcess.ShowModules();
            base.OnPropertyChanged(nameof(SelectedProcess));
        }

        public bool CanInspectProcess() => true;
    }
}