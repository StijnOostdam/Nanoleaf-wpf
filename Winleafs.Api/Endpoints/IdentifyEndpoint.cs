﻿using System.Threading.Tasks;

using RestSharp;
using Winleafs.Api.DTOs;
using Winleafs.Api.Endpoints.Interfaces;

namespace Winleafs.Api.Endpoints
{
    public class IdentifyEndpoint : NanoleafEndpoint, IIdentifyEndpoint
    {
        /// <inheritdoc />
        public IdentifyEndpoint(ClientDto client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public Task Identify()
        {
            return SendRequestAsync("identify", Method.PUT);
        }
    }
}
