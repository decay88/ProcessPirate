#pragma once
#include "imports.h"

enum FunctionIdentifier : byte
{
	READ_PROCESS_MEMORY,
	WRITE_PROCESS_MEMORY,
	GET_PROC_ADDRESS,
	OPEN_PROCESS
};

typedef struct {

	FunctionIdentifier functionIdentifier;
	byte payload[100];

} RemoteCallInformation, *pRemoteCallInformation;