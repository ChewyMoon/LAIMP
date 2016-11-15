namespace DataCollector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

    class Program
    {
        #region Properties

        /// <summary>
        /// Gets or sets the queue.
        /// </summary>
        /// <value>
        /// The queue.
        /// </value>
        private static Queue<Data> Queue { get; set; } = new Queue<Data>();

        #endregion

        #region Methods

        /// <summary>
        ///     The main entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        /// <summary>
        ///     Raises the <see cref="E:GameLoad" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameLoad(EventArgs args)
        {
            Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     Called when an <see cref="Obj_AI_Base" /> issues an order.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectIssueOrderEventArgs" /> instance containing the event data.</param>
        private static void OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != GameObjectType.obj_AI_Hero)
            {
                return;
            }

            Queue.Enqueue(Data.CreateFromHero((Obj_AI_Hero)sender));
        }

        /// <summary>
        ///     Processes the queue.
        /// </summary>
        private static async void ProcessQueue()
        {
            while (!AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                while (Queue.Count > 1)
                {
                    var data = Queue.Dequeue();

                    // todo process data
                }
            }
        }

        #endregion
    }

    internal class Data
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
        ///     Gets or sets the under turret.
        /// </summary>
        /// <value>
        ///     The under turret.
        /// </value>
        public double UnderTurret { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a <see cref="Data" /> object from an <see cref="Obj_AI_Base" />.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public static Data CreateFromHero(Obj_AI_Hero hero)
        {
            return new Data()
                       {
                           PositionX = hero.ServerPosition.X, PositionY = hero.ServerPosition.Y,
                           DirectionX = hero.Direction.X, DirectionY = hero.Direction.Y,
                           HealthPercent = hero.HealthPercent,
                           DistanceToClosestAlly =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team == hero.Team)
                                   .Select(x => x.Distance(hero))
                                   .Min(),
                           AlliesInRange =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team == hero.Team)
                                   .Count(x => hero.Distance(x) <= 1000) - 1,
                           EnemiesInRange =
                               ObjectManager.Get<Obj_AI_Hero>()
                                   .Where(x => x.Team != hero.Team)
                                   .Count(x => hero.Distance(x) <= 1000) - 1,
                           Level = hero.Level,
                           Experience = hero.Experience, CanMove = hero.CanMove ? 1 : 0,
                           CanAttack = hero.CanAttack ? 1 : 0, Recalling = hero.IsRecalling() ? 1 : 0,
                           UnderTurret = hero.UnderTurret(true) ? 1 : 0, ManaPercent = hero.ManaPercent
                       };
        }

        #endregion
    }
}