using Playnite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite.Providers
{
    public interface IGameController : IDisposable
    {
        void Install();

        void Uninstall();

        void Play();

        void ActivateAction(GameTask action);

        event EventHandler Started;

        event EventHandler Stopped;

        event EventHandler Uninstalled;

        event EventHandler Installed;
    }
}
