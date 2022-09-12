namespace RimionshipServer.Data
{
    public class SaveFile
    {
        public int    Id   { get; set; }
        public byte[] File { get; set; }
        public string Name { get; set; }
        public string MD5  { get; set; }
    }
}