﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.ExternalControl;

namespace Winleafs.Api.Endpoints
{
    public class ExternalControlEndpoint : NanoleafEndpoint, IExternalControlEndpoint
    {
        private ExternalControlInfo _externalControlInfo;

        /// <inheritdoc />
        public ExternalControlEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public ExternalControlInfo GetExternalControlInfo()
        {
            return SendRequest<ExternalControlInfo>("effects", Method.PUT, body: "{\"write\": {\"command\": \"display\", \"animType\": \"extControl\"}}");
        }

        /// <inheritdoc />
        public async Task<ExternalControlInfo> GetExternalControlInfoAsync()
        {
            return await SendRequestAsync<ExternalControlInfo>("effects", Method.PUT, body: "{\"write\": {\"command\": \"display\", \"animType\": \"extControl\"}}");
        }

        /// <inheritdoc />
        public async Task PrepareForExternalControl()
        {
            _externalControlInfo = await GetExternalControlInfoAsync();
        }

        /// <inheritdoc />
        public async Task SetPanelColorAsync(int panelId, int red, int green, int blue, int transitionTime = 1)
        {
            if (transitionTime < 1)
            {
                throw new ArgumentException($"{nameof(transitionTime)} must be equal or greater than 1");
            }

            SendUDPCommand(1, panelId, 1, red, green, blue, 0, transitionTime);
        }

        /// <summary>
        /// Sends a string via UDP Datagram to the Nanoleaf device
        /// There is no need to keep the connection alive due to datagram <see cref="SocketType.Dgram"/>
        /// </summary>
        private void SendUDPCommand(params int[] numbers)
        {
            var data = DataToBytes(numbers);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endpoint = new IPEndPoint(IPAddress.Parse(_externalControlInfo.StreamIPAddress), _externalControlInfo.StreamPort);

            socket.SendTo(data, endpoint);
            socket.Close();
        }

        private byte[] DataToBytes(int[] numbers)
        {
            var bytes = new byte[numbers.Length];

            for (var i = 0; i < numbers.Length; i++)
            {
                bytes[i] = Convert.ToByte(numbers[i]);
            }

            return bytes;
        }
    }
}
