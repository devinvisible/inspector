using Inspector.Commands;
using Inspector.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Inspector.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<ProcessViewModel> Processes { get; }

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

            var view = CollectionViewSource.GetDefaultView(Processes);
            (view as ListCollectionView).CustomSort = Comparer<ProcessViewModel>.Create((a, b) => { return a.Name.CompareTo(b.Name); });

            InspectProcessCommand = new RelayCommand(_ => InspectProcess(), _ => CanInspectProcess());

            UpdateProcessList();
            SelectOurProcess();
        }

        private async Task UpdateProcessList()
        {
            var processes = await ProcessRetriever.GetProcesses();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            int current = 0;
            int total = processes.Length;
            Processes.Clear();
            var tasks = new List<Task>();
            foreach (Process p in processes)
            {
                tasks.Add(Task.Run(() =>
                {
                    var process = new ProcessViewModel(p);
                    if (process.IsManaged) // contains mscoree.dll
                        RunOnScheduler(() => Processes.Add(process), scheduler);

                    CurrentProgress = (Interlocked.Increment(ref current) * 100 / total);
                }
                ));
            }
            await Task.WhenAll(tasks);
        }

        //private async Task UpdateProcessList()
        //{
        //    var processes = await ProcessRetriever.GetProcesses();
        //    Processes.Clear();
        //    //var list = processes.AsParallel().Select(x => new ProcessViewModel(x)).Where(x => x.IsManaged);
        //    var list = await Task.Run(() => processes.AsParallel().Select(x => new ProcessViewModel(x)).Where(x => x.IsManaged).ToList());
        //    foreach (var item in list) Processes.Add(item);
        //}

        private void RunOnScheduler(Action action, TaskScheduler scheduler)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler).Wait();
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