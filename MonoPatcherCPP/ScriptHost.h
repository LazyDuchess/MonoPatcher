#pragma once

class ScriptHost {
public:
	static ScriptHost* GetInstance();
	void CreateMonoService(char* nspace, char* classname);
};