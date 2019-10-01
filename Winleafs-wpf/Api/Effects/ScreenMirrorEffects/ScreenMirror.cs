﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;
using Winleafs.Models.Models;
using System.Drawing;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class ScreenMirror : IScreenMirrorEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly IExternalControlEndpoint _externalControlEndpoint;
        private readonly List<DrawablePanel> _panels;
        private readonly int _rectangleSize;
        private readonly Rectangle _screenBounds;
        private readonly Rectangle _capturedBounds; //Simple rectangle that start at 0,0 and has width and height set to the rectangle size

        public ScreenMirror(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient, ScaleType scaleType)
        {
            _nanoleafClient = nanoleafClient;

            _externalControlEndpoint = _nanoleafClient.ExternalControlEndpoint;

            _screenBounds = ScreenBoundsHelper.GetScreenBounds(device.ScreenMirrorMonitorIndex);

            _panels = orchestrator.PanelLayout.GetScaledTriangles(_screenBounds.Width, _screenBounds.Height, scaleType);

            //Set the rectangle size to 1/3th of the length of a side of the triangle. TODO: test what size is best
            var triangle = _panels.FirstOrDefault().Polygon;
            _rectangleSize = (int)Math.Floor(System.Windows.Point.Subtract(triangle.Points[0], triangle.Points[1]).Length / 3);
            _capturedBounds = new Rectangle(0, 0, _rectangleSize, _rectangleSize);
        }

        /// <summary>
        /// Applies the screen mirror effect to the ligts.
        /// Screen mirror takes the average color of each triangle and applies it to that triangle
        /// </summary>
        public async Task ApplyEffect()
        {        
            foreach (var panel in _panels)
            {
                //For each panel, draw a rectangle around its midpoint, according to the set rectangle size
                //Then get the average color of that rectangle and apply the color to the panel
                var startX = (int)Math.Floor(panel.MidPoint.X - (_rectangleSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (_rectangleSize / 2));

                // In multi monitor setup, all screens are joined in one larger pixel area. For example, if you want to take a screenshot of the second from left monitor,
                // you need to start at the right of the first left monitor. Hence, we need to add _screenBounds X and Y here to the location of the rectangle we want to capture
                var bounds = new Rectangle(_screenBounds.X + startX, _screenBounds.Y + startY, _rectangleSize, _rectangleSize);
                var bitmap = ScreenGrabber.CaptureScreen(bounds);

                var color = ScreenGrabber.CalculateAverageColor(bitmap, _capturedBounds, 0);

                await _externalControlEndpoint.SetPanelColorAsync(panel.PanelId, color.R, color.G, color.B);
            }
        }
    }
}
