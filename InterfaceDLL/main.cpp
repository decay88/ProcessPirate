#pragma once
#include "secur.h"

#include <WinSock2.h>
#include "imports.h"
#include "socketclient.h"
#include "remotecall.h"
#include "packaging.h"

#pragma comment (lib, "Ws2_32.lib")


SocketClient* pSocketClient;
HANDLE hProc;
#define HOST_IP "127.0.0.1"
#define HOST_PORT 1337
#define INPUT_BUFF_SIZE sizeof(RemoteCallInformation)

void onDataReceive(char* msg, int length) {	
	pRemoteCallInformation remoteCallInfo = (pRemoteCallInformation)msg;

	switch (remoteCallInfo->functionIdentifier) {

	case FunctionIdentifier::GET_PROC_ADDRESS:
		break;

	case FunctionIdentifier::OPEN_PROCESS:
	{
		RemoteCall::OpenProcessR(&hProc, *(RemoteCall::OpenProc_args*)remoteCallInfo->payload);
		break;
	}
	case FunctionIdentifier::READ_PROCESS_MEMORY:
	{
		RemoteCall::ReadProcessMemoryR(&hProc, pSocketClient, *(RemoteCall::RPM_args*)remoteCallInfo->payload);
		break;
	}

	case FunctionIdentifier::WRITE_PROCESS_MEMORY:
	{
		RemoteCall::WriteProcessMemoryR(&hProc, pSocketClient, *(RemoteCall::WPM_args*)remoteCallInfo->payload)
		break;
	}

	case FunctionIdentifier::GET_MODULE_BASE:
	{
		RemoteCall::GetModuleBaseR(&hProc, pSocketClient, remoteCallInfo->payload);
		break;
	}

	default:
		break;
	}
	
}



BOOL WINAPI DllMain(
	_In_ HINSTANCE hinstDLL,
	_In_ DWORD     fdwReason,
	_In_ LPVOID    lpvReserved
)
{
	if (fdwReason == DLL_PROCESS_ATTACH) {
		AllocConsole();
		freopen("CONIN$", "r", stdin);
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);

		cout << "Starting..." << endl;
		pSocketClient = new SocketClient(HOST_IP, HOST_PORT, INPUT_BUFF_SIZE);
		pSocketClient->start(onDataReceive);
		cout << "Setup..." << endl;
		pSocketClient->sendData("C++ connected");
	}
	return TRUE;
}