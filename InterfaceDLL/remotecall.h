#pragma once
#include "socketclient.h"

class RemoteCall {

private:

public:
	static void ReadProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]);
	static void WriteProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]);
	static void GetProcessAddressR(SocketClient* pSocketClient, byte stack[100]);
	static void OpenProcessR(HANDLE* hProc, byte stack[100]);
};