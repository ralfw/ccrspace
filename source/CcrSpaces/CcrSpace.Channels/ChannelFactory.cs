using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    [Serializable]
    public class CcrsRequest<TInput, TOutput>
    {
        internal CcrsRequest(TInput request, Port<TOutput> responses)
        {
            this.Request = request;
            this.Responses = responses;
        }

        public TInput Request;
        public Port<TOutput> Responses;
    }

    
    internal interface IChannelFactory
    {
        Port<T> CreateChannel<T>(CcrsChannelConfig<T> config);
        PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(CcrsChannelConfig<TInput, TOutput> config);
    }


    internal partial class ChannelFactory : IChannelFactory
    {
        private static IChannelFactory instance;
        public static IChannelFactory Instance
        {
            get
            {
                if (instance == null) instance = new ChannelFactory();
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
                ConfigurePort(port, config);
            }
            return port;
        }


        public PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(CcrsChannelConfig<TInput, TOutput> config)
        {
            var reqRespPort = new PortSet<TInput, CcrsRequest<TInput, TOutput>>();
            {
                Port<TOutput> responses = new Port<TOutput>();

                ConfigurePort(responses, new CcrsChannelConfig<TOutput>
                                                {
                                                    MessageHandler = config.OutputMessageHandler ?? (x=>{}),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.OutputHandlerMode
                                                });

                ConfigurePort(reqRespPort.P0, new CcrsChannelConfig<TInput>
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


                ConfigurePort(reqRespPort.P1, new CcrsChannelConfig<CcrsRequest<TInput, TOutput>>
                                                {
                                                    MessageHandler = req => config.InputMessageHandler(req.Request, req.Responses),
                                                    TaskQueue = config.TaskQueue,
                                                    HandlerMode = config.InputHandlerMode
                                                });
            }
            return reqRespPort;
        }
    }
}
