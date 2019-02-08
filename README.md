# Winleafs

Windows application for Nanoleaf lights.

Installers can be found here: https://github.com/StijnOostdam/Winleafs/releases.

Canvas support currently not guaranteed.

## Features
- Make multiple weekly schedules for your Nanoleaf lights
- Add time based triggers to automatically set your lights to an effect and brightness at your chosen time
- Add triggers based on sunrise and sunset times (requires location)
- Start the program at Windows startup
- Automatically turn off your lights when your PC shuts down
- Override the schedule at any time
- All settings are saved locally

## TODOs & Future features

TODOs and future features can be found on the [GitHub projects board](https://github.com/StijnOostdam/Winleafs/projects/1).

## APIs used

- [Icanhazip](http://icanhazip.com)
- [IP-API](http://ip-api.com/)
- [Sunset Sunrise](https://sunrise-sunset.org/api)

[Donations](https://www.paypal.me/winleafs)

## Bugs
If you are experiencing proglems, try deleting the settings file from %appdata%/Winleafs. Note: this removes all settings and schedules.

1. The Nanoleaf device must be connected to the same router as your Windows device. This means that 2.4Ghz and 5Ghz networks are also seperated as the Nanoleaf connects to 2.4Ghz
