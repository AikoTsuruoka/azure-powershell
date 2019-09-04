﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.Common.Authentication.Authentication.Clients;

namespace Microsoft.Azure.Commands.Common.Authentication
{
    /// <summary>
    /// Class implementing a chain of responsibility pattern for authenticators
    /// </summary>
    public abstract class DelegatingAuthenticator : IAuthenticator
    {
        protected AuthenticationClientFactory _authenticationClientFactory;

        public IAuthenticator Next { get; set; }
        public abstract bool CanAuthenticate(AuthenticationParameters parameters);
        public abstract Task<IAccessToken> Authenticate(AuthenticationParameters parameters);

        public bool TryAuthenticate(AuthenticationParameters parameters, out Task<IAccessToken> token)
        {
            token = null;
            if (CanAuthenticate(parameters))
            {
                token = Authenticate(parameters);
                return true;
            }

            if (Next != null)
            {
                return Next.TryAuthenticate(parameters, _authenticationClientFactory, out token);
            }

            return false;
        }

        public bool TryAuthenticate(AuthenticationParameters parameters, AuthenticationClientFactory authenticationClientFactory, out Task<IAccessToken> token)
        {
            _authenticationClientFactory = authenticationClientFactory;
            return TryAuthenticate(parameters, out token);
        }
    }
}
