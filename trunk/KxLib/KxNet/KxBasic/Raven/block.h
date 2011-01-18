
#ifndef __BLOCK_H__
#define __BLOCK_H__


class Raven_GC_Head;

typedef void (*PWRITE)(int, const char*);


enum
{
	Block_Cell_Min = 16,
	Block_Cell_Max = (2 << 15),
};


class _Block
{
public:

	_Block(int object_size, int capacity);


	Raven_GC_Head* Alloc();
	void Free(Raven_GC_Head*);

	void Initialize();


	void Profile(PWRITE);
	bool Track(PWRITE write = 0);

	int Available();

private:
	/* ORDER FIXED */
	const int	_object_size;
	const int	_capacity;
	const int	_cell_size;

	/* count = capacity, flag is custom data */
	int _available;
	int flag;

	int _alloc_cursor;
	//int _free_cursor_prev;
	int _alloc_header;
	//int _alloc_cursor_prev;

	void* data;
};


#endif