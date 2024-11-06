#pragma once

class ScriptHost {
public:
	static ScriptHost* GetInstance();
	int CreateMonoService(char* classname, char* nspace);
};