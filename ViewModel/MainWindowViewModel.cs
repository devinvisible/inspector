using Inspector.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            private set { SetValue(ref _selectedProcess, value); }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            private set { SetValue(ref _currentProgress, value); }
        }

        private int _maxProgress;
        public int MaxProgress
        {
            get { return _maxProgress; }
            private set { SetValue(ref _maxProgress, value); }
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

        private void UpdateProcessList()
        {
            Processes.Clear();

            var worker = new BackgroundWorker();
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                
                var processes = Process.GetProcesses();
                _maxProgress = processes.Length;

                int progress = 0;
                (sender as BackgroundWorker).ReportProgress(progress);

                foreach (Process p in processes)
                {
                    var process = new ProcessViewModel(p);
                    if (process.IsManaged) // contains mscoree.dll
                        App.Current.Dispatcher.Invoke(() => Processes.Add(process));

                    (sender as BackgroundWorker).ReportProgress(++progress);
                }
                base.OnPropertyChanged(nameof(Processes));
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                CurrentProgress = e.ProgressPercentage;
            };
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
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