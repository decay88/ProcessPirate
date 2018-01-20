#include "secur.h"
#include "imports.h"
#include "piping.h"

Pipe::Pipe(LPTSTR pipeName, int nOutBufferSize, int nInBufferSize, int timeout) {

	this->lpszPipename = pipeName;
	this->nOutBufferSize = nOutBufferSize;
	this->nInBufferSize = nInBufferSize;

	szServerUpdate = (char*)malloc(sizeof(char) * this->nOutBufferSize);
	szClientUpdate = (char*)malloc(sizeof(char) * this->nInBufferSize);

	hPipe = CreateFile(
		lpszPipename,   // pipe name 
		GENERIC_READ |  // read and write access 
		GENERIC_WRITE,
		0,              // no sharing 
		NULL,           // default security attributes
		OPEN_EXISTING,  // opens existing pipe 
		FILE_ATTRIBUTE_NORMAL,              // default attributes 
		NULL);          // no template file 


	if (hPipe != INVALID_HANDLE_VALUE) {

		cout << "hPipe valid." << endl;
		//ConnectNamedPipe(hPipe, NULL);
		bIsConnected = TRUE;

		DWORD dwMode = PIPE_READMODE_MESSAGE;
		DWORD fSuccess = SetNamedPipeHandleState(
			hPipe,    // pipe handle 
			&dwMode,  // new pipe mode 
			NULL,     // don't set maximum bytes 
			NULL);    // don't set maximum time 
		if (!fSuccess)
		{
			cout << TEXT("SetNamedPipeHandleState failed. GLE=") << GetLastError() << endl;
		}
	}
}

VOID Pipe::pipeLoop() {

	while (run) {		

		BOOL readSuccess = ReadFile(hPipe, szClientUpdate, nInBufferSize, &dwRead, NULL);

		if (!readSuccess && GetLastError() != ERROR_MORE_DATA)
		{ 
			cout << (_T("ReadFile failed w/err 0x%08lx\n"), GetLastError()) << endl;;
			break;
		}

		if (dwRead > 0) {
			cout << "read sucess." << endl;
			(*this->onDataReceiveCallback)(szClientUpdate, nInBufferSize);
		}
	}
}

BOOL Pipe::sendData(const char* data, int byteCount) {
	return	WriteFile(hPipe, data, byteCount, &dwWrite, NULL);
}

BOOL Pipe::sendData(string data) {
	return sendData(data.c_str(), (int)data.length());
}

BOOL Pipe::isConnected() {
	return bIsConnected;
}

DWORD __stdcall trampolinePipe(LPVOID arg) {
	if (!arg)
		return 0;
	Pipe *pPipe = (Pipe*)arg;
	pPipe->pipeLoop();
	return 1;
}

void Pipe::start(DataReceive onDataReceiveCallback) {
	cout << "starting" << endl;
	run = true;
	this->onDataReceiveCallback = onDataReceiveCallback;
	sendData("hello from client");
	//if (!hPipeLoopThread)
	//	hPipeLoopThread = CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)&startMethodInThread, this, NULL, NULL);
	pipeLoop();
}

void Pipe::stop() {
	run = false;
}