using MonoPatcherLib;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassPatch
{
    [Plugin]
    public class Main
    {
        public Main()
        {
            MonoPatcher.PatchAll();
            World.sOnStartupAppEventHandler += OnStartupApp;
        }

        private static void OnStartupApp(object sender, EventArgs e)
        {
            CommandSystem.RegisterCommand("classpatchworks", "Class patch works!", (object[] args) =>
            {
                SimpleMessageDialog.Show("MonoPatcher", $"Version: {MonoPatcher.Version}\nInitialization Type: {MonoPatcher.InitializationType}");
                return 1;
            });
        }
    }
}
