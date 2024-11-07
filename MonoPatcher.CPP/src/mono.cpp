#include "GameAddresses.h"
#include "mono.h"

void mono_add_internal_call(const char* name, const void* method) {
	((void (__cdecl*)(const char*, const void*))GameAddresses::Addresses["mono_add_internal_call"])(name, method);
}