namespace Examples.SeparatedStartupTasks
{
    public class SomeStartupDependentService
    {
        public bool FirstStartupTaskCompleted { get; set; }
        public bool SecondStartupTaskCompleted { get; set; }
        public bool SyncStartupTaskCompleted { get; set; }
        public bool MustRunAfterAllTaskCompledCorrect { get; internal set; }
    }
}
