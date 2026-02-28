namespace GameServer.Players;

public class PlayerEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required int Level { get; set; }
}
