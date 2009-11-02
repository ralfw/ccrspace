using System;
using System.Threading;
using Microsoft.Ccr.Core;
namespace CcrSpaces.Actors
{
    public class CcrsPulsator : IDisposable
    {
        private readonly Timer pulsator;


        internal CcrsPulsator(IPort actor, int pulsationPeriodMsec)
        {
            this.pulsator = new Timer(x => actor.PostUnknownType(DateTime.Now), null, pulsationPeriodMsec, pulsationPeriodMsec);
        }


        #region Implementation of IDisposable

        public void Dispose()
        {
            this.pulsator.Dispose();
        }

        #endregion
    }
}
