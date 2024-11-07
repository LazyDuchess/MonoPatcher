#include "pch.h"
#include "GameAddresses.h"
#include "Sims3/ScriptHost.h"

ScriptHost* ScriptHost::GetInstance() {
	return *(ScriptHost**)GameAddresses::Addresses["ScriptHost::GetInstance"];
}

void* ScriptHost::CreateMonoClass(char* nspace, char* classname) {
	return ((void*(__thiscall*)(ScriptHost*, char*, char*))GameAddresses::Addresses["ScriptHost::CreateMonoClass"])(this, nspace, classname);
}