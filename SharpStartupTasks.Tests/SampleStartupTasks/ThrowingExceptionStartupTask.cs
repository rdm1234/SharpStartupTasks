using SharpStartupTasks.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Tests.SampleStartupTasks;

public class ThrowingExceptionStartupTask<TException> : ISyncStartupTask
    where TException : Exception, new()
{
    public void Execute(CancellationToken cancellationToken = default)
    {
        throw new TException();
    }
}

public class ThrowingExceptionStartupTask : ISyncStartupTask
{
    public void Execute(CancellationToken cancellationToken = default)
    {
        throw new Exception("Sample exception");
    }
}