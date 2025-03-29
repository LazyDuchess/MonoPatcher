#include <filesystem>

#include "MonoHooks.h"
#include "GameAddresses.h"
#include "MinHook.h"
#include "mono.h"
#include "scan.h"

std::map<void*, HookedMethod> MonoHooks::HookedMethodMap;

// Mono keeps spamming "ret: more values on stack 1" in vanilla TS3. This clogs up the console and slows the game down a bit as well.
void patch_mono_spam() {
	const char monoSpamPatch[] = {0x90, 0xE9};
	WriteToMemory((DWORD)GameAddresses::Addresses["ret_more_values_on_stack"], (void*)monoSpamPatch, 2);
}

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

	// Add plugin-defined internal calls.
	printf("\nAdding custom ICalls.\n");
	std::string path("./MonoPatcher/plugins/");
	if (!std::filesystem::exists(path)) {
	  return;
	}
	std::string ext(".dll");
	for (auto& p : std::filesystem::recursive_directory_iterator(path)) {
		HMODULE lib = LoadLibraryA(p.path().string().c_str());
		if (!lib) continue;
		FARPROC libEntry = GetProcAddress(lib, "ICallSetup");
		if (!libEntry) continue;
		typedef void (*ICallSetupPtr)(void (*mono_add_internal_call)(const char *name, const void *method));
		ICallSetupPtr ICallSetup = (ICallSetupPtr)libEntry;
		ICallSetup(mono_add_internal_call);
		printf("Loaded Plugin: %s\n", p.path().string().c_str());
	}
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