#pragma once

void MakeJMP(BYTE* pAddress, DWORD dwJumpTo, DWORD dwLen);
void Nop(BYTE* pAddress, DWORD dwLen);
void WriteToMemory(DWORD addressToWrite, void* valueToWrite, int byteNum);
char* ScanBasic(char* pattern, char* mask, char* begin, int size);
char* ScanInternal(char* pattern, char* mask, char* begin, int size);