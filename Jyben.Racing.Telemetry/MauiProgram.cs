using Microsoft.Extensions.Logging;
using Jyben.Racing.Telemetry.Services;
using Jyben.Racing.Telemetry.Services.Impl;
using Jyben.Racing.Telemetry.Infrastructure.States;

namespace Jyben.Racing.Telemetry;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<IPermissionsService, PermissionsService>();
		builder.Services.AddSingleton<AppState>();
		builder.Services.AddSingleton<ITelemetrieArrierePlanService, TelemetrieArrierePlanService>();

        return builder.Build();
	}
}
