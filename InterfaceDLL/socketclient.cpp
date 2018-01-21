#include "imports.h"
#include "socketclient.h"

SocketClient::SocketClient(const char* ip, int port, int nInBufferSize) {

	this->nInBufferSize = nInBufferSize;

	pInBuffer = (char*)malloc(sizeof(char) * this->nInBufferSize);

	WSAStartup(MAKEWORD(2, 0), &WSAData);
	server = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	addr.sin_addr.s_addr = inet_addr(ip);
	addr.sin_family = AF_INET;
	addr.sin_port = htons(port); 
}

VOID SocketClient::messageLoop() {

	while (run) {

		memset(pInBuffer, 0, nInBufferSize * (sizeof pInBuffer[0]));
		int iResult = recv(server, pInBuffer, nInBufferSize, 0);

		if (iResult > 0) {
			(*this->onDataReceiveCallback)(pInBuffer, nInBufferSize);
		}
		else if(iResult < 0) {
			cout << ("recv failed: %d\n", WSAGetLastError()) << endl;
		}
	}
}

BOOL SocketClient::sendData(const char* data, int byteCount) {
	return send(server, data, byteCount, 0);
}

BOOL SocketClient::sendData(string data) {
	return sendData(data.c_str(), (int)data.length());
}

DWORD __stdcall startMethodInThread(LPVOID arg)
{
	if (!arg)
		return 0;
	SocketClient *pSocketClient = (SocketClient*)arg;
	pSocketClient->messageLoop();
	return 1;
}

DWORD __stdcall trampolineSocket(LPVOID arg) {
	if (!arg)
		return 0;
	SocketClient *pSocketClient = (SocketClient*)arg;
	pSocketClient->messageLoop();
	return 1;
}

void SocketClient::start(DataReceive onDataReceiveCallback) {

	this->onDataReceiveCallback = onDataReceiveCallback;

	int status = connect(server, (SOCKADDR *)&addr, sizeof(addr));

	if (!hMessageLoopThread)
		hMessageLoopThread = CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)trampolineSocket, this, NULL, NULL);
}

void SocketClient::stop() {
	run = false;
}