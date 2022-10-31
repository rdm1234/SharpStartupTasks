using SharpStartupTasks.Abstractions;
using SharpStartupTasks.Abstractions.Attributes;
using SharpStartupTasks.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Services
{
	public sealed class StartupTaskConfigurationProvider : IStartupTaskConfigurationProvider
	{
		private readonly List<SharpStartupTasksConfiguration> _configurationContexts;
		private readonly List<StartupTaskConfiguration> _configurations;
		private readonly IEnumerable<IBaseStartupTask> _startupTasks;

		public StartupConfigurationNode GlobalConfig { get; }

		public StartupTaskConfigurationProvider(
			IEnumerable<SharpStartupTasksConfiguration> configurations,
			IEnumerable<IBaseStartupTask> startupTasks)
		{
			_configurationContexts = configurations.ToList();
			GlobalConfig = GetGlobalConfig(_configurationContexts);
			_configurations = AggregateStartupTasksConfiguration(_configurationContexts, GlobalConfig, startupTasks);
			_startupTasks = startupTasks;
		}

		private static StartupConfigurationNode GetGlobalConfig(List<SharpStartupTasksConfiguration> configurations)
		{
			var contatiningGlobalConfigSection = configurations.SingleOrDefault(x => x.Global != null);

			var cfg = contatiningGlobalConfigSection?.Global;

			if (cfg == null)
			{
				cfg = new();
			}

			cfg.Disabled = cfg.Disabled ?? false;
			cfg.IgnoreAllExceptions = cfg.IgnoreAllExceptions ?? false;

			return cfg;
		}

		private static List<StartupTaskConfiguration> AggregateStartupTasksConfiguration(IEnumerable<SharpStartupTasksConfiguration> configurations, StartupConfigurationNode globalCfg, IEnumerable<IBaseStartupTask> startupTasks)
		{
			var result = new List<StartupTaskConfiguration>();

			AddConfigurationsFromStartupTaskContexts(configurations, globalCfg, result);
			AddConfigurationsFromAttributesOrDefaultOnes(globalCfg, startupTasks, result);

			return result;
		}

		private static void AddConfigurationsFromAttributesOrDefaultOnes(StartupConfigurationNode globalCfg, IEnumerable<IBaseStartupTask> startupTasks, List<StartupTaskConfiguration> result)
		{
			foreach (var st in startupTasks)
			{
				var stType = st.GetType();

				var relatedCfg = result.FirstOrDefault(RelatedTaskFilter(st.GetType()));

				if (relatedCfg == null)
				{
					relatedCfg = AddDefaultConfiguration(globalCfg, result);
				}

				AddConfigurationFromAttribute(stType, relatedCfg);

				if (relatedCfg.AnonymousTaskId == null)
				{
					relatedCfg.Namespace = stType.Namespace;
					relatedCfg.Name = stType.Name;
					relatedCfg.Type = stType;
				}
			}
		}

		private static void AddConfigurationFromAttribute(Type stType, StartupTaskConfiguration? relatedCfg)
		{
			var attr = stType.GetCustomAttribute<StartupTaskAttribute>();

			if (attr != null)
			{
				relatedCfg.EnabledEnvironments = relatedCfg.EnabledEnvironments ?? attr.EnabledEnvironments;
				relatedCfg.Disabled = relatedCfg.Disabled ?? attr.Disabled;
				relatedCfg.IgnoreExceptionTypes = relatedCfg.IgnoreExceptionTypes ?? attr.IgnoreExceptionTypes;
			}
		}

		private static StartupTaskConfiguration AddDefaultConfiguration(StartupConfigurationNode globalCfg, List<StartupTaskConfiguration> result)
		{
			StartupTaskConfiguration? relatedCfg = new()
			{
				IgnoreExceptions = globalCfg.IgnoreExceptions,
				IgnoreAllExceptions = globalCfg.IgnoreAllExceptions,
				EnabledEnvironments = globalCfg.EnabledEnvironments,
				Disabled = globalCfg.Disabled,
				IgnoreExceptionTypes = globalCfg.IgnoreExceptionTypes
			};
			result.Add(relatedCfg);
			return relatedCfg;
		}

		private static void AddConfigurationsFromStartupTaskContexts(IEnumerable<SharpStartupTasksConfiguration> configurations, StartupConfigurationNode globalCfg, List<StartupTaskConfiguration> result)
		{
			foreach (var tasksContext in configurations)
			{
				if (tasksContext.Tasks == null)
					continue;

				var currCtxCfg = tasksContext.CurrentSet;
				bool currCtxDisabled = currCtxCfg?.Disabled ?? globalCfg.Disabled!.Value;
				bool currCtxIgnoreAllExceptions = currCtxCfg?.IgnoreAllExceptions ?? globalCfg.IgnoreAllExceptions!.Value;
				var currCtxIgnoreExceptionList = currCtxCfg?.IgnoreExceptions ?? globalCfg.IgnoreExceptions;
				var currCtxEnabledEnvironments = currCtxCfg?.EnabledEnvironments ?? globalCfg.EnabledEnvironments;
				var currCtxIgnoreExceptionTypes = currCtxCfg?.IgnoreExceptionTypes ?? globalCfg.IgnoreExceptionTypes;

				foreach (var taskCfg in tasksContext.Tasks)
				{
					var actualCfg = new StartupTaskConfiguration()
					{
						AnonymousTaskId = taskCfg.AnonymousTaskId,
						Name = taskCfg.Name,
						Namespace = taskCfg.Namespace,
						Type = taskCfg.Type,
						IgnoreAllExceptions = taskCfg.IgnoreAllExceptions ?? currCtxIgnoreAllExceptions,
						IgnoreExceptions = taskCfg.IgnoreExceptions ?? currCtxIgnoreExceptionList,
						Disabled = taskCfg.Disabled ?? currCtxDisabled,
						EnabledEnvironments = taskCfg.EnabledEnvironments ?? currCtxEnabledEnvironments,
						IgnoreExceptionTypes = taskCfg.IgnoreExceptionTypes ?? currCtxIgnoreExceptionTypes
					};

					result.Add(actualCfg);
				}
			}
		}

		public StartupConfigurationNode Get(IBaseStartupTask st)
		{
			if (st is AnonymousStartupTaskBase anonymousStartupTask)
			{
				var cfg = _configurations.FirstOrDefault(RelatedAnonymousTaskFilter(anonymousStartupTask));
				return cfg ?? GlobalConfig;
			}

			return _configurations.First(RelatedTaskFilter(st.GetType()));
		}

		private static Func<StartupTaskConfiguration, bool> RelatedTaskFilter(Type type)
		{
			return x => x.Type == type || (x.Name == type.Name && x.Namespace == type.Namespace);
		}

		private static Func<StartupTaskConfiguration, bool> RelatedAnonymousTaskFilter(AnonymousStartupTaskBase st)
		{
			return x => x.AnonymousTaskId == st.Id;
		}
	}
}