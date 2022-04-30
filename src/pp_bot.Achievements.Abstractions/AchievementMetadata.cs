namespace pp_bot.Achievements;

public sealed class AchievementMetadata
{
	public const string IdType = nameof(Id);
	public const string NameType = nameof(Name);
	public const string DescriptionType = nameof(Description);
	
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
}