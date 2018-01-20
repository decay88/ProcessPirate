#pragma once
#include "secur.h"

#include <WinSock2.h>
#include "imports.h"
#include "socketclient.h"

#pragma comment (lib, "Ws2_32.lib")

SocketClient* pSocketClient;
#define HOST_IP "127.0.0.1"
#define HOST_PORT 1337
#define INPUT_BUFF_SIZE 100

void onDataReceive(char* i, int length) {
	cout << "C++: got data." << i << endl;
}


int main()
{	
	cout << "Starting..." << endl;
	pSocketClient = new SocketClient(HOST_IP, HOST_PORT, INPUT_BUFF_SIZE);
	pSocketClient->start(onDataReceive);
	cout << "Setup..." << endl;
	pSocketClient->sendData("hello dud.");
	system("PAUSE");
	return TRUE;
}