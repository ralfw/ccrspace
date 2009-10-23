using System;
using Microsoft.Ccr.Core;

namespace CcrSpaces.Channels
{
    public interface ICcrsConverterToRequestOfUnknownType
    {
        CcrsRequestOfUnknownType ToRequestOfUnknownType();
    }


    [Serializable]
    public class CcrsRequestOfUnknownType
    {
        public CcrsRequestOfUnknownType(object request, IPort responses)
        {
            this.Request = request;
            this.Responses = responses;
        }

        public object Request;
        public IPort Responses;
    }

    
    [Serializable]
    public class CcrsRequest<TInput, TOutput> : ICcrsConverterToRequestOfUnknownType
    {
        public CcrsRequest(TInput request, Port<TOutput> responses)
        {
            this.Request = request;
            this.Responses = responses;
        }

        public TInput Request;
        public Port<TOutput> Responses;


        public static implicit operator CcrsRequestOfUnknownType(CcrsRequest<TInput, TOutput> source)
        {
            return new CcrsRequestOfUnknownType(source.Request, source.Responses);
        }


        #region Implementation of ICcrsConverterToRequestOfUnknownType
        public CcrsRequestOfUnknownType ToRequestOfUnknownType()
        {
            return new CcrsRequestOfUnknownType(this.Request, this.Responses);
        }
        #endregion
    }

    

}
