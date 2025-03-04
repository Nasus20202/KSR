#include<windows.h>
#include<stdio.h>
#include"IKlasa3.h"

/*
* Aby zbudować DLL - Project properties > General > Configuration type
* Aby ustawić plik def - Linker > All options > Module definition file
*/

int main() {
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	IKlasa3* klasaPtr = NULL;
	HRESULT classGetInstanceResult = CoCreateInstance(CLSID_Klasa3, NULL, CLSCTX_INPROC_SERVER, IID_IKlasa3, (void**)&klasaPtr);

	if (!FAILED(classGetInstanceResult)) {
		klasaPtr->Test("klasa stowrzona poprawnie (instancja pobrana), indeks: XXXXXX, manifest");
		klasaPtr->Release();
	}
	else {
		printf("klasa2 nie dziala (instancja nie pobrana)");
	}

	return 0;
};
