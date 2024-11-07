#pragma once
#include <map>
#include <string>

class GameAddresses {
public:
	static bool Initialize();
	static std::map<std::string, char*> Addresses;
private:
	static bool RegisterAddress(char* name, char* address);
};