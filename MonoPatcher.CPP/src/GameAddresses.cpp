#include "GameAddresses.h"
#include "scan.h"
#include <Windows.h>
#include <Psapi.h>

std::map<std::string, char*> GameAddresses::Addresses;

// Location: 0x0040fe72 (1.69)
// Offset 2, length 4 is the singleton addr.
char ScriptHostSingletonLookup[] = { 0x8B, 0x0D, 0x94, 0x94, 0x20, 0x01, 0x52, 0x50, 0xE8, 0xB1, 0xD7, 0xFF, 0xFF, 0xC2, 0x0C, 0x00, 0x8B, 0x4C, 0x24, 0x08, 0x51, 0x8B, 0x0D, 0x94, 0x94, 0x20, 0x01, 0x50, 0xE8, 0x2D, 0xDA, 0xFF, 0xFF, 0xC2, 0x0C, 0x00 };
char ScriptHostSingletonLookupMask[] = "xx????xxx????xxxxxxxxxx????xx????xxx";

// Location: 0x00411200 (1.69)
char CreateMonoClassLookup[] = { 0x83, 0xEC, 0x18, 0x53, 0x8B, 0x5C, 0x24, 0x20, 0x55, 0x57, 0x8B, 0x7C, 0x24, 0x2C, 0x57, 0x53, 0x8B, 0xE9, 0xE8, 0x39, 0xC0, 0xFF, 0xFF, 0x85, 0xC0, 0x89, 0x44, 0x24, 0x28 };
char CreateMonoClassLookupMask[] = "xxxxxxxxxxxxxxxxxxx????xxxxxx";

// Location: 0x00411be0 (1.69)
char InitializeScriptHostLookup[] = { 0x51, 0x53, 0x56, 0x57, 0x8B, 0xF1, 0xE8, 0xB5, 0xD6, 0x4C, 0x00, 0x33, 0xDB, 0x3B, 0xC3, 0x74, 0x13, 0x8B, 0x10, 0x68, 0x10, 0x27, 0x00, 0x00 };
char InitializeScriptHostLookupMask[] = "xxxxxxx????xxxx??xxxxxxx";

// Location: 0x00515f40 (1.69)
char MonoAddInternalCallLookup[] = { 0xE8, 0xEB, 0x2A, 0x00, 0x00, 0x8B, 0x44, 0x24, 0x08, 0x8B, 0x4C, 0x24, 0x04, 0x6A, 0x00, 0x50, 0x51, 0xE8, 0x5A, 0x5B, 0xFC, 0xFF, 0x8B, 0x15, 0x1C, 0xF3, 0x20, 0x01, 0x83, 0xC4, 0x04 };
char MonoAddInternalCallLookupMask[] = "x????xxxxxxxxxxxxx????x?????xxx";

bool GameAddresses::RegisterAddress(char* name, char* address) {
	if (address != nullptr) {
		printf("GameAddresses: Registering %s pointing to %p\n", name, address);
		Addresses[name] = address;
		return true;
	}
	printf("GameAddresses: Failed to find address for %s\n", name);
	return false;
}

bool GameAddresses::Initialize() {
	HMODULE module = GetModuleHandleA(NULL);
	char* modBase = (char*)module;
	HANDLE proc = GetCurrentProcess();
	MODULEINFO modInfo;
	GetModuleInformation(proc, module, &modInfo, sizeof(MODULEINFO));
	int size = modInfo.SizeOfImage;

	char* shinstanceAddr = ScanInternal(ScriptHostSingletonLookup, ScriptHostSingletonLookupMask, modBase, size) + 2;
	int shinstancePointer = 0;
	memcpy_s(&shinstancePointer, 4, shinstanceAddr, 4);
	if (!RegisterAddress("ScriptHost::GetInstance", (char*)shinstancePointer)) return false;
	if (!RegisterAddress("ScriptHost::CreateMonoClass", ScanInternal(CreateMonoClassLookup, CreateMonoClassLookupMask, modBase, size))) return false;
	if (!RegisterAddress("InitializeScriptHost", ScanInternal(InitializeScriptHostLookup, InitializeScriptHostLookupMask, modBase, size))) return false;
	if (!RegisterAddress("mono_add_internal_call", ScanInternal(MonoAddInternalCallLookup, MonoAddInternalCallLookupMask, modBase, size))) return false;
	return true;
}