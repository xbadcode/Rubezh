﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddHoliday(Holiday holiday)
		{
			var nativeHoliday = HolidayToNativeHoliday(holiday);
			var result = NativeWrapper.WRAP_Insert_Holiday(LoginID, ref nativeHoliday);
			return result;
		}

		public bool EditHoliday(Holiday holiday)
		{
			var nativeHoliday = HolidayToNativeHoliday(holiday);
			var result = NativeWrapper.WRAP_Update_Holiday(LoginID, ref nativeHoliday);
			return result;
		}

		public bool RemoveHoliday(int index)
		{
			var result = NativeWrapper.WRAP_RemoveHoliday(LoginID, index);
			return result;
		}

		public bool RemoveAllHolidays()
		{
			var result = NativeWrapper.WRAP_RemoveAllHolidays(LoginID);
			return result;
		}

		public Holiday GetHolidayInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_HOLIDAY));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetHolidayInfo(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_HOLIDAY nativeHoliday = (NativeWrapper.NET_RECORDSET_HOLIDAY)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_HOLIDAY)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var holiday = NativeHolidayToHoliday(nativeHoliday);
			return holiday;
		}

		public int GetHolidaysCount()
		{
			var holidaysCount = NativeWrapper.WRAP_Get_HolidaysCount(LoginID);
			return holidaysCount;
		}

		public List<Holiday> GetAllHolidays()
		{
			return new List<Holiday>();
		}

		NativeWrapper.NET_RECORDSET_HOLIDAY HolidayToNativeHoliday(Holiday holiday)
		{
			NativeWrapper.NET_RECORDSET_HOLIDAY nativeHoliday = new NativeWrapper.NET_RECORDSET_HOLIDAY();
			nativeHoliday.nDoorNum = holiday.DoorsCount;
			nativeHoliday.sznDoors = new int[32];
			for (int i = 0; i < holiday.Doors.Count; i++)
			{
				nativeHoliday.sznDoors[i] = holiday.Doors[i];
			}

			nativeHoliday.stuStartTime.dwYear = holiday.StartDateTime.Year;
			nativeHoliday.stuStartTime.dwMonth = holiday.StartDateTime.Month;
			nativeHoliday.stuStartTime.dwDay = holiday.StartDateTime.Day;
			nativeHoliday.stuStartTime.dwHour = holiday.StartDateTime.Hour;
			nativeHoliday.stuStartTime.dwMinute = holiday.StartDateTime.Minute;
			nativeHoliday.stuStartTime.dwSecond = holiday.StartDateTime.Second;

			nativeHoliday.stuEndTime.dwYear = holiday.EndDateTime.Year;
			nativeHoliday.stuEndTime.dwMonth = holiday.EndDateTime.Month;
			nativeHoliday.stuEndTime.dwDay = holiday.EndDateTime.Day;
			nativeHoliday.stuEndTime.dwHour = holiday.EndDateTime.Hour;
			nativeHoliday.stuEndTime.dwMinute = holiday.EndDateTime.Minute;
			nativeHoliday.stuEndTime.dwSecond = holiday.EndDateTime.Second;
			nativeHoliday.bEnable = holiday.IsEnabled;
			return nativeHoliday;
		}

		Holiday NativeHolidayToHoliday(NativeWrapper.NET_RECORDSET_HOLIDAY nativeHoliday)
		{
			var holiday = new Holiday();
			holiday.RecordNo = nativeHoliday.nRecNo;
			holiday.DoorsCount = nativeHoliday.nDoorNum;
			holiday.StartDateTime = NET_TIMEToDateTime(nativeHoliday.stuStartTime);
			holiday.EndDateTime = NET_TIMEToDateTime(nativeHoliday.stuEndTime);
			holiday.IsEnabled = nativeHoliday.bEnable;
			return holiday;
		}
	}
}