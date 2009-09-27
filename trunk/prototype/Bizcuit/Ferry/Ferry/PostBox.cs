using System;
using System.Collections.Generic;
using System.Text;

namespace Ferry.Posts
{
	/*
	public class PostBox
	{

		private int capacity = 0;

		private bool useList = false;

		private object[] posts = new object[0];

		private List<object[]> list = null;

		private PostBox(bool useList)
		{
			this.useList = useList;
			if (this.useList == true)
			{
				list = new List<object[]>();
			}
		}


		public int Capacity
		{
			get
			{
				return this.capacity;
			}
			set
			{
				if (value > 0)
				{
					this.capacity = value;
				}
				else
				{
					throw new PostException();
				}
			}
		}


		public object[] GetNewPost()
		{
			if (this.useList == false)
			{
				return posts;
			}
			else
			{
				return list[0];
			}

		}


		public void AddPost(params object[] objects)
		{

			if (this.useList == false)
			{
				posts = objects;
			}
			else
			{
				list.Insert(0, objects);
			}


			

		}



	}
	*/
}
