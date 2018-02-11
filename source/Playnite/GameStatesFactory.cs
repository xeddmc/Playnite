using Playnite.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite
{
    public class GameState
    {
        public bool Installed
        {
            get; set;
        }

        public bool Running
        {
            get; set;
        }

        public bool Updating
        {
            get; set;
        }

        public bool Launching
        {
            get; set;
        }
    }

    public class GameStatesFactory : IDisposable
    {

        private List<IGameStateMonitor> monitors;

        public GameStatesFactory()
        {
            monitors = new List<IGameStateMonitor>();
        }

        public void AddGameMonitor(IGameStateMonitor monitor)
        {

        }

        public void Dispose()
        {
            foreach (var monitor in monitors)
            {
                monitor.StopMonitoring();
                monitor.Dispose();
            }
        }
    }
}
