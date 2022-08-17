namespace RimionshipServer.Data.Detailed;

public record BaseEntry(string UId, DateTime Time, int Id = default);

public record BaseIntRecord(BaseEntry    Id, int   Value);
public record BaseFloatRecord(BaseEntry    Id, float   Value);
public record DamageTakenPawns(BaseEntry Id, float Value) : BaseFloatRecord(Id, Value);

public record DamageTakenThings(BaseEntry Id, float Value) : BaseFloatRecord(Id, Value);

public record DamageDealt(BaseEntry Id, float Value) : BaseFloatRecord(Id, Value);

public record Colonists(BaseEntry Id, int Value) : BaseIntRecord (Id, Value);

public record Wealth(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record MapCount(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record ColonistsNeedTending(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record MedicalConditions(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Enemies(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record WildAnimals(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record TamedAnimals(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Visitors(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Prisoners(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record DownedColonists(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record MentalColonists(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Rooms(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Caravans(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record WeaponDps(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Electricity(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Medicine(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Food(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Fire(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Conditions(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record Temperature(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record NumRaidsEnemy(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record NumThreatBigs(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record ColonistsKilled(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record GreatestPopulation(BaseEntry Id, int Value): BaseIntRecord (Id, Value);

public record InGameHours(BaseEntry Id, int Value): BaseIntRecord (Id, Value);