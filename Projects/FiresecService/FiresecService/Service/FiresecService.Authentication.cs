﻿using Common;
using RubezhAPI;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		OperationResult<bool> Authenticate(ClientCredentials clientCredentials)
		{
			if (!CheckLogin(clientCredentials))
			{
				return OperationResult<bool>.FromError("Неверный логин или пароль");
			}
			if (!CheckRemoteAccessPermissions(clientCredentials))
			{
				return OperationResult<bool>.FromError("У пользователя " + clientCredentials.UserName + " нет прав на подкючение к удаленному серверу c хоста: " + clientCredentials.ClientIpAddressAndPort);
			}
			if (!CheckPermissions(clientCredentials))
			{
				return OperationResult<bool>.FromError("У пользователя " + clientCredentials.UserName + " нет прав на работу с программой");
			}
			if (!CheckClientsCount(clientCredentials))
			{
				return OperationResult<bool>.FromError("Сервер отказал в доступе в связи с отсутствием лицензии или достижением максимального количества клиентов");
			}
			return new OperationResult<bool>(true);
		}

		bool CheckClientsCount(ClientCredentials clientCredentials)
		{
			return clientCredentials.ClientType == ClientType.Administrator || !clientCredentials.IsRemote
				|| ClientsManager.ClientInfos.Count(x => x.ClientCredentials.ClientType != ClientType.Administrator && x.ClientCredentials.IsRemote)
				< LicenseManager.CurrentLicenseInfo.RemoteClientsCount;
		}

		bool CheckRemoteAccessPermissions(ClientCredentials clientCredentials)
		{
			if (CheckHostIps(clientCredentials, "localhost"))
				return true;
			if (CheckHostIps(clientCredentials, "127.0.0.1"))
				return true;

			var remoteAccessPermissions = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName).RemoreAccess;
			if (remoteAccessPermissions == null)
				return false;

			switch (remoteAccessPermissions.RemoteAccessType)
			{
				case RemoteAccessType.RemoteAccessBanned:
					return false;

				case RemoteAccessType.RemoteAccessAllowed:
					return true;

				case RemoteAccessType.SelectivelyAllowed:
					foreach (var hostNameOrIpAddress in remoteAccessPermissions.HostNameOrAddressList)
					{
						if (CheckHostIps(clientCredentials, hostNameOrIpAddress))
							return true;
					}
					break;
			}
			return false;
		}

		bool CheckPermissions(ClientCredentials clientCredentials)
		{
			PermissionType? permission = null;
			if (clientCredentials.ClientType == ClientType.Administrator)
				permission = PermissionType.Adm_ViewConfig;
			else if (clientCredentials.ClientType == ClientType.Monitor)
				permission = PermissionType.Oper_Login;
			if (!permission.HasValue)
				return false;
			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName);
			return user == null ? false : user.HasPermission(permission.Value);
		}

		bool CheckHostIps(ClientCredentials clientCredentials, string hostNameOrIpAddress)
		{
			try
			{
				var addressList = Dns.GetHostEntry(hostNameOrIpAddress).AddressList;
				return addressList.Any(ip => ip.ToString() == clientCredentials.ClientIpAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.CheckHostIps");
				return false;
			}
		}

		bool CheckLogin(ClientCredentials clientCredentials)
		{
			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName);
			{
				if (user == null)
				{
					return false;
				}
				if (!HashHelper.CheckPass(clientCredentials.Password, user.PasswordHash))
				{
					return false;
				}
			}

			SetUserFullName(clientCredentials);
			return true;
		}

		void SetUserFullName(ClientCredentials clientCredentials)
		{
			string userIp = "127.0.0.1";
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					userIp = endpoint.Address;
				}
			}
			catch { }

			var addressList = Dns.GetHostEntry("localhost").AddressList;
			if (addressList.Any(ip => ip.ToString() == userIp))
				userIp = "localhost";

			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName);
			clientCredentials.FriendlyUserName = user.Name;// +" (" + userIp + ")";
		}
	}
}