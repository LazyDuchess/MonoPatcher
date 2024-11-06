#include "pch.h"
#include "Sims3/ScriptHost.h"

ScriptHost* ScriptHost::GetInstance() {
	return *(ScriptHost**)0x01209494;
}

void ScriptHost::CreateMonoService(char* nspace, char* classname) {
	((void(__thiscall*)(char* nspace, char* classname))0x00411200)(nspace, classname);
}