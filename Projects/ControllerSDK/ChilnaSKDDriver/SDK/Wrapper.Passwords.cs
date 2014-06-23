﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddPassword(Password password)
		{
			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = new NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD();
			stuAccessCtlPwd.stuCreateTime.dwYear = password.CreationDateTime.Year;
			stuAccessCtlPwd.stuCreateTime.dwMonth = password.CreationDateTime.Month;
			stuAccessCtlPwd.stuCreateTime.dwDay = password.CreationDateTime.Day;
			stuAccessCtlPwd.stuCreateTime.dwHour = password.CreationDateTime.Hour;
			stuAccessCtlPwd.stuCreateTime.dwMinute = password.CreationDateTime.Minute;
			stuAccessCtlPwd.stuCreateTime.dwSecond = password.CreationDateTime.Second;
			stuAccessCtlPwd.szUserID = StringToCharArray(password.UserID, 32);
			stuAccessCtlPwd.szDoorOpenPwd = StringToCharArray(password.DoorOpenPassword, 64);
			stuAccessCtlPwd.szAlarmPwd = StringToCharArray(password.AlarmPassword, 64);
			stuAccessCtlPwd.nDoorNum = password.DoorsCount;
			stuAccessCtlPwd.sznDoors = new int[32];
			stuAccessCtlPwd.sznDoors[0] = 1;
			stuAccessCtlPwd.sznDoors[1] = 2;

			var result = NativeWrapper.WRAP_Insert_Pwd(LoginID, ref stuAccessCtlPwd);
			return result;
		}

		public bool RemovePassword(int index)
		{
			var result = NativeWrapper.WRAP_RemovePassword(LoginID, index);
			return result;
		}

		public bool RemoveAllPasswords()
		{
			var result = NativeWrapper.WRAP_RemoveAllPasswords(LoginID);
			return result;
		}

		public Password GetPasswordInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetPasswordInfo(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD sdkPassword = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_PWD)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var password = new Password();
			password.RecordNo = sdkPassword.nRecNo;
			password.CreationDateTime = NET_TIMEToDateTime(sdkPassword.stuCreateTime);
			password.UserID = CharArrayToString(sdkPassword.szUserID);
			password.DoorOpenPassword = CharArrayToString(sdkPassword.szDoorOpenPwd);
			password.AlarmPassword = CharArrayToString(sdkPassword.szAlarmPwd);
			password.DoorsCount = sdkPassword.nDoorNum;
			password.Doors = sdkPassword.sznDoors;

			return password;
		}

		public int GetPasswordsCount()
		{
			var passwordsCount = NativeWrapper.WRAP_Get_PasswordsCount(LoginID);
			return passwordsCount;
		}

		public List<Password> GetAllPasswords()
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.PasswordsCollection));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetAllPasswords(LoginID, intPtr);

			NativeWrapper.PasswordsCollection passwordsCollection = (NativeWrapper.PasswordsCollection)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.PasswordsCollection)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var passwords = new List<Password>();

			for (int i = 0; i < Math.Min(passwordsCollection.Count, 500); i++)
			{
				var sdkPassword = passwordsCollection.Passwords[i];
				var password = new Password();
				password.RecordNo = sdkPassword.nRecNo;
				password.CreationDateTime = NET_TIMEToDateTime(sdkPassword.stuCreateTime);
				password.UserID = CharArrayToString(sdkPassword.szUserID);
				password.DoorOpenPassword = CharArrayToString(sdkPassword.szDoorOpenPwd);
				password.AlarmPassword = CharArrayToString(sdkPassword.szAlarmPwd);
				password.DoorsCount = sdkPassword.nDoorNum;
				password.Doors = sdkPassword.sznDoors;
				passwords.Add(password);
			}
			return passwords;
		}
	}
}