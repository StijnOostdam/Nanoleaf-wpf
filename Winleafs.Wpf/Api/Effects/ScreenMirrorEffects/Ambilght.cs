﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class Ambilght : IScreenMirrorEffect
    {
        private readonly bool _controlBrightness;
        private readonly INanoleafClient _nanoleafClient;
        private readonly Rectangle _screenBounds;

        public Ambilght(INanoleafClient nanoleafClient, Device device)
        {
            _nanoleafClient = nanoleafClient;
            _controlBrightness = device.ScreenMirrorControlBrightness;
            _screenBounds = ScreenBoundsHelper.GetScreenBounds(device.ScreenMirrorMonitorIndex);
        }

        /// <summary>
        /// Applies the ambilight effect to the lights. Ambilight is the average color of the whole screen
        /// Sets the color of the nanoleaf with the logging disabled.
        /// Seeing as a maximum of 10 requests per second can be set this will generate a lot of unwanted log data.
        /// See https://github.com/StijnOostdam/Winleafs/issues/40.
        /// </summary>
        public async Task ApplyEffect()
        {
            var bitmap = ScreenGrabber.CaptureScreen(_screenBounds);
            var color = ScreenGrabber.CalculateAverageColor(bitmap, _screenBounds);

            var hue = (int)color.GetHue();
            var sat = (int)(color.GetSaturation() * 100);

            if (_controlBrightness)
            {
                //For brightness calculation see: https://stackoverflow.com/a/596243 and https://www.w3.org/TR/AERT/#color-contrast
                //We do not use Color.GetBrightness() since that value is always null because we use Color.FromArgb in the screengrabber.
                //Birghtness can be maximum 100
                var brightness = Math.Min(100, (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B));

                await _nanoleafClient.StateEndpoint.SetHueSaturationAndBrightnessAsync(hue, sat, brightness, disableLogging: true);
            }
            else
            {
                await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat, disableLogging: true);
            }
        }

        
    }
}
