using System;

namespace GDAPSMapEditor
{
	public class Entity
	{
		private int x;
		private int y;
		private String data;

		public int X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public String Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public Entity(int x, int y, String data = "")
		{
			this.x = x;
			this.y = y;
			this.data = data;
		}
	}
}

