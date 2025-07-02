using System;
using Photon.Realtime;

namespace Photon.Chat.Demo
{
	// Token: 0x020002C5 RID: 709
	public static class AppSettingsExtensions
	{
		// Token: 0x0600118B RID: 4491 RVA: 0x00056650 File Offset: 0x00054850
		public static ChatAppSettings GetChatSettings(this AppSettings appSettings)
		{
			return new ChatAppSettings
			{
				AppIdChat = appSettings.AppIdChat,
				AppVersion = appSettings.AppVersion,
				FixedRegion = (appSettings.IsBestRegion ? null : appSettings.FixedRegion),
				NetworkLogging = appSettings.NetworkLogging,
				Protocol = appSettings.Protocol,
				EnableProtocolFallback = appSettings.EnableProtocolFallback,
				Server = (appSettings.IsDefaultNameServer ? null : appSettings.Server),
				Port = (ushort)appSettings.Port,
				ProxyServer = appSettings.ProxyServer
			};
		}
	}
}
