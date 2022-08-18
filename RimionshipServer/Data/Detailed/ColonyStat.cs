using RimionshipServer.API;
namespace RimionshipServer.Data.Detailed;

public enum ColonyStat
{
    DamageTakenPawns      = StatsRequest.DamageTakenPawnsFieldNumber,
    DamageTakenThings     = StatsRequest.DamageTakenThingsFieldNumber,
    DamageDealt           = StatsRequest.DamageDealtFieldNumber,
    Colonists= StatsRequest.ColonistsFieldNumber,
    Wealth   = StatsRequest.WealthFieldNumber,
    MapCount = StatsRequest.MapCountFieldNumber,
    ColonistsNeedTending  = StatsRequest.ColonistsNeedTendingFieldNumber,
    MedicalConditions     = StatsRequest.MedicalConditionsFieldNumber,
    Enemies  = StatsRequest.EnemiesFieldNumber,
    WildAnimals           = StatsRequest.WildAnimalsFieldNumber,
    TamedAnimals          = StatsRequest.TamedAnimalsFieldNumber,
    Visitors = StatsRequest.VisitorsFieldNumber,
    Prisoners= StatsRequest.PrisonersFieldNumber,
    DownedColonists       = StatsRequest.DownedColonistsFieldNumber,
    MentalColonists       = StatsRequest.MentalColonistsFieldNumber,
    Rooms    = StatsRequest.RoomsFieldNumber,
    Caravans = StatsRequest.CaravansFieldNumber,
    WeaponDps= StatsRequest.WeaponDpsFieldNumber,
    Electricity           = StatsRequest.ElectricityFieldNumber,
    Medicine = StatsRequest.MedicineFieldNumber,
    Food     = StatsRequest.FoodFieldNumber,
    Fire     = StatsRequest.FireFieldNumber,
    Conditions            = StatsRequest.ConditionsFieldNumber,
    Temperature           = StatsRequest.TemperatureFieldNumber,
    NumRaidsEnemy         = StatsRequest.NumRaidsEnemyFieldNumber,
    NumThreatBigs         = StatsRequest.NumThreatBigsFieldNumber,
    ColonistsKilled       = StatsRequest.ColonistsKilledFieldNumber,
    GreatestPopulation    = StatsRequest.GreatestPopulationFieldNumber,
    InGameHours           = StatsRequest.InGameHoursFieldNumber,
}

public static class ColonyStatExtension
{
    public static object GetDbSet(this ColonyStat stat, RimionDbContext context)
    {
        return stat switch{
                   ColonyStat.DamageTakenPawns     => context.DamageTakenPawns,
                   ColonyStat.DamageTakenThings    => context.DamageTakenThings,
                   ColonyStat.DamageDealt          => context.DamageDealt,
                   ColonyStat.Colonists            => context.Colonists,
                   ColonyStat.Wealth               => context.Wealth,
                   ColonyStat.MapCount             => context.MapCount,
                   ColonyStat.ColonistsNeedTending => context.ColonistsNeedTending,
                   ColonyStat.MedicalConditions    => context.MedicalConditions,
                   ColonyStat.Enemies              => context.Enemies,
                   ColonyStat.WildAnimals          => context.WildAnimals,
                   ColonyStat.TamedAnimals         => context.TamedAnimals,
                   ColonyStat.Visitors             => context.Visitors,
                   ColonyStat.Prisoners            => context.Prisoners,
                   ColonyStat.DownedColonists      => context.DownedColonists,
                   ColonyStat.MentalColonists      => context.MentalColonists,
                   ColonyStat.Rooms                => context.Rooms,
                   ColonyStat.Caravans             => context.Caravans,
                   ColonyStat.WeaponDps            => context.WeaponDps,
                   ColonyStat.Electricity          => context.Electricity,
                   ColonyStat.Medicine             => context.Medicine,
                   ColonyStat.Food                 => context.Food,
                   ColonyStat.Fire                 => context.Fire,
                   ColonyStat.Conditions           => context.Conditions,
                   ColonyStat.Temperature          => context.Temperature,
                   ColonyStat.NumRaidsEnemy        => context.NumRaidsEnemy,
                   ColonyStat.NumThreatBigs        => context.NumThreatBigs,
                   ColonyStat.ColonistsKilled      => context.ColonistsKilled,
                   ColonyStat.GreatestPopulation   => context.GreatestPopulation,
                   ColonyStat.InGameHours          => context.InGameHours,
                   _                               => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
               };
    }
    public static Type GetDbType(this ColonyStat stat)
    {
        return stat switch{
                   ColonyStat.DamageTakenPawns     => typeof(DamageTakenPawns),
                   ColonyStat.DamageTakenThings    => typeof(DamageTakenThings),
                   ColonyStat.DamageDealt          => typeof(DamageDealt),
                   ColonyStat.Colonists            => typeof(Colonists),
                   ColonyStat.Wealth               => typeof(Wealth),
                   ColonyStat.MapCount             => typeof(MapCount),
                   ColonyStat.ColonistsNeedTending => typeof(ColonistsNeedTending),
                   ColonyStat.MedicalConditions    => typeof(MedicalConditions),
                   ColonyStat.Enemies              => typeof(Enemies),
                   ColonyStat.WildAnimals          => typeof(WildAnimals),
                   ColonyStat.TamedAnimals         => typeof(TamedAnimals),
                   ColonyStat.Visitors             => typeof(Visitors),
                   ColonyStat.Prisoners            => typeof(Prisoners),
                   ColonyStat.DownedColonists      => typeof(DownedColonists),
                   ColonyStat.MentalColonists      => typeof(MentalColonists),
                   ColonyStat.Rooms                => typeof(Rooms),
                   ColonyStat.Caravans             => typeof(Caravans),
                   ColonyStat.WeaponDps            => typeof(WeaponDps),
                   ColonyStat.Electricity          => typeof(Electricity),
                   ColonyStat.Medicine             => typeof(Medicine),
                   ColonyStat.Food                 => typeof(Food),
                   ColonyStat.Fire                 => typeof(Fire),
                   ColonyStat.Conditions           => typeof(Conditions),
                   ColonyStat.Temperature          => typeof(Temperature),
                   ColonyStat.NumRaidsEnemy        => typeof(NumRaidsEnemy),
                   ColonyStat.NumThreatBigs        => typeof(NumThreatBigs),
                   ColonyStat.ColonistsKilled      => typeof(ColonistsKilled),
                   ColonyStat.GreatestPopulation   => typeof(GreatestPopulation),
                   ColonyStat.InGameHours          => typeof(InGameHours),
                   _                               => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
               };
    }
}