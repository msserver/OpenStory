﻿using System;
using System.ServiceModel;
using OpenStory.Services.Contracts;

namespace OpenStory.Services.Clients
{
    /// <summary>
    /// Provides remote access to an <see cref="IAccountService"/>.
    /// </summary>
    public sealed class AccountServiceClient : GameServiceClient<IAccountService>, IAccountService
    {
        /// <summary>
        /// Initializes a new AccountServiceClient.
        /// </summary>
        public AccountServiceClient()
            : base(ServiceConstants.Uris.AccountService)
        {
        }

        #region IAccountService Members

        /// <inheritdoc />
        public bool TryRegisterSession(int accountId, out int sessionId)
        {
            return base.Channel.TryRegisterSession(accountId, out sessionId);
        }

        /// <inheritdoc />
        public bool TryRegisterCharacter(int accountId, int characterId)
        {
            return base.Channel.TryRegisterCharacter(accountId, characterId);
        }

        /// <inheritdoc />
        public bool TryUnregisterSession(int accountId)
        {
            return base.Channel.TryUnregisterSession(accountId);
        }

        /// <inheritdoc />
        public bool TryKeepAlive(int accountId, out TimeSpan lag)
        {
            try
            {
                return base.Channel.TryKeepAlive(accountId, out lag);
            }
            catch (EndpointNotFoundException)
            {
                lag = default(TimeSpan);
                return false;
            }
            catch (TimeoutException)
            {
                lag = default(TimeSpan);
                return false;
            }
        }

        #endregion
    }
}
