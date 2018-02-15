using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.Providers
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

    public class GameControllerFactory : IDisposable
    {
        private List<IGameController> controllers;

        public GameControllerFactory()
        {
            controllers = new List<IGameController>();
        }

        public void AddController(IGameController monitor)
        {

        }

        public void Dispose()
        {
            foreach (var controller in controllers)
            {
                //controller.StopMonitoring();
                //controller.Dispose();
            }
        }
    }
}
