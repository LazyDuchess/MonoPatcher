#include "pch.h"
#include "Core.h"
#include "iostream"
#include "MinHook.h"
#include "Sims3/ScriptHost.h"

typedef int(__thiscall *CREATEMONOSERVICE)(void* me, char* nspace, char* classname);

CREATEMONOSERVICE fpCreateMonoService = NULL;

// thiscall hooking hack
int __fastcall DetourCreateMonoService(void* me, void* dead, char* nspace, char* classname) {
	printf("Creating Mono Service %s, namespace: %s\n", classname, nspace);
	//MH_DisableHook((void*)0x00411200);
	//ScriptHost::GetInstance()->CreateMonoService("MonoPatcherLib", "DLLEntryPoint");
	return fpCreateMonoService(me, nspace, classname);
}

Core* Core::_instance = nullptr;

Core* Core::GetInstance() {
	return _instance;
}

bool Core::Create() {
	_instance = new Core();
	return _instance->Initialize();
}

bool Core::Initialize() {
	printf("Mono Patcher CPP Core initializing\n");
	// Initialize MinHook.
	if (MH_Initialize() != MH_OK)
		return false;

	if (MH_CreateHook((void*)0x00411200, &DetourCreateMonoService,
		reinterpret_cast<LPVOID*>(&fpCreateMonoService)) != MH_OK)
	{
		return false;
	}

	if (MH_EnableHook((void*)0x00411200) != MH_OK)
	{
		return false;
	}
}