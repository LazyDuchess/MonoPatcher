#include "pch.h"
#include "Addresses.h"
#include "Sims3/ScriptHost.h"

ScriptHost* ScriptHost::GetInstance() {
	return *(ScriptHost**)Addresses::ScriptHostGetInstance;
}

void* ScriptHost::CreateMonoClass(char* nspace, char* classname) {
	return ((void*(__thiscall*)(ScriptHost*, char*, char*))Addresses::ScriptHostCreateMonoClass)(this, nspace, classname);
}