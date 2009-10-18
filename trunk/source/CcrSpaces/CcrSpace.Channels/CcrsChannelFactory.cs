using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{    
    public interface ICcrsChannelFactory
    {
        Port<T> CreateChannel<T>(CcrsChannelConfig<T> config);
        PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> CreateChannel<TInput, TOutput>(CcrsChannelConfig<TInput, TOutput> config);
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


        public Port<T> CreateChannel<T>(CcrsChannelConfig<T> config)
        {
            var port = new Port<T>();
            {
                ConfigureChannel(port, config);
            }
            return port;
        }


        public PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType> CreateChannel<TInput, TOutput>(CcrsChannelConfig<TInput, TOutput> config)
        {
            var reqRespPort = new PortSet<TInput, CcrsRequest<TInput, TOutput>, CcrsRequestOfUnknownType>();
            {
                Port<TOutput> responses = new Port<TOutput>();

                ConfigureChannel(responses, new CcrsChannelConfig<TOutput>
                                                {
                                                    MessageHandler = config.OutputMessageHandler ?? (x=>{}),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.OutputHandlerMode
                                                });

                ConfigureChannel(reqRespPort.P0, new CcrsChannelConfig<TInput>
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


                ConfigureChannel(reqRespPort.P1, new CcrsChannelConfig<CcrsRequest<TInput, TOutput>>
                                                {
                                                    MessageHandler = req => config.InputMessageHandler(req.Request, req.Responses),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });

                ConfigureChannel(reqRespPort.P2, new CcrsChannelConfig<CcrsRequestOfUnknownType>
                                                {
                                                    MessageHandler = req => config.InputMessageHandler((TInput)req.Request, (Port<TOutput>)req.Responses),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });
            }
            return reqRespPort;
        }


        public void ConfigureChannel<T>(Port<T> port, CcrsChannelConfig<T> config)
        {
            if (config.HandlerMode == CcrsChannelHandlerModes.Sequential || config.HandlerMode == CcrsChannelHandlerModes.InCurrentSyncContext)
            {
                Action<T> safeHandler = CreateInSynContextHandler(config);
                CreateSequentialHandler(config, safeHandler, port);
            }
            else
                CreateParallelHandler(config, port);
        }
    }
}
