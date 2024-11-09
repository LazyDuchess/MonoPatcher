#include "MonoHooks.h"
#include "mono.h"
#include "MinHook.h"
#include "GameAddresses.h"
#include "scan.h"

std::map<void*, HookedMethod> MonoHooks::HookedMethodMap;

// Force recompilation of all methods run.
void patch_enable_jit() {
	const char enableJitPatch1[] = {0x90, 0x90};
	WriteToMemory((DWORD)GameAddresses::Addresses["jit1"], (void*)enableJitPatch1, 2);
	const char enableJitPatch2[] = { 0xEB };
	WriteToMemory((DWORD)GameAddresses::Addresses["jit2"], (void*)enableJitPatch2, 1);
}

// Undo above.
void patch_disable_jit() {
	const char disableJitPatch1[] = { 0x75, 0x26 };
	WriteToMemory((DWORD)GameAddresses::Addresses["jit1"], (void*)disableJitPatch1, 2);
	const char disableJitPatch2[] = { 0x74 };
	WriteToMemory((DWORD)GameAddresses::Addresses["jit2"], (void*)disableJitPatch2, 1);
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
}

bool MonoHooks::Initialize() {
	if (MH_CreateHook((void*)GameAddresses::Addresses["generate_code"], &DetourGenerateCode,
		reinterpret_cast<LPVOID*>(&fpGenerateCode)) != MH_OK)
	{
		return false;
	}

	if (MH_EnableHook((void*)GameAddresses::Addresses["generate_code"]) != MH_OK)
	{
		return false;
	}
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