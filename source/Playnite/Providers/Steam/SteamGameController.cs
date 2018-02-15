using NLog;
using Playnite.Database;
using Playnite.Models;
using Polly;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.Providers.Steam
{
    public class SteamGameController : IGameController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler Uninstalled;
        public event EventHandler Installed;

        SteamGameController(GameDatabase database)
        {

        }

        public void ActivateAction(GameTask action)
        {

        }

        public void Install()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Uninstall()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
