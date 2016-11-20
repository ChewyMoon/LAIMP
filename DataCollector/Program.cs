namespace DataCollector
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Newtonsoft.Json;

    class Program
    {
        #region Properties

        /// <summary>
        ///     Gets the json file path.
        /// </summary>
        /// <value>
        ///     The json file path.
        /// </value>
        private static string JsonFilePath => Path.Combine(Config.AppDataDirectory, "LAIMP", "Data.json");

        /// <summary>
        ///     Gets the json settings.
        /// </summary>
        /// <value>
        ///     The json settings.
        /// </value>
        private static JsonSerializerSettings JsonSettings
            => new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        /// <summary>
        ///     Gets or sets the queue.
        /// </summary>
        /// <value>
        ///     The queue.
        /// </value>
        private static Queue<Data> Queue { get; } = new Queue<Data>();

        #endregion

        #region Methods

        /// <summary>
        ///     The main entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            Directory.CreateDirectory(Path.Combine(Config.AppDataDirectory, "LAIMP"));
            if (!File.Exists(JsonFilePath))
            {
                File.Create(JsonFilePath).Dispose();
            }

            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        /// <summary>
        ///     Raises the <see cref="E:GameLoad" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameLoad(EventArgs args)
        {
            Obj_AI_Base.OnNewPath += OnNewPath;
            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Called when a hero has a new path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectNewPathEventArgs"/> instance containing the event data.</param>
        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (!(sender is Obj_AI_Hero))
            {
                return;
            }

            Queue.Enqueue(Data.CreateFromHero((Obj_AI_Hero)sender, args.Path[args.Path.Length - 1].To2D()));
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

                    // todo cache this list
                    var dataList = JsonConvert.DeserializeObject<List<Data>>(
                                       File.ReadAllText(JsonFilePath),
                                       JsonSettings) ?? new List<Data>();

                    dataList.Add(data);

                    File.WriteAllText(JsonFilePath, JsonConvert.SerializeObject(dataList, JsonSettings));

                    Console.WriteLine("Logged!");
                }
            }
        }

        #endregion
    }
}