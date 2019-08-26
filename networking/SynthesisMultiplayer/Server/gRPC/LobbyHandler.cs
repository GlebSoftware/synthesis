﻿using System.Net.Sockets;
using MatchmakingService;
using System.Collections.Generic;
using System;
using Grpc;
using System.Threading.Tasks;
using SynthesisMultiplayer.Server.UDP;
using SynthesisMultiplayer.Threading.Execution;
using static SynthesisMultiplayer.Threading.Execution.ManagedTaskHelper;
using Grpc.Core;
using System.Threading;
using SynthesisMultiplayer.Common;
using SynthesisMultiplayer.Attribute;
using System.Net;
using SynthesisMultiplayer.Util;

namespace SynthesisMultiplayer.Common
{
    public partial class Methods
    {
        public class LobbyHandler
        {
            public const string GetLatestConnection = "GET_LATEST_CONNECTION";
        }
    }
}

namespace SynthesisMultiplayer.Server.gRPC
{
    public class LobbyHandler : IManagedTask, IServer
    {
        private class Job
        {
            public enum Status
            {
                Started,
                Running,
                Complete,
                Errored
            }
            public Guid JobId;
            public Status JobStatus;
            public IPEndPoint Ip;
        }

        public bool Initialized { get; private set; }
        public bool Alive { get; private set; }
        public Guid Id { get; private set; }
        public ManagedTaskStatus Status { get; private set; }
        protected Grpc.Core.Server Server;
        protected Guid ListenerPid;
        protected int Port;
        protected Channel<Guid> CompletedJobs;
        public LobbyHandler(Guid listenerPid, int port = 33002)
        {
            ListenerPid = listenerPid;
            Port = port;
            CompletedJobs = new Channel<Guid>();
        }

        private class LobbyGrpcServer : ServerHost.ServerHostBase
        {
            Guid ListenerPid;
            Dictionary<Guid, Job> Jobs;
            Channel<Guid> CompletedJobs;
            public LobbyGrpcServer(Guid listener, Channel<Guid> completedJobs)
            {
                ListenerPid = listener;
                Jobs = new Dictionary<Guid, Job>();
                CompletedJobs = completedJobs;
            }

            public override Task<JoinLobbyResponse> JoinLobby(JoinLobbyRequest req, ServerCallContext context)
            {
                var job = new Job
                {
                    JobId = Guid.NewGuid(),
                    JobStatus = Job.Status.Started,
                    Ip = new IPEndPoint(IPAddress.Parse(context.Host.Split(':')[0]), int.Parse(context.Host.Split(':')[1]))
                };
                Jobs.Add(job.JobId, job);
                Console.WriteLine("New join request");
                return Task.FromResult(new JoinLobbyResponse
                {
                    Api = "v1",
                    JobId = job.JobId.ToString()
                });
            }

            public override async Task JoinLobbyStatus(IAsyncStreamReader<JoinLobbyStatusRequest> requestStream, IServerStreamWriter<JoinLobbyStatusResponse> responseStream, ServerCallContext context)
            {
                while (await requestStream.MoveNext())
                {
                    var listener = (ConnectionListener)GetTask(ListenerPid);
                    var connInfo = listener.GetConnectionInfo(new Guid(requestStream.Current.JobId));
                    if (connInfo != null)
                    {
                        if (connInfo.Address != null)
                        {
                            await responseStream.WriteAsync(new JoinLobbyStatusResponse
                            {
                                Api = "v1",
                                Status = JoinLobbyStatusResponse.Types.Status.Connected
                            });
                            CompletedJobs.Send(new Guid(requestStream.Current.JobId));
                        }
                    }
                    else
                    {
                        await responseStream.WriteAsync(new JoinLobbyStatusResponse
                        {
                            Api = "v1",
                            Status = JoinLobbyStatusResponse.Types.Status.Connecting
                        });
                        Thread.Sleep(50);
                    }
                }
            }
        }

        [Callback(methodName: Methods.LobbyHandler.GetLatestConnection)]
        public void GetLatestConnectionCallback(ITaskContext context, AsyncCallHandle handle) =>
            handle.Result = CompletedJobs.TryGet();
        [Callback(methodName: Methods.Server.Serve)]
        public void ServeCallback(ITaskContext context, AsyncCallHandle handle)
        {

            Server = new Grpc.Core.Server
            {
                Services = { ServerHost.BindService(new LobbyGrpcServer(ListenerPid, CompletedJobs)) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            Server.Start();
            Console.WriteLine("Serving Grpc");
            handle.Done();
        }

        [Callback(methodName: Methods.Server.Restart)]
        public void RestartCallback(ITaskContext context, AsyncCallHandle handle)
        {
            throw new NotImplementedException();
        }

        [Callback(methodName: Methods.Server.Shutdown)]
        public void ShutdownCallback(ITaskContext context, AsyncCallHandle handle)
        {
            Server.KillAsync().Wait();
            Server = null;
            handle.Done();
        }
        public void Initialize(Guid taskId)
        {
            Id = taskId;
            Status = ManagedTaskStatus.Initialized;
            Initialized = true;
            Alive = true;
        }

        public void Terminate(string reason = null, params dynamic[] args)
        {
            this.Call(Methods.Server.Shutdown);
        }

        public void Loop() { }

        public void Dispose()
        {

        }

    }
}
