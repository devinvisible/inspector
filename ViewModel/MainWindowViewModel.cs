using Inspector.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Inspector.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<ProcessViewModel> Processes { get; }
        public ICollectionView ProcessesCV { get; }

        private ProcessViewModel _selectedProcess;
        public ProcessViewModel SelectedProcess
        {
            get { return _selectedProcess; }
            set { SetValue(ref _selectedProcess, value); }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            private set { SetValue(ref _currentProgress, value); }
        }

        public ICommand InspectProcessCommand { get; }

        public MainWindowViewModel()
        {
            Processes = new ObservableCollection<ProcessViewModel>();
            ProcessesCV = CollectionViewSource.GetDefaultView(Processes);
            (ProcessesCV as ListCollectionView).CustomSort = Comparer<ProcessViewModel>.Create((a, b) => { return a.Name.CompareTo(b.Name); });

            InspectProcessCommand = new RelayCommand(_ => InspectProcess(), _ => CanInspectProcess());

            UpdateProcessList();
            //SelectOurProcess();
        }

        private Task UpdateProcessList()
        {
            Processes.Clear();

            var progressHandler = new Progress<int>(value =>
            {
                CurrentProgress = value;
            });
            var progress = progressHandler as IProgress<int>;

            return Task.Run(() =>
            {
                var processes = Process.GetProcesses();
                int total = processes.Length;
                int current = 0;

                foreach (Process p in processes)
                {
                    var process = new ProcessViewModel(p);
                    if (process.IsManaged) // contains mscoree.dll
                        App.Current.Dispatcher.Invoke(() => Processes.Add(process));

                    progress?.Report(current * 100 / total);
                    current++;
                }
            });
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
        }
        
        public void InspectProcess()
        {
            SelectedProcess.ShowModules();
            base.OnPropertyChanged(nameof(SelectedProcess));
        }

        public bool CanInspectProcess() => true;
    }
}