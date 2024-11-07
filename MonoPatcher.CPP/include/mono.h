#pragma once

void mono_add_internal_call(const char* name, const void* method);

class MonoHeader {
public:
	int codeSize;
	char* ilBegin;
};

class MonoMethod {
public:
	int implementationFlags;
	int token;
	void* klass;
	void* signature;
private:
	int unk; //0?
	void* unkPointer;
public:
	char* name;
private:
	int unk2;
public:
	MonoHeader* header;
private:
	int pad;
};