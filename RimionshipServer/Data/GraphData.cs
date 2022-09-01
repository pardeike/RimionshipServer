namespace RimionshipServer.Data
{
    public class GraphData
    {
        public int            Id              { get; set; }
        public string         Accesscode      { get; set; }
        public string         Secret          { get; set; }
        public string         Statt           { get; set; }
        public string[]       Users           { get; set; }
        public DateTimeOffset Start           { get; set; }
        public DateTimeOffset End             { get; set; }
        public int            IntervalSeconds { get; set; }
    }
}