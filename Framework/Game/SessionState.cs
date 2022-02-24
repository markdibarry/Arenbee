namespace Arenbee.Framework.Game
{
    public class SessionState
    {
        public int TotalGameTime { get; set; }
        public int CurrentGameTime { get; set; }
        public int TimesDamaged { get; set; }
        public int DamageDealt { get; set; }
        public int DamageRecieved { get; set; }
        public int TimesDied { get; set; }
        public int EnemiesDefeated { get; set; }
    }
}