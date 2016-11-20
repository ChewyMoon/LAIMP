namespace DataCollector
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Newtonsoft.Json;

    using SharpDX;

    class OtherHeroData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the allies in range.
        /// </summary>
        /// <value>
        ///     The allies in range.
        /// </value>
        public double AlliesInRange { get; set; }

        /// <summary>
        ///     Gets or sets the can attack.
        /// </summary>
        /// <value>
        ///     The can attack.
        /// </value>
        public double CanAttack { get; set; }

        /// <summary>
        ///     Gets or sets the can move.
        /// </summary>
        /// <value>
        ///     The can move.
        /// </value>
        public double CanMove { get; set; }

        /// <summary>
        ///     Gets or sets the direction x.
        /// </summary>
        /// <value>
        ///     The direction x.
        /// </value>
        public double DirectionX { get; set; }

        /// <summary>
        ///     Gets or sets the direction y.
        /// </summary>
        /// <value>
        ///     The direction y.
        /// </value>
        public double DirectionY { get; set; }

        /// <summary>
        ///     Gets or sets the distance to closest ally.
        /// </summary>
        /// <value>
        ///     The distance to closest ally.
        /// </value>
        public double DistanceToClosestAlly { get; set; }

        /// <summary>
        ///     Gets or sets the enemies in range.
        /// </summary>
        /// <value>
        ///     The enemies in range.
        /// </value>
        public double EnemiesInRange { get; set; }

        /// <summary>
        ///     Gets or sets the experience.
        /// </summary>
        /// <value>
        ///     The experience.
        /// </value>
        public double Experience { get; set; }

        /// <summary>
        ///     Gets or sets the health percent.
        /// </summary>
        /// <value>
        ///     The health percent.
        /// </value>
        public double HealthPercent { get; set; }

        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>
        ///     The level.
        /// </value>
        public double Level { get; set; }

        /// <summary>
        ///     Gets or sets the mana percent.
        /// </summary>
        /// <value>
        ///     The mana percent.
        /// </value>
        public double ManaPercent { get; set; }

        /// <summary>
        ///     Gets or sets the network identifier.
        /// </summary>
        /// <value>
        ///     The network identifier.
        /// </value>
        [JsonIgnore]
        public int NetworkId { get; set; }

        /// <summary>
        ///     Gets or sets the position x.
        /// </summary>
        /// <value>
        ///     The position x.
        /// </value>
        public double PositionX { get; set; }

        /// <summary>
        ///     Gets or sets the position y.
        /// </summary>
        /// <value>
        ///     The position y.
        /// </value>
        public double PositionY { get; set; }

        /// <summary>
        ///     Gets or sets the recalling.
        /// </summary>
        /// <value>
        ///     The recalling.
        /// </value>
        public double Recalling { get; set; }

        /// <summary>
        ///     Gets or sets the team.
        /// </summary>
        /// <value>
        ///     The team.
        /// </value>
        [JsonIgnore]
        public GameObjectTeam Team { get; set; }

        /// <summary>
        ///     Gets or sets the under turret.
        /// </summary>
        /// <value>
        ///     The under turret.
        /// </value>
        public double UnderTurret { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a <see cref="OtherHeroData" /> object from an <see cref="Obj_AI_Base" />.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static OtherHeroData CreateFromHero(Obj_AI_Hero hero, Vector2 position = new Vector2())
        {
            position = position.IsZero ? hero.Position.To2D() : position;

            return new OtherHeroData()
                       {
                           PositionX = position.X, PositionY = position.Y, DirectionX = hero.Direction.X,
                           DirectionY = hero.Direction.Y, HealthPercent = hero.HealthPercent,
                           DistanceToClosestAlly =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team == hero.Team && x.NetworkId != hero.NetworkId)
                                   .Select(x => x.Distance(hero))
                                   .Min(),
                           AlliesInRange =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team == hero.Team && x.NetworkId != hero.NetworkId)
                                   .Count(x => hero.Distance(x) <= 2000),
                           EnemiesInRange =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team != hero.Team)
                                   .Count(x => hero.Distance(x) <= 2000),
                           Level = hero.Level,
                           Experience = hero.Experience, CanMove = hero.CanMove ? 1 : 0,
                           CanAttack = hero.CanAttack ? 1 : 0, Recalling = hero.IsRecalling() ? 1 : 0,
                           UnderTurret = hero.UnderTurret(true) ? 1 : 0, ManaPercent = hero.ManaPercent,
                           Team = hero.Team, NetworkId = hero.NetworkId
                       };
        }

        #endregion
    }
}