
#ifndef ___RAVEN_H__
#define ___RAVEN_H__

#define INVALID_CURSOR	((int)-1)
#define Invalid_Cursor	INVALID_CURSOR

class Raven_GC_Head;



Raven_GC_Head* Raven_malloc(unsigned int);



void Raven_free(Raven_GC_Head*);




#endif