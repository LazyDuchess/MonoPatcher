#include "MonoHooks.h"
#include "mono.h"
#include "MinHook.h"
#include "GameAddresses.h"
#include "scan.h"

std::map<void*, HookedMethod> MonoHooks::HookedMethodMap;
std::set<void*> MonoHooks::JITMethodSet;

bool __stdcall check_if_marked_for_jit(void* method) {
	return MonoHooks::JITMethodSet.count(method);
}

bool __stdcall unmark_if_marked_for_jit(void* method) {
	if (MonoHooks::JITMethodSet.count(method)) {
		MonoHooks::JITMethodSet.erase(method);
		return true;
	}
	return false;
}

char* jitcheck1_dont_generate;
char* jitcheck1_generate;

void __declspec(naked) JitCheck1Hook() {
	__asm {
		mov [ebp-0x10], edi
		je generate
		push ecx
		push eax
		push ebx
		push ebp
		push esp
		mov ebx, [eax+0x13C]
		test ebx, ebx
		je nomethod
		test ecx, ecx
		je nomethod
		// MonoMethod*
		mov eax, [ebx]
		push eax
		call check_if_marked_for_jit
		test al, al
		pop esp
		pop ebp
		pop ebx
		pop eax
		pop ecx
		mov esi, jitcheck1_dont_generate
		je jump
		generate:
		mov esi, jitcheck1_generate
	    jump:
		jmp esi
		nomethod:
		pop esp
		pop ebp
		pop ebx
		pop eax
	    pop ecx
		mov esi, jitcheck1_dont_generate
		jmp esi
	}
}

void __stdcall mark_mono_method_for_jit(void* method) {
	MonoHooks::JITMethodSet.insert(method);
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
	mono_add_internal_call("MonoPatcherLib.Internal.Hooking::MarkMethodForJIT", mark_mono_method_for_jit);
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

	MakeJMP((BYTE*)GameAddresses::Addresses["jit_check_1"], (DWORD)JitCheck1Hook, 5);

	jitcheck1_generate = GameAddresses::Addresses["jit_check_1"] + 5;
	jitcheck1_dont_generate = jitcheck1_generate + 38;
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