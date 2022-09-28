namespace RimionshipServer.Data
{
    public class GraphRotationData
    {
        public         int                    Id            { get; set; }
        public         string                 RotationName  { get; set; } = null!;
        public         int                    TimeToDisplay { get; set; }
        public virtual IEnumerable<GraphData> ToRotate      { get; set; } = null!;
    }
}