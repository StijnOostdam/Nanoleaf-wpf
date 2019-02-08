﻿using System.Threading.Tasks;

using Winleafs.Models.Models.Sunset;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface ISunsetEndpoint
    {
        /// <summary>
        /// Gets the sunset and sunrise times based off the latitude and longitude.
        /// </summary>
        /// <param name="lat">The user's latitude.</param>
        /// <param name="lon">The user's longitude.</param>
        /// <returns>The sunset and sunrise times.</returns>
        Task<SunsetTimes> GetSunsetSunriseAsync(double lat, double lon);

        /// <inheritdoc cref="GetSunsetSunriseAsync"/>
		SunsetTimes GetSunsetSunrise(double lat, double lon);
    }
}
