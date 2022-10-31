using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StartupTaskAttribute : Attribute
{
    public bool IgnoreAllExceptions { get; set; }
    public string[]? EnabledEnvironments { get; set; }
    public List<string> IgnoreExceptions { get; set; }
    public bool? Disabled { get; set; }
    public Type[]? IgnoreExceptionTypes { get; set; }
}
