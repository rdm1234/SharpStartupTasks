namespace Examples.AspNetCoreWebApi.Services
{
    public class MainStartupDependentService
    {
        public bool FirstStartupTaskCompleted { get; set; }
        public bool SecondStartupTaskCompleted { get; set; }
        public bool FirstSyncStartupTaskCompleted { get; set; }
        public bool SecondSyncStartupTaskCompleted { get; set; }
        public bool MustRunAfterAllStartupTaskCompletedCorrect { get; set; }
        public bool SecondMustRunAfterAllStartupTaskCompletedCorrect { get; set; }
    }
}
