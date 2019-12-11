using System;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Domain.ViewModels
{
    public class CommunicationRoleViewModel
    {
        public string Name
        {
            get
            {
                switch (Role)
                {
                    case CommunicationRole.Server:
                        return "Server";
                    case CommunicationRole.Client:
                        return "Client";
                    case CommunicationRole.P2PClient:
                    case CommunicationRole.P2PClientPassive:
                        return "P2PClient";
                    default:
                        throw new NotImplementedException($"Missing CommunicationRole {Role}");
                }
            }
        }

        public CommunicationRole Role { get; }
        public CommunicationRoleViewModel(CommunicationRole role)
        {
            Role = role;
        }
    }
}