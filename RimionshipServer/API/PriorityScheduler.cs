using System.Collections.Concurrent;

//TAKEN FROM: https://stackoverflow.com/a/9056702 and adapted to .NET6 and our needs

namespace RimionshipServer.API
{
    public class PriorityScheduler : TaskScheduler
    {
        public static PriorityScheduler Lowest           = new (ThreadPriority.Lowest);
        public static PriorityScheduler LowestSingleCore = new (ThreadPriority.Lowest, 1);

        private          BlockingCollection<Task> _tasks = new ();
        private          Thread[]?                _threads;
        private          ThreadPriority           _priority;
        private readonly int                      _maximumConcurrencyLevel;

        public PriorityScheduler(ThreadPriority priority)
        {
            _priority                = priority;
            _maximumConcurrencyLevel = Math.Max(1, Environment.ProcessorCount);
        }
        
        public PriorityScheduler(ThreadPriority priority, int maximumConcurrencyLevel)
        {
            _priority                = priority;
            _maximumConcurrencyLevel = maximumConcurrencyLevel;
        }

        public override int MaximumConcurrencyLevel
        {
            get { return _maximumConcurrencyLevel; }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks;
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);

            if (_threads != null)
            {
                return;
            }

            _threads = new Thread[_maximumConcurrencyLevel];

            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(() =>
                                         {
                                             foreach (Task t in _tasks.GetConsumingEnumerable())
                                                 TryExecuteTask(t);
                                         }){
                                               Name         = $"PriorityScheduler: {i}",
                                               Priority     = _priority,
                                               IsBackground = true
                                           };
                _threads[i].Start();
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false; // we might not want to execute task that should schedule as high or low priority inline
        }
    }
}