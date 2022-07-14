using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace RimionshipServer.Services
{
	public class ConsoleLoggingProvider : ILoggerProvider
	{
		private readonly List<ILogger> loggers = new();

		public ILogger CreateLogger(string categoryName)
		{
			var logger = new ConsoleLogger();
			loggers.Add(logger);

			return logger;
		}

		public void Dispose() => loggers.Clear();
	}

	public class ConsoleLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state) => null;
		public bool IsEnabled(LogLevel logLevel) => true;

		static readonly string[] levels = new[]
		{
			"TRC",
			"DBG",
			"INF",
			"WAR",
			"ERR",
			"CRT",
			""
		};

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write($"{DateTime.Now:yyyyMMddHHmmss.fff} ");

			Console.ForegroundColor = logLevel <= LogLevel.Information ? ConsoleColor.Green : (logLevel == LogLevel.Warning ? ConsoleColor.Yellow : ConsoleColor.Red);
			Console.Write($"{levels[(int)logLevel]} ");
			Console.ResetColor();

			Console.Write(formatter(state, exception));
			Console.WriteLine();
		}
	}
}
