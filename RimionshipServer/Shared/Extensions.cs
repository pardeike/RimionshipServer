namespace RimionshipServer
{
	public static class Extensions
	{
		public static bool IsEmpty(this string str)
		{
			return str == null || str == "";
		}

		public static bool IsNotEmpty(this string str)
		{
			return str != null && str != "";
		}
	}
}
