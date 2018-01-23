#include "imports.h"
#include "remotecall.h"
#include <psapi.h>
#include <tchar.h>

void RemoteCall::ReadProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100]) {

	int i = 0;
	LPCVOID  lpBaseAddress;
	memcpy(&lpBaseAddress, stack + i, sizeof(lpBaseAddress));
	i += sizeof(lpBaseAddress);

	SIZE_T  nSize;
	memcpy(&nSize, stack + i, sizeof(nSize));
	i += sizeof(nSize);

	SIZE_T NumberOfBytesRead; 
	char* out = new char[nSize];
	BOOL readSuccess = ReadProcessMemory(*hProc, lpBaseAddress, out, nSize, &NumberOfBytesRead);
	if (!readSuccess)
		cout << "read failed. error: " << GetLastError() << endl;

	int testResult = *(int*)out;

	pSocketClient->sendData(out, nSize);
	delete[] out;
	out = NULL;
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

	*hProc = OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);	
}

void RemoteCall::GetModuleBaseR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100])
{
	HMODULE hMods[1024];
	HANDLE hProcess = *hProc;
	DWORD cbNeeded; 
	unsigned int i;
	LPWSTR desired = (LPWSTR)stack;

	if (hProcess == NULL)
		return;

	if (EnumProcessModules(hProcess, hMods, sizeof(hMods), &cbNeeded))
	{
		for (i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
		{
			LPWSTR szModName = new WCHAR[MAX_PATH];

			if (GetModuleFileNameEx(hProcess, hMods[i], szModName, sizeof(szModName) / sizeof(TCHAR)))
			{
				if (wcscmp(szModName, desired)) {
					pSocketClient->sendData((char*)hMods[i], sizeof(HMODULE));
					return;
				}
			}
			delete[] szModName;
			szModName = NULL;
		}
	}
}