namespace NextStopAnnouncementGenerator.Google
{
	public class SsmlStopName
	{
		public string StopName { get; set; }
		public string SsmlOverride { get; set; }

		public override string ToString() => StopName;
	}
}
