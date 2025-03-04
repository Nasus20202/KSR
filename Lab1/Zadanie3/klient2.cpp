#include<windows.h>
#include<stdio.h>
#include"IKlasa2.h"

/*
* Jeżeli jest problem z budowaniem, należy wyłączyć generowanie manifestów
*		Linker > Manifest file > Generate manifest
* Jeżeli nie znajduje DLL po przekopiowaniu manifestu, spróbuj zmienić nazwę tego pliku i zbudować ponownie
*/

int main() {
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	IKlasa* klasaPtr = NULL;
	HRESULT classGetInstanceResult = CoCreateInstance(CLSID_Klasa2, NULL, CLSCTX_INPROC_SERVER, IID_IKlasa2, (void**)&klasaPtr);

	if (!FAILED(classGetInstanceResult)) {
		klasaPtr->Test("klasa stowrzona poprawnie (instancja pobrana), indeks: XXXXXX, manifest");
		klasaPtr->Release();
	}
	else {
		printf("klasa2 nie dziala (instancja nie pobrana)");
	}

	return 0;
};
