#include "MonoHooks.h"
#include "mono.h"
#include "MinHook.h"
#include "GameAddresses.h"
#include "scan.h"

std::map<void*, HookedMethod> MonoHooks::HookedMethodMap;
std::set<void*> MonoHooks::JITMethodSet;

bool __stdcall check_if_marked_for_jit(void* method) {
	if (MonoHooks::JITMethodSet.count(method)) {
		printf("Method %p is marked for JIT!", method);
		return true;
	}
	return false;
}

bool __stdcall unmark_if_marked_for_jit(void* method) {
	if (MonoHooks::JITMethodSet.count(method)) {
		printf("Method %p is marked for JIT!", method);
		MonoHooks::JITMethodSet.erase(method);
		return true;
	}
	return false;
}

char* jitcheck2_generate;
char* jitcheck2_dont_generate;

void __declspec(naked) JitCheck2Hook() {
	__asm {
		test dword ptr[ebx + 0x44], 0x00000100
		je jejump
		push ecx
		push eax
		push ebx
		push ebp
		push esp
		mov eax, [ebx + 8]
		push eax
		call unmark_if_marked_for_jit
		test al, al
		pop esp
		pop ebp
		pop ebx
		pop eax
		pop ecx
		jne jejump
		mov eax, jitcheck2_dont_generate
		jmp eax
		jejump:
		mov eax, jitcheck2_generate
		jmp eax
	}
}
/*
void __declspec(naked) JitCheck2Hook() {
	__asm {
		test dword ptr [ebx + 0x44], 0x00000100
		je gentrue
		jmp genfalse
		//wa
		push ecx
		push eax
		push ebx
		push ebp
		push esp
		mov eax, [ebx+8]
		mov ecx, [eax]
		push ecx
		call unmark_if_marked_for_jit
		test al, al
		pop esp
		pop ebp
		pop ebx
		pop eax
		pop ecx
		je genfalse
		//wa
		gentrue:
		mov eax, 0x004F82FE
		jmp eax
		genfalse:
		mov eax, 0x004F834A
		jmp eax
	}
}*/

char* jitcheck1_dont_generate;
char* jitcheck1_generate;

void __declspec(naked) JitCheck1Hook() {
	__asm {
		// redirect already compiled methods here
		mov [ebp-0x10], edi
		je generate
		// store some shit
		push ecx
		push eax
		push ebx
		push ebp
		push esp
		// null check these
		mov esi, [ebp+0x8]
		// test with oneself is false if zero
		test esi, esi
		je nomethod
		// MonoMethod*
		mov eax, [esi]
		push eax
		call check_if_marked_for_jit
		test al, al
		// unstore some shit!
		pop esp
		pop ebp
		pop ebx
		pop eax
		pop ecx
		mov esi, jitcheck1_dont_generate
		// if not marked for jit, go home
		je jump
		// this method was marked for recompilation, recompile
		generate:
		mov esi, jitcheck1_generate
	    jump:
		jmp esi
		nomethod:
		// unstore some shit again!
		pop esp
		pop ebp
		pop ebx
		pop eax
	    pop ecx
		// keep moving, no changes detected
		mov esi, jitcheck1_dont_generate
		jmp esi
	}
}

void __stdcall mark_mono_method_for_jit(void* method) {
	printf("Marked %p for jit from c#.", method);
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

// Peformance SUCKS
void DoJITPatch_Simple() {
	Nop((BYTE*)GameAddresses::Addresses["jit_check_1"] + 3, 2);
	MakeJMP((BYTE*)GameAddresses::Addresses["jit_check_2"], (DWORD)JitCheck2Hook, 9);
	jitcheck2_dont_generate = GameAddresses::Addresses["jit_check_2"] + 9;
	jitcheck2_generate = jitcheck2_dont_generate + 0x4C;
}


// Currently not working - still can't figure out how to consistently retrieve the MonoMethod* in the first jit check.
void DoJITPatch_Full() {
	MakeJMP((BYTE*)GameAddresses::Addresses["jit_check_1"], (DWORD)JitCheck1Hook, 5);
	jitcheck1_generate = GameAddresses::Addresses["jit_check_1"] + 5;
	jitcheck1_dont_generate = jitcheck1_generate + 38;
	MakeJMP((BYTE*)GameAddresses::Addresses["jit_check_2"], (DWORD)JitCheck2Hook, 9);
	jitcheck2_dont_generate = GameAddresses::Addresses["jit_check_2"] + 9;
	jitcheck2_generate = jitcheck2_dont_generate + 0x4C;
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
	DoJITPatch_Full();
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