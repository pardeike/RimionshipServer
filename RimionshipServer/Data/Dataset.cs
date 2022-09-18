using System.Drawing;
using System.Runtime.CompilerServices;
using RimionshipServer.Pages.Api;
namespace RimionshipServer.Data
{
    public record Dataset
    {
        public Dataset(string label, Color color, IEnumerable<DataEntry> data)
        {
            this.label      = label;
            borderColor     = HexConverter(color);
            this.data       = data;
            backgroundColor = HexConverter(color);
        }

        public string                        backgroundColor { get; init; }
        public string                        label           { get; init; }
        public string                        borderColor     { get; init; }
        public IEnumerable<DataEntry> data            { get; init; }
        public bool                          showLine        { get; init; } = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
    public record DataEntry(long x, string y);
}