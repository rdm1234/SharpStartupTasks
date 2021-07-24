# SharpStartupTasks
1. [Short description](#ShortDescription)
2. [Installation](#Installation)
<br/>2.1 [Install package](#Installation)
<br/>2.2 [Configure Program.cs](#ConfigureProgramCs)
3. [Documentation](#Documentation)
<br/>3.1. [Add a single task](#AddSingleTask)
<br/>3.2. [Add multiple tasks](#AddMultipleTasks)
<br/>3.3. [Add tasks from assembley](#AddFromAssembley)
<br/>3.4. [Add tasks from assembley except some types](#AddFromAssembleyExceptTypes)
<br/>3.5. [Add tasks from assembley except some tasks which are added after in strict order](#AddFromAssembleyTypesThenOrdered)
<br/>3.6. [Add tasks from assembley except some tasks and tasks are added after in strict order](#AddFromAssembleyTypesThenOrderedExcept)
4. [Remarks](#Remarks)

<a name="shortDescription"></a> 
## 1. Short description
<b>SharpStartupTasks</b> is project for	simple management of startup tasks in Asp Net Core. 

<b>Startup task</b> in this context — task running only once at the application startup before app 
starts processing requests.

There are 2 types of startup tasks:
- <b>Async (default)</b> — running asynchronously. Related interface: `IStartupTask`.
- <b>Sync</b> — running synchronously. Related interface: `ISyncStartupTask`.

<b>Example</b>: [`Examples.ApsNetCoreWebApi`](https://github.com/rdm1234/SharpStartupTasks/tree/master/Examples.AspNetCoreWebApi) with dependent 
	[`Examples.SeparatedStartupTasks`](https://github.com/rdm1234/SharpStartupTasks/tree/master/Examples.SeparatedStartupTasks) project.

<a name="Installation"></a> 
## 2. Installation
### 2.1 Install package
[![NuGet Package](https://img.shields.io/nuget/v/SharpStartupTasks?color=ff4081&label=NuGet%20Package&logo=nuget&style=flat-square)](https://www.nuget.org/packages/SharpStartupTasks/) 

You can get package using NuGet Package Manager in your IDE or through Package Manager Console:

```
Install-Package SharpStartupTasks -Version 1.0.3.1
```

Also you can use .NET CLI:

```
dotnet add package SharpStartupTasks --version 1.0.3.1
```

Or include package in .csproj:

```C#
<PackageReference Include="SharpStartupTasks" Version="1.0.3.1" />
```

<a name="ConfigureProgramCs"></a> 
### 2.2 Configure Program.cs
Your Program.cs Main method should look like this:
```C#
public static async Task Main(string[] args)
{
    /* If you want to log information about execution of startup tasks 
     *  you should provide ILogger to the RunWithTasksAsync method.
     *  In this example, SerilogLoggerFactory is used for this purpose. */
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    var factory = new SerilogLoggerFactory(Log.Logger);
    var logger = factory.CreateLogger("SharpStartupTasks.HostExtensions");
    
    // Call RunWithTasksAsync extension method instead of Run or RunAsync
    await CreateHostBuilder(args).Build().RunWithTasksAsync(logger);
}
```

You can also provide cancellation token to RunWithTasksAsync method:
```C#
RunWithTasksAsync(cancellationToken: cancellationTokenSource.Token)
```

<a name="Documentation"></a> 
## 3. Documentation
There is `IServiceCollection` extension methods for adding tasks:

<a name=AddSingleTask>1. To add a single task:</a>
```c#
// To use async startup tasks:
services.AddStartupTask<SomeAsyncStartupTask>();
// or
services.AddStartupTask(tyepof(SomeAsyncStartupTask));

// To use sync startup tasks:
services.AddSyncStartupTask<SomeSyncStartupTask>();
// or
services.AddSyncStartupTask(tyepof(SomeSyncStartupTask));
```

<a name="AddMultipleTasks">2. To add multiple tasks (this way you can both add sync and async tasks):</a>
```C#
services.AddMixedStartupTasks(
    typeof(FirstStartupTask), 
    typeof(SecondStartupTask), 
    typeof(SyncFirstStartupTask), 
    typeof(SyncSecondStartupTask));
```
<a name="AddFromAssembley">3. To add all tasks from assembley of:</a>
```C#
// From Startup.cs project assembley:
services.AddStartupTasksFromAssembleyOf<Startup>();

// From other assembley:
services.AddStartupTasksFromAssembleyOf<OtherAssembleyClass>();

// Also using typeof(...)
services.AddStartupTasksFromAssembleyOf(typeof(Startup));
```

<a name="AddFromAssembleyExceptTypes">4. To add tasks from assembley of type except some (for example, you need to disable them for some time):</a>
```C#
services.AddStartupTasksFromAssembleyOf<Startup>(
    typeof(ShouldNotRunForNowStartupTask), 
    typeof(SecondShouldNotRunForNowStartupTask));

// or
services.AddStartupTasksFromAssembleyOf<Startup>(
    exceptTypes: new[] 
    { 
        typeof(MustRunAfterAllStartupTask),
        typeof(SecondMustRunAfterAllStartupTask)
    });

// Also using typeof(...) for assembley as first param
```

<a name="AddFromAssembleyTypesThenOrdered">5. To add tasks from assembley except some which are added be added later in strict order:</a>
```C#
services.AddStartupTasksFromAssembleyThenOrdered<SomeStartupDependentService>(
    typeof(MustRunAfterAllSeparetedStartupTask),
    typeof(SecondMustRunAfterAllSeparetedStartupTask));

// or
services.AddStartupTasksFromAssembleyThenOrdered<SomeStartupDependentService>(
    orderedStartupTasks: new[] 
    { 
        typeof(MustRunAfterAllSeparetedStartupTask), 
        typeof(SecondMustRunAfterAllSeparetedStartupTask) 
    });
```

<a name="AddFromAssembleyTypesThenOrderedExcept"> 6. To add tasks from assembley except types and except which are added be added later in strict order:</a>
```C#
services.AddStartupTasksFromAssembleyThenOrdered<SomeStartupDependentService>(
    orderedStartupTasks: new[] { typeof(MustRunAfterAllSeparetedStartupTask) },
    exceptTypes: new[] { typeof(ShouldNotRunForNowSeparetedStartupTask) });

// Also using typeof(...) for assembley as first param
```

## 4. Remarks
- Tasks are called in order the were added to the DI. If you want to add all tasks from assembley 
in non-ordered way and add only some of them in the strict order after others, you should use 
one of overloads of `AddStartupTasksFromAssembleyThenOrdered`.
- For now `AddStartupTask(Type)` and `AddSyncStartupTask(Type)` works the same way. 
At the moment, the possibility of reworking the add and extract system is being considered, 
because of which they may work differently. At this point, it was decided to separate them for consistency.