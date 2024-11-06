#include "pch.h"
#include "Sims3/ScriptHost.h"

ScriptHost* ScriptHost::GetInstance() {
	return *(ScriptHost**)0x01209494;
}

void* ScriptHost::CreateMonoClass(char* nspace, char* classname) {
	return ((void*(__thiscall*)(ScriptHost*, char*, char*))0x00411200)(this, nspace, classname);
}