#pragma once

typedef void(*DataReceive)(char* i, int length);

class Pipe {

public:	
	Pipe(LPTSTR pipeName, int nOutBufferSize, int nInBufferSize, int timeout);
	~Pipe();

	BOOL sendData(const char* data, int byteCount);
	BOOL sendData(string data);
	BOOL isConnected();
	void start(DataReceive onDataReceiveCallback);
	void stop();
	VOID pipeLoop();

private:	
	LPTSTR lpszPipename; // = (LPTSTR)"\\\\.\\pipe\\SAPipe";
	HANDLE hPipe = NULL;
	BOOL bIsConnected = FALSE;
	DWORD dwWrite, dwRead;

	char* szServerUpdate;
	char* szClientUpdate;
	int nOutBufferSize;
	int nInBufferSize;

	BOOL run = TRUE;
	HANDLE hPipeLoopThread;

	DataReceive onDataReceiveCallback;
};