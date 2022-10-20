using System;

namespace SharpStartupTasks.Abstractions
{
	public abstract class AnonymousStartupTaskBase
	{
		public AnonymousStartupTaskBase(string? id)
		{
			Id = id ?? Guid.NewGuid().ToString();
		}

		public string Id { get; }
	}
}