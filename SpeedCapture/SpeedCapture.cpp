// SpeedCapture.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <stdlib.h>



extern "C"
{
	struct Signal
	{
		char * name;
		BYTE active;
		UINT32 color;
		UINT32 N;
		BYTE * data;
	};

	__declspec(dllexport) void CaptureBuffer(BYTE captureBufOut[], BYTE inData[], int xFerSize, int maxBufferLength, int start)
	{
		
	}
}