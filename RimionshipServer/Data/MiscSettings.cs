namespace RimionshipServer.Data
{
    public class MiscSettings
    {
        public class BroadcastMessage
        {
            public int    Id   { get; set; }
            public string Text { get; set; } = null!;
        }
        
        public class Settings
        {
            public                          int        Id                 { get; set; }
            public                          string     Name               { get; set; }  = null!;
            public                          Rising     Rising             { get; set; }  = null!;
            public                          Punishment Punishment         { get; set; }  = null!;
            public                          Traits     Traits             { get; set; }  = null!;
            public static implicit operator API.Settings(Settings settings) => new (){
                                                                                         Punishment = settings.Punishment,
                                                                                         Rising     = settings.Rising,
                                                                                         Traits     = settings.Traits,
                                                                                     };
        }
        
        public class Rising
        {
            public int Id                         { get; set; }
            public int MaxFreeColonistCount       { get; set; }
            public int RisingCooldown             { get; set; }
            public int RisingInterval             { get; set; }
            public int RisingIntervalMinimum      { get; set; }
            public int RisingReductionPerColonist { get; set; }
            public static implicit operator API.Rising(Rising rising) => new (){
                                                                                   MaxFreeColonistCount       = rising.MaxFreeColonistCount      ,
                                                                                   RisingCooldown             = rising.RisingCooldown            ,
                                                                                   RisingInterval             = rising.RisingInterval            ,
                                                                                   RisingIntervalMinimum      = rising.RisingIntervalMinimum     ,
                                                                                   RisingReductionPerColonist = rising.RisingReductionPerColonist,
                                                                               };
        }
        
        public class Punishment
        {
            public int   Id                 { get; set; }
            public int   FinalPauseInterval { get; set; }
            public float MaxThoughtFactor   { get; set; }
            public float MinThoughtFactor   { get; set; }
            public int   StartPauseInterval { get; set; }
            public static implicit operator API.Punishment(Punishment punishment) => new (){
                                                                                               FinalPauseInterval = punishment.FinalPauseInterval,
                                                                                               MaxThoughtFactor   = punishment.MaxThoughtFactor,
                                                                                               MinThoughtFactor   = punishment.MinThoughtFactor,
                                                                                               StartPauseInterval = punishment.StartPauseInterval,
                                                                                           };
        }
        
        public class Traits
        {
            public int   Id                   { get; set; }
            public float BadTraitSuppression  { get; set; }
            public float GoodTraitSuppression { get; set; }
            public float ScaleFactor          { get; set; }
            public int   MaxMeleeSkill        { get; set; }
            public int   MaxMeleeFlames       { get; set; }
            public int   MaxShootingFlames    { get; set; }
            public int   MaxShootingSkill     { get; set; }
            public static implicit operator API.Traits(Traits traits) => new (){
                                                                                   BadTraitSuppression  = traits.BadTraitSuppression ,
                                                                                   GoodTraitSuppression = traits.GoodTraitSuppression,
                                                                                   ScaleFactor          = traits.ScaleFactor         ,
                                                                                   MaxMeleeSkill        = traits.MaxMeleeSkill       ,
                                                                                   MaxMeleeFlames     = traits.MaxMeleeFlames   ,
                                                                                   MaxShootingFlames  = traits.MaxShootingFlames,
                                                                                   MaxShootingSkill   = traits.MaxShootingSkill 
                                                                               };
        }

        public class State
        {
            public int Id                 { get; set; }
            public int PlannedStartHour   { get; set; }
            public int PlannedStartMinute { get; set; }
            public int GameState          { get; set; }
            public static implicit operator API.State(State state) => new (){
                                                                                PlannedStartHour = state.PlannedStartHour,
                                                                                PlannedStartMinute = state.PlannedStartMinute,
                                                                                Game = (API.State.Types.Game) state.GameState
                                                                            };
        }
        public class SaveSettings
        {
            public int     Id             { get; set; }
            public string? SaveFile       { get; set; }
            public string  DownloadURI    { get; set; } = null!;
            public int     CountColonists { get; set; }
        }
    }
}