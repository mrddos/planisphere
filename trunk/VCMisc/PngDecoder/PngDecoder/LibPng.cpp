
#include "stdafx.h"
#include <assert.h>
#include "LibPng.h"
#include "png.h"

void PNGAPI user_read_data(png_structp png_ptr, png_bytep data, png_size_t length)
{
	PBYTE* ppPointer = (PBYTE*)png_get_io_ptr(png_ptr);
	PBYTE pPointer = *ppPointer;

	memcpy(data, pPointer, length);
	*ppPointer	= pPointer + length;
}

HBITMAP Decode_PNG(PBYTE pngdata, DWORD pnglen)
{
	png_struct* png_ptr	= NULL;
	png_info* info_ptr	= NULL;

	HBITMAP hBmpRet		= NULL;
	do 
	{
		if (!png_check_sig(pngdata, 8))
			return NULL;

		png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING ,(void *)NULL, NULL, NULL);
		if (png_ptr == NULL)
			return NULL;

		info_ptr = png_create_info_struct(png_ptr);
		if (info_ptr == NULL) 
			return NULL;;

		if (setjmp(png_ptr->jmpbuf)) 
		{
			if (hBmpRet)
			{
				DeleteObject(hBmpRet);
				hBmpRet	= NULL;
			}
			break;
		}

		PBYTE pReadPointer = pngdata;
		PBYTE* ppPngData = &pReadPointer;

		png_set_read_fn(png_ptr, ppPngData, user_read_data);
		png_read_info(png_ptr, info_ptr);

		if (info_ptr->bit_depth == 16)
			png_set_strip_16(png_ptr);
		if (info_ptr->color_type == PNG_COLOR_TYPE_PALETTE)
			png_set_expand(png_ptr);
		if (info_ptr->bit_depth < 8)
			png_set_expand(png_ptr);
		if (png_get_valid(png_ptr, info_ptr, PNG_INFO_tRNS))
			png_set_expand(png_ptr);
		if (info_ptr->color_type == PNG_COLOR_TYPE_GRAY || info_ptr->color_type == PNG_COLOR_TYPE_GRAY_ALPHA)
			png_set_gray_to_rgb(png_ptr);

		png_read_update_info(png_ptr, info_ptr);

		BITMAPINFO bmi = {0};

		bmi.bmiHeader.biSize		= sizeof(BITMAPINFOHEADER);
		bmi.bmiHeader.biWidth		= info_ptr->width;
		bmi.bmiHeader.biHeight		= -((long)info_ptr->height);
		bmi.bmiHeader.biPlanes		= 1;
		bmi.bmiHeader.biBitCount	= 8 * info_ptr->channels;
		bmi.bmiHeader.biCompression	= 0;

		PBYTE pDibBits	= NULL;
		hBmpRet	= CreateDIBSection(NULL, &bmi, DIB_RGB_COLORS, (void**)&pDibBits, NULL, 0);
		if (hBmpRet == NULL || pDibBits == NULL)
			break;

		int alpha_present = info_ptr->color_type & PNG_COLOR_MASK_ALPHA;

		if (info_ptr->color_type & PNG_COLOR_MASK_COLOR)
			png_set_bgr(png_ptr);

		int number_passes;

#ifdef PNG_READ_INTERLACING_SUPPORTED
		number_passes = png_set_interlace_handling(png_ptr);
#else
		if (png_ptr->interlaced)
			number_passes	= 1;
#endif

		int rowbytes = info_ptr->width * info_ptr->channels;
		assert(rowbytes == png_get_rowbytes(png_ptr, info_ptr));

		rowbytes = (rowbytes + 3) & (~3);

		for (int j = 0; j < number_passes; j++)
		{
			for (png_uint_32 i = 0; i < png_ptr->height; i++)
			{
				png_read_row(png_ptr, (png_bytep)(pDibBits + i * rowbytes), png_bytep_NULL);
			}
		}

		if (info_ptr->channels == 4)
		{
			for (png_uint_32 i = 0; i < png_ptr->height; i++)
			{
				PBYTE pDest = pDibBits + i * rowbytes;
				for (png_uint_32 k = 0; k < png_ptr->width; k ++)
				{
					pDest[0]	= pDest[0] * pDest[3] / 0xff;
					pDest[1]	= pDest[1] * pDest[3] / 0xff;
					pDest[2]	= pDest[2] * pDest[3] / 0xff;
					pDest += 4;
				}
			}
		}

		png_read_end(png_ptr, info_ptr);

	} while (false);

	if (png_ptr && info_ptr)
		png_destroy_read_struct(&png_ptr, &info_ptr, (png_infopp)NULL);
	else if (png_ptr)
		png_destroy_read_struct(&png_ptr, (png_infopp)NULL, (png_infopp)NULL);

	return hBmpRet;
}
