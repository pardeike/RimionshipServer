namespace RimionshipServer.Data
{
    public class MiscSettings
    {
        public class BroadcastMessage
        {
            public int    Id   { get; set; }
            public string Text { get; set; }
        }
        
        public class Settings
        {
            public                          int        Id                 { get; set; }
            public                          string     Name               { get; set; }
            public                          Rising     Rising             { get; set; }
            public                          Punishment Punishment         { get; set; }
            public                          Traits     Traits             { get; set; }
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
    }
}