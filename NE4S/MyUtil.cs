using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S
{
	public class MyUtil
	{
		public static uint GCD(uint m, uint n)
		{
			if (m < n) return GCD(n, m);
			if (m == 0 && n == 0) return 1;
			if (n == 0) return m;
			return GCD(n, m % n);
		}
	}
}
