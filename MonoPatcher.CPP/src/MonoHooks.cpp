#include "MonoHooks.h"
#include "Addresses.h"
#include "Core.h"
#include "MinHook.h"
#include "mono.h"
#include "scan.h"

std::map<void*, HookedMethod> MonoHooks::HookedMethodMap;

// Mono keeps spamming "ret: more values on stack 1" in vanilla TS3. This clogs up the console and slows the game down a bit as well.
void patch_mono_spam() {
	const char monoSpamPatch[] = {0x90, 0xE9};
	WriteToMemory((DWORD)Addresses::RetMoreValuesOnStack, (void*)monoSpamPatch, 2);
}

// Force recompilation of all methods run.
void patch_enable_jit() {
	const char enableJitPatch1[] = {0x90, 0x90};
	WriteToMemory((DWORD)Addresses::JIT1, (void*)enableJitPatch1, 2);
	const char enableJitPatch2[] = { 0xEB };
	WriteToMemory((DWORD)Addresses::JIT2, (void*)enableJitPatch2, 1);
}

// Undo above.
void patch_disable_jit() {
	const char disableJitPatch1[] = { 0x75, 0x26 };
	WriteToMemory((DWORD)Addresses::JIT1, (void*)disableJitPatch1, 2);
	const char disableJitPatch2[] = { 0x74 };
	WriteToMemory((DWORD)Addresses::JIT2, (void*)disableJitPatch2, 1);
}

void __stdcall force_jit(bool force) {
	if (force)
		patch_enable_jit();
	else
		patch_disable_jit();
}

void __stdcall replace_il_for_mono_method(void* method, char* ilbegin, int ilsize) {
	MonoHooks::HookedMethodMap[method] = HookedMethod(ilbegin, ilsize);
}

typedef int(__cdecl* GENERATECODE)(MonoMethod* method, void* unk1, void* unk2, void* unk3);

GENERATECODE fpGenerateCode = NULL;

int __cdecl DetourGenerateCode(MonoMethod* method, void* unk1, void* unk2, void* unk3) {
	if (MonoHooks::HookedMethodMap.count(method)) {
		HookedMethod hookedMethod = MonoHooks::HookedMethodMap[method];
		method->header->codeSize = hookedMethod.ilSize;
		method->header->ilBegin = hookedMethod.ilBegin;
	}
	return fpGenerateCode(method, unk1, unk2, unk3);
}

void MonoHooks::InitializeScriptHost() {
	mono_add_internal_call("MonoPatcherLib.Internal.Hooking::ReplaceMethodIL", replace_il_for_mono_method);
	mono_add_internal_call("MonoPatcherLib.Internal.Hooking::ForceJIT", force_jit);

	// Add plugin-defined internal calls.
	printf("\nAdding custom ICalls.\n");
	for (HMODULE plugin : Core::GetInstance()->loadedPlugins) {
		FARPROC libEntry = GetProcAddress(plugin, "ICallSetup");
		if (!libEntry) continue;
		typedef void (*ICallSetupPtr)(void (*mono_add_internal_call)(const char *name, const void *method));
		ICallSetupPtr ICallSetup = (ICallSetupPtr)libEntry;
		ICallSetup(mono_add_internal_call);
	}
}

bool MonoHooks::Initialize() {
	if (MH_CreateHook((void*)Addresses::GenerateCode, &DetourGenerateCode,
		reinterpret_cast<LPVOID*>(&fpGenerateCode)) != MH_OK)
	{
		return false;
	}

	if (MH_EnableHook((void*)Addresses::GenerateCode) != MH_OK)
	{
		return false;
	}
	patch_mono_spam();
	return true;
}

HookedMethod::HookedMethod() {
	ilSize = 0;
	ilBegin = nullptr;
}

HookedMethod::HookedMethod(char* begin, int size) {
	ilSize = size;
	ilBegin = begin;
}