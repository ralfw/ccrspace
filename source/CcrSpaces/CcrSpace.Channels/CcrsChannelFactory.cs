using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{    
    public interface ICcrsChannelFactory
    {
        Port<T> CreateChannel<T>(CcrsOneWayChannelConfig<T> config);
        PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> CreateChannel<TInput, TOutput>(CcrsRequestResponseChannelConfig<TInput, TOutput> config);
        Port<TInput> CreateChannel<TInput, TOutput>(CcrsFilterChannelConfig<TInput, TOutput> config);
    }


    public partial class CcrsChannelFactory : ICcrsChannelFactory
    {
        private static ICcrsChannelFactory instance;
        public static ICcrsChannelFactory Instance
        {
            get
            {
                if (instance == null) instance = new CcrsChannelFactory();
                return instance;
            }
            set
            {
                instance = value;
            }
        }


        public Port<T> CreateChannel<T>(CcrsOneWayChannelConfig<T> config)
        {
            var port = new Port<T>();
            {
                ConfigureChannel(port, config);
            }
            return port;
        }


        public PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> CreateChannel<TInput, TOutput>(CcrsRequestResponseChannelConfig<TInput, TOutput> config)
        {
            var reqRespPort = new PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType>();
            {
                Port<TOutput> responses = new Port<TOutput>();

                ConfigureChannel(responses, new CcrsOneWayChannelConfig<TOutput>
                                                {
                                                    MessageHandler = config.OutputMessageHandler ?? (x=>{}),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.OutputHandlerMode
                                                });

                ConfigureChannel(reqRespPort.P0, new CcrsOneWayChannelConfig<TInput>
                                                {
                                                    MessageHandler = msg =>
                                                                         {
                                                                             if (config.OutputMessageHandler == null) 
                                                                                 throw new InvalidOperationException("Missing response handler! Posting to a request/response channel is only allowed, if a response handler has been registered upon creation!");
                                                                             config.InputMessageHandler(msg, responses);
                                                                         },
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });


                ConfigureChannel(reqRespPort.P1, new CcrsOneWayChannelConfig<CcrsRequest<TInput, TOutput>>
                                                {
                                                    MessageHandler = req => config.InputMessageHandler(req.Request, req.Responses),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });

                ConfigureChannel(reqRespPort.P2, new CcrsOneWayChannelConfig<CcrsRequestOfUnknownType>
                                                {
                                                    MessageHandler = req => config.InputMessageHandler((TInput)req.Request, (Port<TOutput>)req.Responses),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });
            }
            return reqRespPort;
        }


        public Port<TInput> CreateChannel<TInput, TOutput>(CcrsFilterChannelConfig<TInput, TOutput> config)
        {
            var port = new Port<TInput>();
            {
                ConfigureChannel(port, new CcrsOneWayChannelConfig<TInput>
                                                {
                                                    MessageHandler = msg => config.InputMessageHandler(msg, config.OutputPort),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });
            }
            return port;
        }


        public void ConfigureChannel<T>(Port<T> port, CcrsOneWayChannelConfig<T> config)
        {
            if (config.HandlerMode == CcrsChannelHandlerModes.Sequential || config.HandlerMode == CcrsChannelHandlerModes.InCurrentSyncContext)
            {
                Action<T> safeHandler = CreateInSyncContextHandler(config);
                CreateSequentialHandler(config, safeHandler, port);
            }
            else
                CreateParallelHandler(config, port);
        }
    }
}
