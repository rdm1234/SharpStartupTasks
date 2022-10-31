SharpStartupTasks
-----------------

- [1 Short description](#1-short-description)
- [2 Installation](#2-installation)
  - [2.1 Install package](#21-install-package)
  - [2.2 Configure Program.cs](#22-configure-programcs)
- [3 Adding startup tasks](#3-adding-startup-tasks)
  - [3.1 Default startup task management](#31-default-startup-task-management)
    - [3.1.1 Add a single task](#311-add-a-single-task)
    - [3.1.2 Add multiple tasks](#312-add-multiple-tasks)
    - [3.1.3 Add all tasks from assembley](#313-add-all-tasks-from-assembley)
    - [3.1.4 Add all tasks from assembley except some tasks](#314-add-all-tasks-from-assembley-except-some-tasks)
    - [3.1.5 Add all tasks from assembley except some tasks which are added after in strict order](#315-add-all-tasks-from-assembley-except-some-tasks-which-are-added-after-in-strict-order)
    - [3.1.6 Add all tasks from assembley except some tasks and tasks are added after in strict order](#316-add-all-tasks-from-assembley-except-some-tasks-and-tasks-are-added-after-in-strict-order)
    - [3.1.7 Add tasks using factory method](#317-add-tasks-using-factory-method)
  - [3.2. Anonymous tasks](#32-anonymous-tasks)
    - [3.2.1 Simple adding tasks](#321-simple-adding-tasks)
    - [3.2.2 Other anonymous function types](#322-other-anonymous-function-types)
      - [3.2.2.1 Access cancellation token](#3221-access-cancellation-token)
      - [3.2.2.2 Access service provider](#3222-access-service-provider)
- [4. Startup tasks configuration](#4-startup-tasks-configuration)
  - [4.1 Ways to provide configuration](#41-ways-to-provide-configuration)
    - [4.1.1 For default startup tasks](#411-for-default-startup-tasks)
      - [4.1.1.1 Using `appsettings.json`](#4111-using-appsettingsjson)
      - [4.1.1.2 Using `AddStartupTasksConfiguration`](#4112-using-addstartuptasksconfiguration)
      - [4.1.1.3 Using `AddStartupTask`/`AddSyncStartupTask` overloads](#4113-using-addstartuptaskaddsyncstartuptask-overloads)
    - [4.1.2 For adding default startup tasks from assembleys](#412-for-adding-default-startup-tasks-from-assembleys)
    - [4.1.3 For anonymous startup tasks](#413-for-anonymous-startup-tasks)
      - [4.1.3.1 For **anonymous** tasks using `appsettings.json`/`AddStartupTasksConfiguration`](#4131-for-anonymous-tasks-using-appsettingsjsonaddstartuptasksconfiguration)
      - [4.1.3.2 For **anonymous** tasks using `AddStartupTask`/`AddSyncStartupTask` overloads](#4132-for-anonymous-tasks-using-addstartuptaskaddsyncstartuptask-overloads)
- [5. Remarks](#5-remarks)

# 1 Short description
**SharpStartupTasks** is project for simple management of startup tasks in Asp Net Core. 

**Startup task** in this context — task running only once at the application startup before app 
starts processing requests.

There are 2 types of startup tasks:
- **Async (default)** — running asynchronously. Related interface: `IStartupTask`.
- **Sync** — running synchronously. Related interface: `ISyncStartupTask`.

**Example**: [`Examples.ApsNetCoreWebApi`](https://github.com/rdm1234/SharpStartupTasks/tree/master/Examples.AspNetCoreWebApi) with dependent 
	[`Examples.SeparatedStartupTasks`](https://github.com/rdm1234/SharpStartupTasks/tree/master/Examples.SeparatedStartupTasks) project.
 
# 2 Installation
## 2.1 Install package
[![NuGet Package](https://img.shields.io/nuget/v/SharpStartupTasks?color=ff4081&label=NuGet%20Package&logo=nuget&style=flat-square)](https://www.nuget.org/packages/SharpStartupTasks/) 

You can get package using NuGet Package Manager in your IDE or through Package Manager Console:

```
Install-Package SharpStartupTasks -Version 2.0.0
```

Also you can use .NET CLI:

```
dotnet add package SharpStartupTasks --version 2.0.0
```

Or include package in .csproj:

```C#
<PackageReference Include="SharpStartupTasks" Version="2.0.0" />
```

## 2.2 Configure Program.cs
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
 
# 3 Adding startup tasks
## 3.1 Default startup task management
There is `IServiceCollection` extension methods for adding tasks:

### 3.1.1 Add a single task
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

### 3.1.2 Add multiple tasks 
This way you can both add sync and async tasks
```C#
services.AddMixedStartupTasks(
    typeof(FirstStartupTask), 
    typeof(SecondStartupTask), 
    typeof(SyncFirstStartupTask), 
    typeof(SyncSecondStartupTask));
```
### 3.1.3 Add all tasks from assembley
```C#
// From Startup.cs project assembley:
services.AddStartupTasksFromAssembleyOf<Startup>();

// From other assembley:
services.AddStartupTasksFromAssembleyOf<OtherAssembleyClass>();

// Also using typeof(...)
services.AddStartupTasksFromAssembleyOf(typeof(Startup));
```

### 3.1.4 Add all tasks from assembley except some tasks
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

### 3.1.5 Add all tasks from assembley except some tasks which are added after in strict order
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

### 3.1.6 Add all tasks from assembley except some tasks and tasks are added after in strict order
```C#
services.AddStartupTasksFromAssembleyThenOrdered<SomeStartupDependentService>(
    orderedStartupTasks: new[] { typeof(MustRunAfterAllSeparetedStartupTask) },
    exceptTypes: new[] { typeof(ShouldNotRunForNowSeparetedStartupTask) });

// Also using typeof(...) for assembley as first param
```

### 3.1.7 Add tasks using factory method
```C#
services.AddStartupTask((sp) =>
{
    var dependentService = sp.GetRequiredService<SomeStartupDependentService>();
    bool param = true;
    return new FirstFromFactorySeparetedStartupTask(dependentService, param);
});
```

## 3.2. Anonymous tasks
You can add anonymous tasks using `AddStartupTask` and `AddSyncStartupTask` methods:
### 3.2.1 Simple adding tasks
Async:
```C#
services.AddStartupTask(async () => 
{
    // Some async actions here
});
```
Sync:
```C#
services.AddSyncStartupTask(() => 
{
    // Some actions here
});
```
### 3.2.2 Other anonymous function types
> In samples async overloads are used, however sync are can be used in the same way.
#### 3.2.2.1 Access cancellation token
```C#
services.AddStartupTask(async cancellationToken => 
{
    // Some actions here
});
```
#### 3.2.2.2 Access service provider
```C#
services.AddStartupTask(async (serviceProvider, cancellationToken) => 
{
    // Some actions here
});
```

# 4. Startup tasks configuration
> TODO: Complete this section
## 4.1 Ways to provide configuration
### 4.1.1 For default startup tasks
#### 4.1.1.1 Using `appsettings.json`

#### 4.1.1.2 Using `AddStartupTasksConfiguration`

#### 4.1.1.3 Using `AddStartupTask`/`AddSyncStartupTask` overloads

### 4.1.2 For adding default startup tasks from assembleys



### 4.1.3 For anonymous startup tasks
#### 4.1.3.1 For **anonymous** tasks using `appsettings.json`/`AddStartupTasksConfiguration`

#### 4.1.3.2 For **anonymous** tasks using `AddStartupTask`/`AddSyncStartupTask` overloads


# 5. Remarks
- Tasks are called in order the were added to the DI. If you want to add all tasks from assembley 
in non-ordered way and add only some of them in the strict order after others, you should use 
one of overloads of `AddStartupTasksFromAssembleyThenOrdered`.
- For now `AddStartupTask(Type)` and `AddSyncStartupTask(Type)` works the same way. 
At the moment, the possibility of reworking the add and extract system is being considered, 
because of which they may work differently. At this point, it was decided to separate them for consistency.
