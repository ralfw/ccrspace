using System;
using CcrSpaces.Core;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    public class CcrsPendingRequest<TInput, TOutput>
    {
        internal PortSet<TInput, CcrsRequest<TInput, TOutput>> Requests;
        internal TInput Request;


        public void Receive(Action<TOutput> responseHandler)
        {
            this.Receive(new ChannelFactory().CreateChannel(new CcrsChannelConfig<TOutput> {MessageHandler = responseHandler}));
        }

        public void Receive(Action<TOutput> responseHandler, CcrsChannelHandlerModes handlerMode)
        {
            this.Receive(new ChannelFactory().CreateChannel(new CcrsChannelConfig<TOutput> { MessageHandler = responseHandler, HandlerMode=handlerMode }));
        }

        public void Receive(Port<TOutput> responsePort)
        {
            this.Requests.Post(new CcrsRequest<TInput, TOutput>(this.Request, responsePort));
        }
    }


    public static class CcrsChannelFactoryExtensions
    {
        #region one way channel
        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler)
        {
            return CreateChannel(space, messageHandler, CcrsChannelHandlerModes.Sequential);
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler, CcrsChannelHandlerModes handlerMode)
        {
            return CreateChannel(space, new CcrsChannelConfig<T>
                                            {
                                                MessageHandler = messageHandler,
                                                HandlerMode = handlerMode
                                            });
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, CcrsChannelConfig<T> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return ChannelFactory.Instance.CreateChannel(config);
        }
        #endregion


        #region request/response channel
        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> requestHandler)
        {
            return CreateChannel(space, new CcrsChannelConfig<TInput, TOutput>
                                             {
                                                 InputMessageHandler = (input, outputPort) => outputPort.Post(requestHandler(input))
                                             });
        }

        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> requestHandler, Action<TOutput> responseHandler)
        {
            return CreateChannel(space, new CcrsChannelConfig<TInput, TOutput>
                                            {
                                                InputMessageHandler = (input, outputPort) => outputPort.Post(requestHandler(input)),
                                                OutputMessageHandler = responseHandler

                                            });
        }

        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> requestHandler, Action<TOutput> responseHandler)
        {
            return CreateChannel(space, new CcrsChannelConfig<TInput, TOutput>
                                            {
                                                InputMessageHandler = requestHandler,
                                                OutputMessageHandler = responseHandler
                                            });
        }

        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> requestHandler)
        {
            return CreateChannel(space, new CcrsChannelConfig<TInput, TOutput>
                                             {
                                                 InputMessageHandler = requestHandler
                                             });
        }

        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, CcrsChannelConfig<TInput, TOutput> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return ChannelFactory.Instance.CreateChannel(config);
        }
        #endregion


        public static CcrsPendingRequest<TInput, TOutput> Request<TInput, TOutput>(this PortSet<TInput, CcrsRequest<TInput, TOutput>> ports, TInput request)
        {
            return new CcrsPendingRequest<TInput, TOutput>
                       {
                            Requests = ports,
                            Request = request
                       };
        }
    }
}
