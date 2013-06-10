using System;
using System.ServiceModel;

namespace GuildWars2.Overlay.Contract
{
    [ServiceContract]
    public interface IOverlay
    {
        [OperationContract]
        void Reload();

        [OperationContract]
        void LoadHTML(string html);

        [OperationContract]
        void LoadUri(Uri uri);

        [OperationContract]
        void ExecuteJavascript(string js);
    }
}
