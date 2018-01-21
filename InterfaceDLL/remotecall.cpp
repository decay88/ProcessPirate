#include "imports.h"
#include "remotecall.h"

void RemoteCall::ReadProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]) {

	int i = 0;
	INT64  lpBaseAddress;
	memcpy(&lpBaseAddress, stack + i, sizeof(lpBaseAddress));
	i += sizeof(lpBaseAddress);

	SIZE_T  nSize;
	memcpy(&nSize, stack + i, sizeof(nSize));
	i += sizeof(nSize);

	SIZE_T NumberOfBytesRead; 
	const char* out = (char*)malloc(nSize);
	ReadProcessMemory(hProc, (LPCVOID)lpBaseAddress, (LPVOID)out, nSize, &NumberOfBytesRead);

	pSocketClient->sendData(out, nSize);
}

void RemoteCall::WriteProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]) {


}

void RemoteCall::GetProcessAddressR(SocketClient* pSocketClient, byte stack[100]) {

}

void RemoteCall::OpenProcessR(HANDLE* hProc, byte stack[100]) {
	int i = 0;
	DWORD dwDesiredAccess;
	memcpy(&dwDesiredAccess, stack + i, sizeof(dwDesiredAccess));
	i += sizeof(dwDesiredAccess);

	BOOL bInheritHandle;
	memcpy(&bInheritHandle, stack + i, sizeof(bInheritHandle));
	i += sizeof(bInheritHandle);

	DWORD dwProcessId;
	memcpy(&dwProcessId, stack + i, sizeof(dwProcessId));

	*hProc = OpenProcess(dwDesiredAccess, bInheritHandle, dwDesiredAccess);	
}