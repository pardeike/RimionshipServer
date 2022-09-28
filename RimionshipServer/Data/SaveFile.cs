namespace RimionshipServer.Data
{
    public class SaveFile
    {
        public int    Id   { get; set; }
        public byte[] File { get; set; }  = null!;
        public string Name { get; set; }  = null!;
        public string MD5  { get; set; }  = null!;
    }
}