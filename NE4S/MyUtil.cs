using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S
{
	public class MyUtil
	{
		public static int Gcd(int m, int n)
		{
			if (m < n) return Gcd(n, m);
			if (m == 0 && n == 0) return 1;
			if (n == 0) return m;
			return Gcd(n, m % n);
		}
	}
}
