#include "StdAfx.h"
#include "Wrap.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_DevConfig_TypeAndSoftInfo(int lLoginID, WRAP_DevConfig_TypeAndSoftInfo_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	DHDEV_VERSION_INFO stuInfo = {0};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_SOFTWARE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szDevType, stuInfo.szDevType, sizeof(stuInfo.szDevType));
		strncpy(result->szSoftWareVersion, stuInfo.szSoftWareVersion, sizeof(stuInfo.szSoftWareVersion));
		result->dwSoftwareBuildDate_1 = (stuInfo.dwSoftwareBuildDate>>16) & 0xffff;
		result->dwSoftwareBuildDate_2 = (stuInfo.dwSoftwareBuildDate>>8) & 0xff;
		result->dwSoftwareBuildDate_3 = stuInfo.dwSoftwareBuildDate & 0xff;
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_Get_DevConfig_IPMaskGate(int lLoginID, WRAP_CFG_NETWORK_INFO_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szBuffer, 32 * 1024, &nError, 3000);
	if (bRet)
	{
		int nRetLen = 0;
		CFG_NETWORK_INFO stuNetwork = {0};
		bRet = CLIENT_ParseData(CFG_CMD_NETWORK, szBuffer, &stuNetwork, sizeof(stuNetwork), &nRetLen);

		strncpy(result->szIP, stuNetwork.stuInterfaces->szIP, sizeof(stuNetwork.stuInterfaces->szIP));
		strncpy(result->szSubnetMask, stuNetwork.stuInterfaces->szSubnetMask, sizeof(stuNetwork.stuInterfaces->szSubnetMask));
		strncpy(result->szDefGateway, stuNetwork.stuInterfaces->szDefGateway, sizeof(stuNetwork.stuInterfaces->szDefGateway));
		result->nMTU = stuNetwork.stuInterfaces->nMTU;
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_Set_DevConfig_IPMaskGate(int lLoginID, char* ip, char* mask, char* gate, int mtu)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	int nError = 0;
	CFG_NETWORK_INFO stuNetwork = {0};
	memset(stuNetwork.stuInterfaces[0].szIP, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szIP, ip, sizeof(ip));
	memset(stuNetwork.stuInterfaces[0].szSubnetMask, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szSubnetMask, mask, sizeof(mask));
	memset(stuNetwork.stuInterfaces[0].szDefGateway, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szDefGateway, gate, sizeof(gate));
	stuNetwork.stuInterfaces->nMTU = mtu;

	char szOutBuffer[32 * 1024] = {0};
	int nRestart = 0;
	BOOL bRet = CLIENT_PacketData(CFG_CMD_NETWORK, &stuNetwork, sizeof(stuNetwork), szOutBuffer, 32 * 1024);
	if (bRet)
	{
		bRet = CLIENT_SetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szOutBuffer, 32 * 1024, &nError, &nRestart, 3000);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_DevConfig_MAC(int lLoginID, WRAP_DevConfig_MAC_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	DHDEV_NETINTERFACE_INFO stuInfo = {sizeof(stuInfo)};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_NETINTERFACE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szMAC, stuInfo.szMAC, sizeof(stuInfo.szMAC));
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_DevConfig_RecordFinderCaps(int lLoginID, WRAP_DevConfig_RecordFinderCaps_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	memset(szBuffer, 0, 32 * 1024);
	BOOL bRet = CLIENT_QueryNewSystemInfo(lLoginID, CFG_CAP_CMD_RECORDFINDER, -1, szBuffer, 32 * 1024, &nError, SDK_API_WAITTIME);
	if (bRet)
	{
		int nRetLen = 0;
		CFG_CAP_RECORDFINDER_INFO stuCaps = {0};
		bRet = CLIENT_ParseData(CFG_CAP_CMD_RECORDFINDER, szBuffer, &stuCaps, sizeof(stuCaps), &nRetLen);
		if (bRet)
		{
			result->nMaxPageSize = stuCaps.nMaxPageSize;
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_DevConfig_GetCurrentTime(int lLoginID, NET_TIME* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	BOOL bRet = CLIENT_QueryDeviceTime(lLoginID, &stuNetTime, SDK_API_WAITTIME);
	result->dwYear = stuNetTime.dwYear;
	result->dwMonth = stuNetTime.dwMonth;
	result->dwDay = stuNetTime.dwDay;
	result->dwHour = stuNetTime.dwHour;
	result->dwMinute = stuNetTime.dwMinute;
	result->dwSecond = stuNetTime.dwSecond;
	return bRet;
}

BOOL CALL_METHOD WRAP_DevConfig_SetCurrentTime(int lLoginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	stuNetTime.dwYear = dwYear;
	stuNetTime.dwMonth = dwMonth;
	stuNetTime.dwDay = dwDay;
	stuNetTime.dwHour = dwHour;
	stuNetTime.dwMinute = dwMinute;
	stuNetTime.dwSecond = dwSecond;
	BOOL bRet = CLIENT_SetupDeviceTime(lLoginID, &stuNetTime);
	return bRet;
}

BOOL CALL_METHOD WRAP_Dev_QueryLogList(int lLoginID, WRAP_Dev_QueryLogList_Result* result)
{
	printf("WRAP_Dev_QueryLogList");

	if (NULL == lLoginID)
	{
		return FALSE;
	}
	QUERY_DEVICE_LOG_PARAM stuGetLog;
	memset(&stuGetLog, 0, sizeof(QUERY_DEVICE_LOG_PARAM));
	stuGetLog.emLogType = DHLOG_ALL;
	stuGetLog.stuStartTime.dwYear = 2013;
	stuGetLog.stuStartTime.dwMonth = 10;
	stuGetLog.stuStartTime.dwDay = 1;
	stuGetLog.stuStartTime.dwHour = 0;
	stuGetLog.stuStartTime.dwMinute = 0;
	stuGetLog.stuStartTime.dwSecond = 0;
	
	stuGetLog.stuEndTime.dwYear = 2015;
	stuGetLog.stuEndTime.dwMonth = 10;
	stuGetLog.stuEndTime.dwDay = 7;
	stuGetLog.stuEndTime.dwHour = 0;
	stuGetLog.stuEndTime.dwMinute = 0;
	stuGetLog.stuEndTime.dwSecond = 0;
	
	stuGetLog.nLogStuType = 1;


	int nMaxNum = 10;
	stuGetLog.nStartNum = 0;
	stuGetLog.nEndNum = nMaxNum - 1;
	
	DH_DEVICE_LOG_ITEM_EX* szLogInfos = new DH_DEVICE_LOG_ITEM_EX[32];
	int nRetLogNum = 0;
	BOOL bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
	if (bRet)
	{
		if (nRetLogNum <= 0)
		{
			//printf("No Log record!");
		}
		else
		{
			for (unsigned int i = 0; i < nRetLogNum; i++)
			{
				result->Test = 123;
				result->Logs[i].nLogType = szLogInfos[i].nLogType;
				result->Logs[i].stuOperateTime.day = szLogInfos[i].stuOperateTime.day;
				result->Logs[i].stuOperateTime.hour = szLogInfos[i].stuOperateTime.hour;
				result->Logs[i].stuOperateTime.minute = szLogInfos[i].stuOperateTime.minute;
				result->Logs[i].stuOperateTime.month = szLogInfos[i].stuOperateTime.month;
				result->Logs[i].stuOperateTime.second = szLogInfos[i].stuOperateTime.second;
				result->Logs[i].stuOperateTime.year = szLogInfos[i].stuOperateTime.year;
				strncpy(result->Logs[i].szOperator, szLogInfos[i].szOperator, sizeof(szLogInfos[i].szOperator));
				strncpy(result->Logs[i].szOperation, szLogInfos[i].szOperation, sizeof(szLogInfos[i].szOperation));
				strncpy(result->Logs[i].szDetailContext, szLogInfos[i].szDetailContext, sizeof(szLogInfos[i].szDetailContext));
			}
		}
		
		if (nRetLogNum < nMaxNum)
		{
			memset(szLogInfos, 0, sizeof(DH_DEVICE_LOG_ITEM_EX) * 32);
			stuGetLog.nStartNum += nRetLogNum;  
			nRetLogNum = 0;
			bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
			if (bRet)
			{
				if (nRetLogNum <= 0)
				{
					//printf("No more Log record!");
				}
				else
				{
					for (unsigned int i = 0; i < nRetLogNum; i++)
					{
						//DisPlayLogInfo(szLogInfos[i], i);
					}
				}
			}
		}
	}
	delete[] szLogInfos;
	return TRUE;
}

BOOL CALL_METHOD WRAP_GetProjectPassword(int lLoginId, WRAP_GeneralConfig_Password* result)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_GENERAL_INFO stuInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf, &stuInfo, sizeof(stuInfo), &nRetLen);
		strcpy(result->szProjectPassword, stuInfo.szProjectPassword);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetProjectPassword(int lLoginId, char password[])
{
	char szJsonBufSet[1024 * 40] = {0};
	int nerror = 0;
	int nrestart = 0;

	CFG_ACCESS_GENERAL_INFO stuInfo = {sizeof(stuInfo)};
	stuInfo.abProjectPassword = true;
	strncpy(stuInfo.szProjectPassword, password, __min(strlen(password), sizeof(stuInfo.szProjectPassword)));
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
	bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
	return bRet;
}

BOOL CALL_METHOD WRAP_GetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* result)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;

	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_EVENT_INFO stuGeneralInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, &stuGeneralInfo, sizeof(stuGeneralInfo), &nRetLen);
		memcpy(result, &stuGeneralInfo, sizeof(stuGeneralInfo));
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo)
{
	char szJsonBufSet[1024 * 40] = {0};
			
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, stuGeneralInfo, sizeof(CFG_ACCESS_EVENT_INFO), szJsonBufSet, sizeof(szJsonBufSet));
	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		return bRet;
	}
}

BOOL CALL_METHOD WRAP_DevCtrl_ReBoot(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_REBOOT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

BOOL CALL_METHOD WRAP_DevCtrl_DeleteCfgFile(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RESTOREDEFAULT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

int CALL_METHOD WRAP_DevCtrl_GetLogCount(int lLoginID, QUERY_DEVICE_LOG_PARAM* logParam)
{
	if (NULL == lLoginID)
	{
		return 0;
	}
	NET_IN_GETCOUNT_LOG_PARAM stuInLogCount = {sizeof(NET_IN_GETCOUNT_LOG_PARAM)};
	NET_OUT_GETCOUNT_LOG_PARAM stuOutLogCount = {sizeof(NET_OUT_GETCOUNT_LOG_PARAM)};

	QUERY_DEVICE_LOG_PARAM& stuGetLog = stuInLogCount.stuQueryCondition;
	stuGetLog.stuStartTime.dwYear = logParam->stuStartTime.dwYear;
	stuGetLog.stuStartTime.dwMonth = logParam->stuStartTime.dwMonth;
	stuGetLog.stuStartTime.dwDay = logParam->stuStartTime.dwDay;
	stuGetLog.stuStartTime.dwHour = logParam->stuStartTime.dwHour;
	stuGetLog.stuStartTime.dwMinute = logParam->stuStartTime.dwMinute;
	stuGetLog.stuStartTime.dwSecond = logParam->stuStartTime.dwSecond;
	
	stuGetLog.stuEndTime.dwYear = logParam->stuEndTime.dwYear;
	stuGetLog.stuEndTime.dwMonth = logParam->stuEndTime.dwMonth;
	stuGetLog.stuEndTime.dwDay = logParam->stuEndTime.dwDay;
	stuGetLog.stuEndTime.dwHour = logParam->stuEndTime.dwHour;
	stuGetLog.stuEndTime.dwMinute = logParam->stuEndTime.dwMinute;
	stuGetLog.stuEndTime.dwSecond = logParam->stuEndTime.dwSecond;

	stuGetLog.nStartNum = logParam->nStartNum;
	stuGetLog.nLogStuType = logParam->nLogStuType;
	stuGetLog.nEndNum = logParam->nEndNum;
	
	BOOL bResult = CLIENT_QueryDevLogCount(lLoginID, &stuInLogCount, &stuOutLogCount, SDK_API_WAITTIME);
 	return stuOutLogCount.nLogCount;
}

BOOL CALL_METHOD WRAP_OpenDoor(int lLoginID, int channelNo)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_OPEN stuInert = {sizeof(stuInert)};
	stuInert.nChannelID = channelNo;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_ACCESS_OPEN, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_CloseDoor(int lLoginId, int channelNo)
{
	if (NULL == lLoginId)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = channelNo;
    BOOL bResult = CLIENT_ControlDevice(lLoginId, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
	return bResult;
}

int CALL_METHOD WRAP_GetDoorStatus(int lLoginId, int channelNo)
{
	if (NULL == lLoginId)
	{
		return - 1;
	}
	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
	stuParam.nChannel = channelNo;
	int nRetLen = 0;
	BOOL bResult = CLIENT_QueryDevState(lLoginId, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
	if (bResult)
	{
		switch (stuParam.emStateType)
		{
		case EM_NET_DOOR_STATUS_OPEN:
			return 1;
			break;
		case EM_NET_DOOR_STATUS_CLOSE:
			return 2;
			break;
		default:
			return 0;
			break;
		}
	}
	else
	{
		return - 1;
	}
}
