using System;
using System.Threading.Tasks;

namespace RimionshipServer.Shared
{
	public static class Extensions
	{
		public static bool IsEmpty(this string str)
		{
			return str == null || str == "";
		}

		public static bool IsNotEmpty(this string str)
		{
			return str != null && str != "";
		}
	}

	public static class Run
	{
		public static void Sync(Func<Task> function)
		{
			Task.Run(function).Wait();
		}
	}
}
