namespace PlayFOnline.UI.View.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FOQuery.Data;
    using PlayFOnline.Core;

    /// <summary>
    /// WIP.
    /// </summary>
    public class ConsoleMainView : IMainView
    {
        public event EventHandler RefreshServers;
        public event EventHandler FoDevLinkClicked;
        public event EventHandler<FOGameInfo> InstallGame;
        public event EventHandler<FOGameInfo> ChangedGame;
        public event ItemEventHandler<string> LaunchProgram;
        public event ItemEventHandler<bool> ShowOfflineChanged;
        public event ItemEventHandler<bool> PingChanged;

        public bool AskYesNoQuestion(string question, string title)
        {
            throw new NotImplementedException();
        }

        public string GetFolderPath()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string errorMsg)
        {
            Console.WriteLine("Error: " + errorMsg);
        }

        public void ShowInfo(string infoMsg)
        {
            Console.WriteLine(infoMsg);
        }

        static void PrintLine()
        {
            Console.WriteLine(new string('-', 77));
        }

        void PrintRow(params string[] columns)
        {
            int width = (77 - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }
            Console.WriteLine(row);
        }

        string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public void RefreshServerList(List<FOGameInfo> servers, bool pingServers)
        {
            //Console.Clear();
            PrintRow("", "", "", "");
            PrintRow("Name", "Players", "Host", "Port");
            PrintRow("", "", "", "");
            foreach (var server in servers)
            {
                PrintRow(server.Name, server.Status.PlayersStr, server.Host, server.Port.ToString());
            }
        }

        public void UpdateStatusBar(string text)
        {
            Console.WriteLine("Status: " + text);
        }

        public UISettings GetWindowProperties()
        {
            return null;
        }

        public void ClearGameSelection()
        {

        }

        public void AddInstallButton()
        {
            //throw new NotImplementedException();
        }

        public void SelectGame(FOGameInfo game, List<FOGameLaunchProgram> programs)
        {
            //throw new NotImplementedException();
        }

        public void SetWindowProperties(UISettings settings)
        {
            //throw new NotImplementedException();
        }

        public void SetTitle(string title)
        {
            //throw new NotImplementedException();
            Console.Title = title;
        }

        public void Load()
        {
            //throw new NotImplementedException();
        }

        private void ShowHelp()
        {
            Console.WriteLine("clear - Clear console buffer.");
            Console.WriteLine("show-offline <true|false> - Toggles display mode.");
            Console.WriteLine("update - Updates status of servers, such as uptime and number of players online.");
        }

        private void ProcessCommand(string command)
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Gray;
            string[] parts = command.Split("".ToArray());

            switch (parts[0])
            {
                case "clear":
                    Console.Clear();
                    break;
                case "help":
                    ShowHelp();
                    break;
                case "show-offline":
                    bool arg = (parts[1] == "true");
                    ShowOfflineChanged(null, new ItemEventArgs<bool>(arg));
                    break;
                case "update":
                    RefreshServers(null, null);
                    break;
            }
        }

        public void StartApplication()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Play-FOnline! Type help to get a list of commands.");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">");
                ProcessCommand(Console.ReadLine());
            }
        }
    }
}
