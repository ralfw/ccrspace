﻿using System;
using CcrSpaces.Core.Channels;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Core
{
    public static class CcrSpaceExtensionsForChannels
    {
        #region one way channel
        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler)
        {
            return CreateChannel(space, messageHandler, CcrsHandlerModes.Sequential);
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, Action<T> messageHandler, CcrsHandlerModes handlerMode)
        {
            return CreateChannel(space, new CcrsOneWayChannelConfig<T>
                                            {
                                                MessageHandler = messageHandler,
                                                HandlerMode = handlerMode
                                            });
        }

        public static Port<T> CreateChannel<T>(this ICcrSpace space, CcrsOneWayChannelConfig<T> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return CcrsChannelFactory.Instance.CreateChannel(config);
        }
        #endregion


        #region request/response channel
        public static Port<CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> requestHandler)
        {
            return CreateChannel(space, new CcrsRequestResponseChannelConfig<TInput, TOutput>
                                             {
                                                 InputMessageHandler = (input, outputPort) => outputPort.Post(requestHandler(input))
                                             }).P1;
        }

        public static Port<TInput> CreateChannel<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> requestHandler, Action<TOutput> responseHandler)
        {
            return CreateChannel(space, new CcrsRequestResponseChannelConfig<TInput, TOutput>
                                            {
                                                InputMessageHandler = (input, outputPort) => outputPort.Post(requestHandler(input)),
                                                OutputMessageHandler = responseHandler
                                            }).P0;
        }

        public static Port<TInput> CreateChannel<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> requestHandler, Action<TOutput> responseHandler)
        {
            return CreateChannel(space, new CcrsRequestResponseChannelConfig<TInput, TOutput>
                                            {
                                                InputMessageHandler = requestHandler,
                                                OutputMessageHandler = responseHandler
                                            }).P0;
        }

        public static Port<CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> requestHandler)
        {
            return CreateChannel(space, new CcrsRequestResponseChannelConfig<TInput, TOutput>
                                             {
                                                 InputMessageHandler = requestHandler
                                             }).P1;
        }

        public static PortSet<TInput, CcrsRequest<TInput, TOutput>> CreateChannel<TInput, TOutput>(this ICcrSpace space, CcrsRequestResponseChannelConfig<TInput, TOutput> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return CcrsChannelFactory.Instance.CreateChannel(config);
        }
        #endregion


        #region Filter channel
        public static Port<TInput> CreateChannel<TInput, TOutput>(this ICcrSpace space, Func<TInput, TOutput> inputHandler, Port<TOutput> responseChannel)
        {
            return CreateChannel<TInput, TOutput>(space, (input, outputPort) => outputPort.Post(inputHandler(input)), responseChannel);
        }

        
        public static Port<TInput> CreateChannel<TInput, TOutput>(this ICcrSpace space, Action<TInput, Port<TOutput>> inputHandler, Port<TOutput> responseChannel)
        {
            var config = new CcrsFilterChannelConfig<TInput, TOutput>
                             {
                                 InputMessageHandler = inputHandler,
                                 OutputPort = responseChannel,
                             };
            return CreateChannel(space, config);
        }

        
        public static Port<TInput> CreateChannel<TInput, TOutput>(this ICcrSpace space, CcrsFilterChannelConfig<TInput, TOutput> config)
        {
            config.TaskQueue = config.TaskQueue ?? space.DefaultTaskQueue;
            return CcrsChannelFactory.Instance.CreateChannel(config);
        }
        #endregion


        public static CcrsPendingRequest<TInput, TOutput> Request<TInput, TOutput>(this Port<CcrsRequest<TInput, TOutput>> port, TInput request)
        {
            return new CcrsPendingRequest<TInput, TOutput>
                       {
                            Requests = port,
                            Request = request
                       };
        }
    }
}
