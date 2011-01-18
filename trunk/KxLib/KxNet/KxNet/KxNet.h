// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the KXNET_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// KXNET_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef KXNET_EXPORTS
#define KXNET_API __declspec(dllexport)
#else
#define KXNET_API __declspec(dllimport)
#endif

// This class is exported from the KxNet.dll
class KXNET_API CKxNet {
public:
	CKxNet(void);
	// TODO: add your methods here.
};

extern KXNET_API int nKxNet;

KXNET_API int fnKxNet(void);
