#pragma once

class ScriptHost {
public:
	static ScriptHost* GetInstance();
	void* CreateMonoClass(char* nspace, char* classname);
};