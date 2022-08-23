using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RimionshipServer
{
	public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, long>
	{
		public DateTimeOffsetConverter(ConverterMappingHints? mappingHints = null)
			 : base(
				  value => value.UtcTicks,
				  value => new DateTimeOffset(value, TimeSpan.Zero),
				  mappingHints)
		{
		}

		public static ValueConverterInfo DefaultInfo { get; } =
			 new(typeof(DateTimeOffset), typeof(long), info => new DateTimeOffsetConverter(info.MappingHints));
	}
}
