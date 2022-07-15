using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RimionshipServer.Shared
{
	public static class EfHelper
	{
		public static DbTransaction GetDbTransaction(this IDbContextTransaction source)
		{
			return (source as IInfrastructure<DbTransaction>).Instance;
		}

		public static async IAsyncEnumerable<T> FromSqlQuery<T>(this DbContext context, string query, Func<IDataRecord, T> projector, params object[] parameters) where T : new()
		{
			using var command = context.Database.GetDbConnection().CreateCommand();
			if (command.Connection.State != ConnectionState.Open)
				command.Connection.Open();
			var currentTransaction = context.Database.CurrentTransaction;
			if (currentTransaction != null)
				command.Transaction = currentTransaction.GetDbTransaction();
			command.CommandText = query;
			if (parameters.Any())
				command.Parameters.AddRange(parameters);

			using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
				yield return projector(reader);
		}
	}
}
