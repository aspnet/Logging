Logging
=======

**This GitHub project has been archived.** Ongoing development on this project can be found in <https://github.com/aspnet/Extensions>.

Common logging abstractions and a few implementations. Refer to the [wiki](https://github.com/aspnet/Logging/wiki) for more information

This project is part of ASP.NET Core. You can find samples, documentation and getting started instructions for ASP.NET Core at the [AspNetCore](https://github.com/aspnet/AspNetCore) repo.

## Providers

Community projects adapt _Microsoft.Extensions.Logging_ for use with different back-ends.

 * [Sentry](https://github.com/getsentry/sentry-dotnet) - provider for the [Sentry](https://github.com/getsentry/sentry) service
 * [Serilog](https://github.com/serilog/serilog-framework-logging) - provider for the Serilog library
 * [elmah.io](https://github.com/elmahio/Elmah.Io.Extensions.Logging) - provider for the elmah.io service
 * [Loggr](https://github.com/imobile3/Loggr.Extensions.Logging) - provider for the Loggr service
 * [NLog](https://github.com/NLog/NLog.Extensions.Logging) - provider for the NLog library
 * [Graylog](https://github.com/mattwcole/gelf-extensions-logging) - provider for the Graylog service
 * [Sharpbrake](https://github.com/airbrake/sharpbrake#microsoftextensionslogging-integration) - provider for the Airbrake notifier
 * [KissLog.net](https://github.com/catalingavan/KissLog-net) - provider for the KissLog.net service

## Building from source

To run a complete build on command line only, execute `build.cmd` or `build.sh` without arguments. See [developer documentation](https://github.com/aspnet/Home/wiki) for more details.
