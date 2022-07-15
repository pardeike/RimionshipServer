namespace RimionshipServer.Models
{
	public class StreamInformationHolder
	{
		public StreamInformation[] Data { get; set; }
	}

	public class StreamInformation
	{
		public string Id { get; set; }
		public string User_Id { get; set; }
		public string User_Login { get; set; }
		public string User_Name { get; set; }
		public string Game_Id { get; set; }
		public string Game_Name { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public int Viewer_Count { get; set; }
		public string Started_At { get; set; }
		public string Language { get; set; }
		public string Thumbnail_Url { get; set; }
		public string[] Tag_Ids { get; set; }
		public bool Is_Mature { get; set; }

		public override string ToString()
		{
			return $"Id={Id}, User_Id={User_Id}, User_Login={User_Login}, User_Name={User_Name}, Game_Id={Game_Id}, Game_Name={Game_Name}, Type={Type}, Title={Title}, Viewer_Count={Viewer_Count}, Started_At={Started_At}, Language={Language}, Thumbnail_Url={Thumbnail_Url}, Tag_Ids={string.Join("|", Tag_Ids)}, Is_Mature={Is_Mature}";
		}
	}
}
