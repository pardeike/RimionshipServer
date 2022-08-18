using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Data.Detailed;
namespace RimionshipServer.Services;

public class GameDataService
{
    private RimionDbContext Context { get; }

    private const int PollingRate = 10;

    public GameDataService(RimionDbContext context)
    {
        Context = context;
    }

    private static readonly ReaderWriterLockSlim             _lock = new ();
    private static          Dictionary<string, StatsRequest> LastRequests     { get; } = new ();
    private static          Dictionary<string, DateTime>     LastRequestTimes { get; } = new ();
    
    private static readonly MethodInfo                       _getIntDataForTimeSpanMethodInfo                    = typeof(GameDataService).GetMethod(nameof(GetIntDataForTimeSpanGeneric))!;
    private static readonly MethodInfo                       _getFloatDataForTimeSpanMethodInfo                  = typeof(GameDataService).GetMethod(nameof(GetFloatDataForTimeSpanGeneric))!;
    private static readonly MethodInfo                       _getIntDataForTimeSpanAndPlayersGenericMethodInfo   = typeof(GameDataService).GetMethod(nameof(GetIntDataForTimeSpanAndPlayersGeneric))!;
    private static readonly MethodInfo                       _getFloatDataForTimeSpanAndPlayersGenericMethodInfo = typeof(GameDataService).GetMethod(nameof(GetFloatDataForTimeSpanAndPlayersGeneric))!;
    
    private static int RoundToNearestMultipleOfFactor(int value, int factor)
    {
        if (factor == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), factor, "Cannot be zero");
        }

        var halfAbsFactor = Math.Abs(factor) >> 1;
        return value + Math.Sign(value) * (halfAbsFactor - (Math.Abs(value) % factor + halfAbsFactor % factor) % factor);
    }

    private DateTime GetTimeAdjustedNow(string id)
    {
        DateTime lastRequestTime;
        bool     gotLastTime;
        try
        {
            _lock.EnterReadLock();
            gotLastTime = LastRequestTimes.TryGetValue(id, out lastRequestTime);
        }
        finally
        {
            if (_lock.IsReadLockHeld)
                _lock.ExitReadLock();
        }
        var now = DateTime.Now;
        if (gotLastTime)
        {
            var secondsDiff         = (now - lastRequestTime).Seconds;
            var secondsDiffAdjusted = RoundToNearestMultipleOfFactor(secondsDiff, PollingRate);
            if (secondsDiffAdjusted == secondsDiff)
            {
                secondsDiffAdjusted += PollingRate;
            }
            var nowToUse = now
                          .Subtract(TimeSpan.FromSeconds(now.Second))
                          .AddSeconds(secondsDiffAdjusted)
                          .Subtract(TimeSpan.FromMilliseconds(now.Millisecond));
            return nowToUse;
        }
        else
        {
            var secondsDiff         = now.Second;
            var secondsDiffAdjusted = RoundToNearestMultipleOfFactor(secondsDiff, PollingRate);
            lastRequestTime = now
                             .Subtract(TimeSpan.FromSeconds(now.Second))
                             .AddSeconds(secondsDiffAdjusted)
                             .Subtract(TimeSpan.FromMilliseconds(now.Millisecond));
            try
            {
                _lock.EnterWriteLock();
                LastRequestTimes[id] = lastRequestTime;
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
            return lastRequestTime;
        }
    }

    public async Task ProcessStatsRequestAsync(StatsRequest request, CancellationToken cancellationToken)
    {
        StatsRequest? lastRequest;
        try
        {
            _lock.EnterReadLock();
            LastRequests.TryGetValue(request.Id, out lastRequest);
        }
        finally
        {
            if (_lock.IsReadLockHeld)
                _lock.ExitReadLock();
        }

        var binaryNow = GetTimeAdjustedNow(request.Id);

        //Datapoint
        var baseEntry = Context.BaseEntry.Add(new BaseEntry(request.Id, binaryNow)).Entity;

#region ActualTransaction
        if (lastRequest is null || request.Caravans != lastRequest.Caravans)
            Context.Caravans.Add(new Caravans(baseEntry, request.Caravans));

        if (lastRequest is null || request.Colonists != lastRequest.Colonists)
            Context.Colonists.Add(new Colonists(baseEntry, request.Colonists));

        if (lastRequest is null || request.Conditions != lastRequest.Conditions)
            Context.Conditions.Add(new Conditions(baseEntry, request.Conditions));

        if (lastRequest is null || request.Electricity != lastRequest.Electricity)
            Context.Electricity.Add(new Electricity(baseEntry, request.Electricity));

        if (lastRequest is null || request.Enemies != lastRequest.Enemies)
            Context.Enemies.Add(new Enemies(baseEntry, request.Enemies));

        if (lastRequest is null || request.Fire != lastRequest.Fire)
            Context.Fire.Add(new Fire(baseEntry, request.Fire));

        if (lastRequest is null || request.Food != lastRequest.Food)
            Context.Food.Add(new Food(baseEntry, request.Food));

        if (lastRequest is null || request.Medicine != lastRequest.Medicine)
            Context.Medicine.Add(new Medicine(baseEntry, request.Medicine));

        if (lastRequest is null || request.Prisoners != lastRequest.Prisoners)
            Context.Prisoners.Add(new Prisoners(baseEntry, request.Prisoners));

        if (lastRequest is null || request.Rooms != lastRequest.Rooms)
            Context.Rooms.Add(new Rooms(baseEntry, request.Rooms));

        if (lastRequest is null || request.Temperature != lastRequest.Temperature)
            Context.Temperature.Add(new Temperature(baseEntry, request.Temperature));

        if (lastRequest is null || request.Visitors != lastRequest.Visitors)
            Context.Visitors.Add(new Visitors(baseEntry, request.Visitors));

        if (lastRequest is null || request.Wealth != lastRequest.Wealth)
            Context.Wealth.Add(new Wealth(baseEntry, request.Wealth));

        if (lastRequest is null || request.ColonistsKilled != lastRequest.ColonistsKilled)
            Context.ColonistsKilled.Add(new ColonistsKilled(baseEntry, request.ColonistsKilled));

        if (lastRequest is null || Math.Abs(request.DamageDealt - lastRequest.DamageDealt) > 0.01f)
            Context.DamageDealt.Add(new DamageDealt(baseEntry, request.DamageDealt));

        if (lastRequest is null || request.DownedColonists != lastRequest.DownedColonists)
            Context.DownedColonists.Add(new DownedColonists(baseEntry, request.DownedColonists));

        if (lastRequest is null || request.GreatestPopulation != lastRequest.GreatestPopulation)
            Context.GreatestPopulation.Add(new GreatestPopulation(baseEntry, request.GreatestPopulation));

        if (lastRequest is null || request.MapCount != lastRequest.MapCount)
            Context.MapCount.Add(new MapCount(baseEntry, request.MapCount));

        if (lastRequest is null || request.MedicalConditions != lastRequest.MedicalConditions)
            Context.MedicalConditions.Add(new MedicalConditions(baseEntry, request.MedicalConditions));

        if (lastRequest is null || request.MentalColonists != lastRequest.MentalColonists)
            Context.MentalColonists.Add(new MentalColonists(baseEntry, request.MentalColonists));

        if (lastRequest is null || request.TamedAnimals != lastRequest.TamedAnimals)
            Context.TamedAnimals.Add(new TamedAnimals(baseEntry, request.TamedAnimals));

        if (lastRequest is null || request.WeaponDps != lastRequest.WeaponDps)
            Context.WeaponDps.Add(new WeaponDps(baseEntry, request.WeaponDps));

        if (lastRequest is null || request.WildAnimals != lastRequest.WildAnimals)
            Context.WildAnimals.Add(new WildAnimals(baseEntry, request.WildAnimals));

        if (lastRequest is null || request.ColonistsNeedTending != lastRequest.ColonistsNeedTending)
            Context.ColonistsNeedTending.Add(new ColonistsNeedTending(baseEntry, request.ColonistsNeedTending));

        if (lastRequest is null || Math.Abs(request.DamageTakenPawns - lastRequest.DamageTakenPawns) > 0.01f)
            Context.DamageTakenPawns.Add(new DamageTakenPawns(baseEntry, request.DamageTakenPawns));

        if (lastRequest is null || Math.Abs(request.DamageTakenThings - lastRequest.DamageTakenThings) > 0.01f)
            Context.DamageTakenThings.Add(new DamageTakenThings(baseEntry, request.DamageTakenThings));

        if (lastRequest is null || request.InGameHours != lastRequest.InGameHours)
            Context.InGameHours.Add(new InGameHours(baseEntry, request.InGameHours));

        if (lastRequest is null || request.NumRaidsEnemy != lastRequest.NumRaidsEnemy)
            Context.NumRaidsEnemy.Add(new NumRaidsEnemy(baseEntry, request.NumRaidsEnemy));

        if (lastRequest is null || request.NumThreatBigs != lastRequest.NumThreatBigs)
            Context.NumThreatBigs.Add(new NumThreatBigs(baseEntry, request.NumThreatBigs));
#endregion

        try
        {
            _lock.EnterWriteLock();
            LastRequests[request.Id] = request;
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
                _lock.ExitWriteLock();
        }
        await Context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<DateTime> GetTimePointsForTimeSpan(DateTime begin, DateTime end, TimeSpan stepsize)
    {
        while (begin <= end)
            yield return begin += stepsize;
    }

    private async Task<IEnumerable<DateTime>> GetTimePointsForUser(string playerId, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
    {
        /*
         * SQLite does not support Dates or Times. -_-
         */
        var  ticks = start.Ticks;
        long dbTicks;
        try
        {
            dbTicks = await Context.BaseEntry
                                   .AsNoTracking()
                                   .Where(x => x.UId == playerId)
                                   .Select(x => x.TimeTicks)
                                   .OrderBy(x => ticks > x ? ticks - x : x - ticks)
                                   .FirstAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return ArraySegment<DateTime>.Empty;
        }
        var timing = new DateTime(dbTicks);
        return GetTimePointsForTimeSpan(timing, end, stepsize);
    }

    public Task<ILookup<DateTime, int>> GetIntDataForTimeSpan(string playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
    {
        var type = stat.GetDbType();
        return (Task<ILookup<DateTime, int>>) _getIntDataForTimeSpanMethodInfo
                                             .MakeGenericMethod(type)
                                             .Invoke(this, new object[]{playerId, stat, start, end, stepsize, cancellationToken})!;
    }

    public Task<ILookup<DateTime, float>> GetFloatDataForTimeSpan(string playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
    {
        var type = stat.GetDbType();
        return (Task<ILookup<DateTime, float>>) _getFloatDataForTimeSpanMethodInfo
                                               .MakeGenericMethod(type)
                                               .Invoke(this, new object[]{playerId, stat, start, end, stepsize, cancellationToken})!;
    }


    public async Task<ILookup<DateTime, int>> GetIntDataForTimeSpanGeneric<T>(string playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
        where T : BaseIntRecord
    {
        var set = (DbSet<T>) stat.GetDbSet(Context);
        var timePoints = (await GetTimePointsForUser(playerId, start, end, stepsize, cancellationToken))
           .Select(x => x.Ticks);

        return (await set.AsNoTrackingWithIdentityResolution()
                         .Include(x => x.Id)
                         .Where(x => x.Id.UId == playerId)
                         .Where(x => timePoints.Contains(x.Id.TimeTicks))
                         .Select(x => new{x.Id.Time, x.Value})
                         .ToListAsync(cancellationToken))
           .ToLookup(x => x.Time, x => x.Value);
    }

    public async Task<ILookup<DateTime, float>> GetFloatDataForTimeSpanGeneric<T>(string playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
        where T : BaseFloatRecord
    {
        var set = (DbSet<T>) stat.GetDbSet(Context);
        var timePoints = (await GetTimePointsForUser(playerId, start, end, stepsize, cancellationToken))
           .Select(x => x.Ticks);

        return (await set.AsNoTrackingWithIdentityResolution()
                         .Include(x => x.Id)
                         .Where(x => x.Id.UId == playerId)
                         .Where(x => timePoints.Contains(x.Id.TimeTicks))
                         .Select(x => new{x.Id.Time, x.Value})
                         .ToListAsync(cancellationToken))
           .ToLookup(x => x.Time, x => x.Value);
    }

    public record TimeIntValue(DateTime Time, int Value)
    {
        public override string ToString()
        {
            return $"{{ Time = {Time}, Value = {Value} }}";
        }
    }

    public record TimeFloatValue(DateTime Time, float Value)
    {
        public override string ToString()
        {
            return $"{{ Time = {Time}, Value = {Value} }}";
        }
    }
    
    public Task<ILookup<string, TimeIntValue>> GetIntDataForTimeSpanAndPlayers(ICollection<string> playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
    {
        var type = stat.GetDbType();
        return (Task<ILookup<string, TimeIntValue>>) _getIntDataForTimeSpanAndPlayersGenericMethodInfo
                                                    .MakeGenericMethod(type)
                                                    .Invoke(this, new object[]{playerId, stat, start, end, stepsize, cancellationToken})!;
    }

    public Task<ILookup<string, TimeFloatValue>> GetFloatDataForTimeSpanAndPlayers(ICollection<string> playerId, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize, CancellationToken cancellationToken)
    {
        var type = stat.GetDbType();
        return (Task<ILookup<string, TimeFloatValue>>) _getFloatDataForTimeSpanAndPlayersGenericMethodInfo
                                                      .MakeGenericMethod(type)
                                                      .Invoke(this, new object[]{playerId, stat, start, end, stepsize, cancellationToken})!;
    }

    public async Task<ILookup<string, TimeIntValue>> GetIntDataForTimeSpanAndPlayersGeneric<T>(ICollection<string> playerIds, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize,
                                                                                               CancellationToken   cancellationToken)
        where T : BaseIntRecord
    {
        var                   set        = (DbSet<T>) stat.GetDbSet(Context);
        IEnumerable<DateTime> timePoints = ArraySegment<DateTime>.Empty;
        foreach (var playerId in playerIds)
        {
            timePoints = timePoints.Concat(await GetTimePointsForUser(playerId, start, end, stepsize, cancellationToken));
        }

        var timePointTicks = timePoints.Select(x => x.Ticks);

        return (await set.AsNoTrackingWithIdentityResolution()
                         .Include(x => x.Id)
                         .Where(x => playerIds.Contains(x.Id.UId))
                         .Where(x => timePointTicks.Contains(x.Id.TimeTicks))
                         .Select(x => new{x.Id.UId, x.Id.Time, x.Value})
                         .ToListAsync(cancellationToken))
           .ToLookup(x => x.UId, x => new TimeIntValue(x.Time, x.Value));
    }

    public async Task<ILookup<string, TimeFloatValue>> GetFloatDataForTimeSpanAndPlayersGeneric<T>(ICollection<string> playerIds, ColonyStat stat, DateTime start, DateTime end, TimeSpan stepsize,
                                                                                                   CancellationToken   cancellationToken)
        where T : BaseFloatRecord
    {
        var                   set        = (DbSet<T>) stat.GetDbSet(Context);
        IEnumerable<DateTime> timePoints = ArraySegment<DateTime>.Empty;
        foreach (var playerId in playerIds)
        {
            timePoints = timePoints.Concat(await GetTimePointsForUser(playerId, start, end, stepsize, cancellationToken));
        }

        var timePointTicks = timePoints.Select(x => x.Ticks);

        return (await set.AsNoTrackingWithIdentityResolution()
                         .Include(x => x.Id)
                         .Where(x => playerIds.Contains(x.Id.UId))
                         .Where(x => timePointTicks.Contains(x.Id.TimeTicks))
                         .Select(x => new{x.Id.UId, x.Id.Time, x.Value})
                         .ToListAsync(cancellationToken))
           .ToLookup(x => x.UId, x => new TimeFloatValue(x.Time, x.Value));
    }
}