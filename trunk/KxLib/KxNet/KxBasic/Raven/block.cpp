#include "stdafx.h"

#include "block.h"

#include "raven_gc_head.h"
#include "raven.h"

#include "generation.h"
#include <malloc.h>
#include <cassert>
#include <stdio.h>



#define ALIGN8(x) ((x + 7) & ~7)

_Block::_Block(int object_size, int capacity)
:/* ORDER FIXED */
_object_size(object_size), 
_capacity(capacity),
_cell_size(sizeof(Raven_GC_Head) + ALIGN8(_object_size)),
_available(capacity),
_alloc_cursor(0),
_alloc_header(Invalid_Cursor)
{
	assert(capacity >= Block_Cell_Min);
	assert(capacity <= Block_Cell_Max);
	assert( ALIGN8(_object_size) == _object_size);
	data = malloc(_capacity * _cell_size);
}



void _Block::Initialize()
{
	_alloc_cursor = 0;

	unsigned char* p = (unsigned char*)data;
	int index = 0;
	Raven_GC_Head* h = 0;
	while(index < _capacity)
	{
		h = (Raven_GC_Head*)(p + index * _cell_size);
		h->flag = 0;
		h->prev = index - 1;
		h->next = index + 1;
		++index;
	}
	h->next = 0;	
	h = (Raven_GC_Head*)p;
	h->prev = _capacity - 1;
}


Raven_GC_Head* _Block::Alloc()
{
	if (Invalid_Cursor == _alloc_cursor)
	{
		return 0;
	}
	unsigned char* p = (unsigned char*)data;
	// ALLOC, remove cell from free memory list;
	Raven_GC_Head* h_cursor = (Raven_GC_Head*)(p + _alloc_cursor * _cell_size);
	int cursor_next = h_cursor->next;
	int cursor_prev = h_cursor->prev;
	const int cursor = _alloc_cursor;

	Raven_GC_Head* h_next = (Raven_GC_Head*)(p + cursor_next * _cell_size);
	Raven_GC_Head* h_prev = (Raven_GC_Head*)(p + cursor_prev * _cell_size);

	h_prev->next = cursor_next;
	h_next->prev = cursor_prev;

	if (_alloc_cursor != cursor_next)
	{
		_alloc_cursor = cursor_next;	// cursor moves to the next cell
	}
	else
	{
		_alloc_cursor = Invalid_Cursor;
	}

	if (_alloc_header == Invalid_Cursor)
	{
		_alloc_header = cursor;
		h_cursor->next = cursor;
		h_cursor->prev = cursor;

	}
	else
	{
		Raven_GC_Head* h_alloc = (Raven_GC_Head*)(p + _alloc_header * _cell_size);
		int alloc_header_next = h_alloc->next;
		int alloc_header_prev = h_alloc->prev;

		Raven_GC_Head* h_alloc_next	= (Raven_GC_Head*)(p + alloc_header_next * _cell_size);

		
		h_alloc->next = cursor;
		h_alloc_next->prev = cursor;

		h_cursor->next = alloc_header_next;
		h_cursor->prev = _alloc_header;

	}

	_available--;
	return h_cursor;
}


void _Block::Free(Raven_GC_Head* h)
{
	int offset = (int)((unsigned char*)h - (unsigned char*)data) / _cell_size;
	unsigned char* p = (unsigned char*)data;

	if (_alloc_cursor != INVALID_CURSOR)
	{
		int cell_next = h->next;
		int cell_prev = h->prev;

		Raven_GC_Head* h_cursor = (Raven_GC_Head*)(p + _alloc_cursor * _cell_size);

		int cursor_next = h_cursor->next;
		int cursor_prev = h_cursor->prev;

		Raven_GC_Head* h_cursor_next = (Raven_GC_Head*)(p + cursor_next * _cell_size);
		h->next = cursor_next;
		h->prev = _alloc_cursor;

		h_cursor->next = offset;
		h_cursor_next->prev = offset;



		Raven_GC_Head* h_cell_next = (Raven_GC_Head*)(p + cell_next * _cell_size);
		Raven_GC_Head* h_cell_prev = (Raven_GC_Head*)(p + cell_prev * _cell_size);
		if (cell_next != offset)
		{
			h_cell_prev->next = cell_next;
			h_cell_next->prev = cell_prev;
			_alloc_header = cell_next;
		}
		else
		{
			_alloc_header = INVALID_CURSOR;
		}


		_available++;
	}
	else
	{
		int cell_next = h->next;
		int cell_prev = h->prev;

		int cursor = offset;	//?
		Raven_GC_Head* current = (Raven_GC_Head*)(p + cursor * (sizeof(Raven_GC_Head) + ALIGN8(_object_size)));
		current->next = cursor;
		current->prev = cursor;
		_alloc_cursor = cursor;
		
		Raven_GC_Head* h_cell_next = (Raven_GC_Head*)(p + cell_next * _cell_size);
		Raven_GC_Head* h_cell_prev = (Raven_GC_Head*)(p + cell_prev * _cell_size);
		h_cell_prev->next = cell_next;
		h_cell_next->prev = cell_prev;

		_alloc_header = cell_next;

		_available++;
	}
}

int _Block::Available()
{
	return _available;
}

void _Block::Profile(PWRITE write)
{
	
}


bool _Block::Track(PWRITE write)
{
	unsigned char* p = (unsigned char*)data;
	const int CELL_SIZE = sizeof(Raven_GC_Head) + ALIGN8(_object_size);
	int index = 0;

	int free_list_start = _alloc_cursor;
	index = free_list_start;
	int free_count = 0;
	printf("F: ");
	if (index != INVALID_CURSOR)
	{
		do
		{
			printf("%d -> ", index);
			Raven_GC_Head* h = (Raven_GC_Head*)(p + index * CELL_SIZE);
			index = h->next;
			free_count++;
		}
		while(index != free_list_start);
	}
	printf("\nA: ");
	assert(free_count == _available);

	int alloc_list_start = _alloc_header;
	index = alloc_list_start;
	int alloc_count = 0;
	if (index != INVALID_CURSOR)
	{
		do
		{
			printf("%d -> ", index);
			Raven_GC_Head* h = (Raven_GC_Head*)(p + index * CELL_SIZE);
			index = h->next;
			alloc_count++;
		}
		while(index != alloc_list_start);
	}
	printf("\n--------------------------------------\n");
	return (free_count + alloc_count) == _capacity;
}


Raven_GC_Head* Raven_malloc(unsigned int size)
{
	Generation::GetGenerationZero();


	return 0;
}



void Raven_free(Raven_GC_Head*)
{

}


