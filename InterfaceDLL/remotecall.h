#pragma once
#include "socketclient.h"

class RemoteCall {

private:

public:
	typedef struct RPM_args {
		LPCVOID  lpBaseAddress;
		SIZE_T  nSize;
		SIZE_T NumberOfBytesRead;
	};

	typedef struct WPM_args {
		LPVOID  lpBaseAddress;
		LPCVOID lpBuffer;
		SIZE_T  nSize;
		SIZE_T  *lpNumberOfBytesWritten;
	};

	typedef struct GetProcAddr_args {
		HMODULE hModule;
		LPCSTR  lpProcName;
	};

	typedef struct OpenProc_args {
		DWORD dwDesiredAccess;
		BOOL bInheritHandle;
		DWORD dwProcessId;
	};

	static void ReadProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, RPM_args args);
	static void WriteProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, WPM_args args);
	static void GetProcessAddressR(SocketClient* pSocketClient, GetProcAddr_args args);
	static void OpenProcessR(HANDLE* hProc, OpenProc_args args);
	static void GetModuleBaseR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]);
};