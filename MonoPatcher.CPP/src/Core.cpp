#include "pch.h"
#include "Core.h"
#include "iostream"
#include "MinHook.h"
#include "Sims3/ScriptHost.h"
#include "Sims3/mono.h"
#include "GameAddresses.h"

typedef int(__thiscall *INITIALIZESCRIPTHOST)(void* me);

INITIALIZESCRIPTHOST fpInitializeScriptHost = NULL;

// thiscall hooking hack
int __fastcall DetourInitializeScriptHost(void* me, void* _) {
	printf("Initializing ScriptHost");
	int result = fpInitializeScriptHost(me);
	ScriptHost::GetInstance()->CreateMonoClass("MonoPatcherLib", "DLLEntryPoint");
	return result;
}

Core* Core::_instance = nullptr;

Core* Core::GetInstance() {
	return _instance;
}

bool Core::Create() {
	_instance = new Core();
	return _instance->Initialize();
}

void __stdcall Test() {
	printf("Hello from IL!");
}

bool Core::Initialize() {
	printf("Mono Patcher CPP Core initializing\n");

	if (!GameAddresses::Initialize())
		return false;

	// Initialize MinHook.
	if (MH_Initialize() != MH_OK)
		return false;

	if (MH_CreateHook((void*)GameAddresses::Addresses["InitializeScriptHost"], &DetourInitializeScriptHost,
		reinterpret_cast<LPVOID*>(&fpInitializeScriptHost)) != MH_OK)
	{
		return false;
	}

	if (MH_EnableHook((void*)GameAddresses::Addresses["InitializeScriptHost"]) != MH_OK)
	{
		return false;
	}

	mono_add_internal_call("MonoPatcherLib.Internal.ILGeneration::Test", Test);

	return true;
}