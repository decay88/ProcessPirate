#include "imports.h"
#include "remotecall.h"
#include <psapi.h>
#include <tchar.h>

void RemoteCall::ReadProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, RPM_args args) {

	char* out = new char[args.nSize];
	BOOL readSuccess = ReadProcessMemory(*hProc, args.lpBaseAddress, out, args.nSize, &args.NumberOfBytesRead);
	if (!readSuccess)
		cout << "read failed. error: " << GetLastError() << endl;

	pSocketClient->sendData(out, args.nSize);
	delete[] out;
	out = NULL;
}

void RemoteCall::WriteProcessMemoryR(HANDLE* hProc, SocketClient* pSocketClient, WPM_args args) {
	
}

void RemoteCall::GetProcessAddressR(SocketClient* pSocketClient, GetProcAddr_args args) {

}

void RemoteCall::OpenProcessR(HANDLE* hProc, OpenProc_args args) {
	
	*hProc = OpenProcess(args.dwDesiredAccess, args.bInheritHandle, args.dwProcessId);
}

void RemoteCall::GetModuleBaseR(HANDLE* hProc, SocketClient* pSocketClient, byte stack[100])
{
	HMODULE hMods[1024];
	HANDLE hProcess = *hProc;
	DWORD cbNeeded; 
	unsigned int i;
	LPWSTR desired = (LPWSTR)stack;
	DWORD lastError;

	if (hProcess == NULL || hProcess == INVALID_HANDLE_VALUE)
		return;

	if (EnumProcessModules(hProcess, hMods, sizeof(hMods), &cbNeeded))
	{
		for (i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
		{
			LPWSTR szModName = new WCHAR[MAX_PATH];

			if (GetModuleFileNameEx(hProcess, hMods[i], szModName, MAX_PATH * sizeof(TCHAR)))
			{
				LPWSTR pwc = wcstok(szModName, L"\\");
				LPWSTR last = NULL;
				while (pwc){
					last = pwc;
					pwc = wcstok(NULL, L"\\");					 
				}
				if (wcscmp(last, desired) == 0) {
					HMODULE hMod = hMods[i];
					pSocketClient->sendData((char*)&hMod, sizeof(HMODULE));
					delete[] szModName;
					szModName = NULL;
					return;
				}
			}
			delete[] szModName;
			szModName = NULL;
		}
	}
	else
		lastError = GetLastError();
}