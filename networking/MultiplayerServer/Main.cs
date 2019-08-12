﻿using System;
using SynthesisMultiplayer.Server.UDP;
using SynthesisMultiplayer.Threading;
using SynthesisMultiplayer.Common;
namespace MultiplayerServer
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var test1 = ManagedTaskHelper.Start(new ConnectionListener(), "listener");
            var test2 = ManagedTaskHelper.Start(new LobbyHostBroadcaster(), "broadcaster");
            ManagedTaskHelper.GetTask("listener").Call(Methods.Server.Serve);
            ManagedTaskHelper.GetTask("broadcaster").Call(Methods.Server.Serve);
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            { }
            ManagedTaskHelper.Restart(test1);
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            { }
            ManagedTaskHelper.GetTask(test1).Terminate();

            Console.WriteLine("Server Closing. Please wait...");
            int counter = 0;
            while (ManagedTaskHelper.GetTask(test1).Status != ManagedTaskStatus.Completed)
            {
                if(counter % 50 == 0)
                {
                    Console.Write(".");
                }
                ++counter;
            }
        }
    }
}
