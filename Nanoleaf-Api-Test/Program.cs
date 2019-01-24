﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api;
using Nanoleaf_Api.Endpoints;

namespace Nanoleaf_Api_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var nanoLeafClient = new NanoleafClient("192.168.178.160", 16021);
            Console.WriteLine("Authorizing..");
            nanoLeafClient.AuthorizationEndpoint.GetAuthToken().GetAwaiter().GetResult();
            Console.WriteLine("Authorized!");
            Console.WriteLine("Getting effects...");
            var effects = nanoLeafClient.EffectsEndpoint.GetEffectsListAsync().GetAwaiter().GetResult();
            foreach (var effect in effects)
            {
                Console.WriteLine(effect);
            }

            while (true)
            {
                var effectName = Console.ReadLine();
                nanoLeafClient.EffectsEndpoint.SetSelectedEffectAsync(effectName).GetAwaiter().GetResult();
                Console.WriteLine("Set {0}", effectName);
            }

            Console.ReadLine();
        }
    }
}