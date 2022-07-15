namespace RimionshipServer.Models
{
	public class UserInformationHolder
	{
		public UserInformation[] Data { get; set; }
	}

	public class UserInformation
	{
		public string Id { get; set; }
		public string Login { get; set; }
		public string Broadcaster_Type { get; set; }
		public string Description { get; set; }
		public string Display_Name { get; set; }
		public string Offline_Image_Url { get; set; }
		public string Profile_Image_Url { get; set; }
		public string Type { get; set; }
		public int View_Count { get; set; }
		public string Email { get; set; }
		public string Created_At { get; set; }

		public override string ToString()
		{
			return $"Id={Id}, Login={Login}, Broadcaster_Type={Broadcaster_Type}, Description={Description}, Display_Name={Display_Name}, Offline_Image_Url={Offline_Image_Url}, Profile_Image_Url={Profile_Image_Url}, Type={Type}, View_Count={View_Count}, Emai={Email}, Created_At={Created_At}";
		}
	}
}
