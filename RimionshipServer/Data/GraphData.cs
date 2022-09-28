namespace RimionshipServer.Data
{
    public class GraphData
    {
        public         int                            Id              { get; set; }
        public         string                         Accesscode      { get; set; } = null!;
        public         string                         Statt           { get; set; } = null!;
        public         DateTimeOffset                 Start           { get; set; }
        public         DateTimeOffset                 End             { get; set; }
        public         int                            IntervalSeconds { get; set; }
        public         bool                           Autorefresh     { get; set; }
        public         string[]                       Users           { get => UsersReference.Select(x => x.UserName).ToArray(); }
        public virtual IEnumerable<RimionUser>        UsersReference  { get; set; } = new List<RimionUser>();
        public         int?                           CountUser       { get; set; }
        public virtual IEnumerable<GraphRotationData> InRotations     { get; set; } = null!;
    }
}