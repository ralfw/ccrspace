using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcrSpaces.Api.Config
{
    internal class CcrsListenerConfig<TMessage>
    {
        public string Name;
        public Action<TMessage> MessageHandler;
        public bool ProcessSequentially;
    }
}
