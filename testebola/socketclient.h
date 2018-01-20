#pragma once

typedef void(*DataReceive)(char* i, int length);

class SocketClient {

public:
	SocketClient(const char* ip, int port, int nInBufferSize);
	~SocketClient();

	BOOL sendData(const char* data, int byteCount);
	BOOL sendData(string data);
	void start(DataReceive onDataReceiveCallback);
	void stop();
	VOID messageLoop();

private:
	WSADATA WSAData;
	SOCKET server;
	SOCKADDR_IN addr;

	char* pInBuffer;
	int nInBufferSize;

	BOOL run = TRUE;
	HANDLE hMessageLoopThread;

	DataReceive onDataReceiveCallback;
};