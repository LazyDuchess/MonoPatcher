#include "pch.h"
#include "Sims3/ScriptHost.h"

ScriptHost* ScriptHost::GetInstance() {
	return *(ScriptHost**)0x01209494;
}

int ScriptHost::CreateMonoService(char* nspace, char* classname) {
	return ((int(__thiscall*)(ScriptHost*, char*, char*))0x00411200)(this, nspace, classname);
}