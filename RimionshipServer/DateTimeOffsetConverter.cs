using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RimionshipServer
{
    public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, long>
    {
        public DateTimeOffsetConverter(ConverterMappingHints? mappingHints = null)
            : base(
                value => value.UtcDateTime.Ticks,
                value => new DateTimeOffset(value, TimeSpan.Zero),
                mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo { get; } = 
            new(typeof(DateTimeOffset), typeof(long), info => new DateTimeOffsetConverter(info.MappingHints));
    }
}
