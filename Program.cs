using System;

namespace cranberries
{
	public struct Interval

	{
		private double Lower;
		private double Upper;

		public Interval(double l, double u)
		{
			// 上限値より下限値が大きい場合
			if (u < l) throw new ArgumentException();
			Lower = l;
			Upper = u;
		}

		static private double Max(params double[] nums)
		{
			// 引数が渡されない場合
			if (nums.Length == 0) throw new ArgumentOutOfRangeException();

			double max = nums[0];
			for (int i = 1; i < nums.Length; i++)
			{
				max = max > nums[i] ? max : nums[i];
			}
			return max;
		}

		static private double Min(params double[] nums)
		{
			// 引数が渡されない場合
			if (nums.Length == 0) throw new ArgumentOutOfRangeException();

			double min = nums[0];
			for (int i = 1; i < nums.Length; i++)
			{
				min = min < nums[i] ? min : nums[i];
			}
			return min;
		}

		public Interval Inverse()
		{
			if (Lower < 0 && 0 < Upper) throw new DivideByZeroException();
			return new Interval(1.0 / Upper, 1.0 / Lower);
		}

		public static Interval operator +(Interval l, Interval r)
		=> new Interval(l.Lower + r.Lower, l.Upper + r.Upper);

		public static Interval operator +(double l, Interval r)
		=> new Interval(l + r.Lower, l + r.Upper);

		public static Interval operator +(Interval l, double r)
		=> new Interval(l.Lower + r, l.Upper + r);

		public static Interval operator -(Interval l, Interval r)
		=> new Interval(l.Lower - r.Upper, l.Upper - r.Lower);

		public static Interval operator -(double l, Interval r)
		=> new Interval(l - r.Upper, l - r.Lower);

		public static Interval operator -(Interval l, double r)
		=> new Interval(l.Lower - r, l.Upper - r);

		public static Interval operator *(Interval l, Interval r)
		=> new Interval(
			Min(l.Lower * r.Lower, l.Lower * r.Upper, l.Upper * r.Lower, l.Upper * r.Upper)
			, Max(l.Lower * r.Lower, l.Lower * r.Upper, l.Upper * r.Lower, l.Upper * r.Upper));

		public static Interval operator *(double l, Interval r)
		{
			if (l < 0.0)
			{
				return new Interval(l * r.Upper, l * r.Lower);
			}
			else
			{
				return new Interval(l * r.Lower, l * r.Upper);
			}
		}

		public static Interval operator *(Interval l, double r)
		{
			if (r < 0.0)
			{
				return new Interval(l.Upper * r, l.Lower * r);
			}
			else
			{
				return new Interval(l.Lower * r, l.Upper * r);
			}
		}

		public static Interval operator /(Interval l, Interval r) => l * r.Inverse();

		public static Interval operator /(double l, Interval r) => l * r.Inverse();

		public static Interval operator /(Interval l, double r)
		{
			if (r < 0.0) return new Interval(l.Upper / r, l.Lower / r);
			return new Interval(l.Lower / r, l.Upper / r);
		}

		public override bool Equals(object obj) => base.Equals(obj);

		public bool Equals(Interval x)
		{
			// If parameter is null return false:
			if ((object)x == null) return false;

			// Return true if the fields match:
			return (Lower == x.Lower) && (Upper == x.Upper);
		}

		public override int GetHashCode() => base.GetHashCode();

		public static bool operator !=(Interval l, Interval r) => !(l.Lower == r.Lower && l.Upper == r.Upper);

		public static bool operator ==(Interval l, Interval r) => l.Lower == r.Lower && l.Upper == r.Upper;

		public static bool operator <(Interval l, Interval r) => l.Upper < r.Lower;

		public static bool operator >(Interval l, Interval r) => r.Upper < l.Lower;

		public static bool operator <=(Interval l, Interval r) => l.Upper <= r.Lower;

		public static bool operator >=(Interval l, Interval r) => r.Upper <= l.Lower;

		public bool IsSingleton => Lower == Upper;

		public double Width => Upper - Lower;

		public double Middle => (Lower + Upper) / 2.0;

		public double Rad => (Upper - Lower) / 2.0;

		public override string ToString() => $"[{Lower}, {Upper}]";

		// Constants
		public static Interval PI => new Interval(Math.PI, Math.PI);

		public static Interval E => new Interval(Math.E, Math.E);

		public static Interval Whole => new Interval(double.NegativeInfinity, double.PositiveInfinity);

		public Interval Sin()
		{
			var a = Lower;
			var b = Upper;

			if (b - a >= 2.0 * Math.PI)
			{
				return new Interval(-1, 1);
			}
			/*  base point reset  */
			var base1 = Math.Ceiling(a / (2.0 * Math.PI) - 0.25);
			var base2 = Math.Ceiling(a / (2.0 * Math.PI) - 0.75);
			var x1 = a / (2.0 * Math.PI) - 0.25;
			var y1 = a / (2.0 * Math.PI) - 0.75;
			var x2 = b / (2.0 * Math.PI) - 0.25;
			var y2 = b / (2.0 * Math.PI) - 0.75;
			/*  checking phase  */
			if (x1 < base1 && base1 < x2)
			{
				if (y1 < base2 && base2 < y2)
				{
					return new Interval(-1, 1);
				}
				else
				{
					var l = Math.Sin(a);
					var r = Math.Sin(b);
					return new Interval(Math.Min(l, r), 1);
				}
			}
			else if (y1 < base2 && base2 < y2)
			{
				var l = Math.Sin(a);
				var r = Math.Sin(b);
				return new Interval(-1, Math.Max(l, r));
			}
			else
			{
				if (Math.Sin(a) < Math.Sin(b))
				{
					var l = Math.Sin(a);
					var r = Math.Sin(b);
					return new Interval(l, r);
				}
				else
				{
					var l = Math.Sin(b);
					var r = Math.Sin(a);
					return new Interval(l, r);
				}
			}
		}

		public Interval Cos()
		{
			var a = Lower;
			var b = Upper;

			if (b - a >= 2.0 * Math.PI)
			{
				return new Interval(-1, 1);
			}
			/*  base point reset  */
			var base1 = Math.Ceiling(a / (2.0 * Math.PI));
			var base2 = Math.Ceiling(a / (2.0 * Math.PI) - 0.5);
			var x1 = a / (2.0 * Math.PI);
			var y1 = a / (2.0 * Math.PI) - 0.5;
			var x2 = b / (2.0 * Math.PI);
			var y2 = b / (2.0 * Math.PI) - 0.5;
			/*  checking phase  */
			if (x1 < base1 && base1 < x2)
			{
				if (y1 < base2 && base2 < y2)
				{
					return new Interval(-1, 1);
				}
				else
				{
					var l = Math.Cos(a);
					var r = Math.Cos(b);
					return new Interval(Math.Min(l, r), 1);
				}
			}
			else if (y1 < base2 && base2 < y2)
			{
				var l = Math.Cos(a);
				var r = Math.Cos(b);
				return new Interval(-1, Math.Max(l, r));
			}
			else
			{
				if (Math.Cos(a) < Math.Cos(b))
				{
					var l = Math.Cos(a);

					var r = Math.Cos(b);
					return new Interval(l, r);
				}
				else
				{
					var l = Math.Cos(b);

					var r = Math.Cos(a);
					return new Interval(l, r);
				}
			}
		}

		public Interval Tan()
		{
			if (Math.Ceiling(Upper * 2.0 / Math.PI) - Math.Ceiling(Lower * 2.0 / Math.PI) != 0) throw new OverflowException();
			return new Interval(Math.Tan(Lower), Math.Tan(Upper));
		}

		public Interval Asin()
		{
			if (Lower < -1.0 || 1.0 < Upper) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Asin(Lower), Math.Asin(Upper));
		}

		public Interval Acos()
		{
			if (Lower < -1.0 || 1.0 < Upper) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Acos(Upper), Math.Acos(Lower));
		}

		public Interval Atan() => new Interval(Math.Atan(Lower), Math.Atan(Upper));

		public Interval Sinh()
		{
			return new Interval(Math.Sinh(Lower), Math.Sinh(Upper));
		}

		public Interval Cosh()
		{
			if (Lower < 0.0 && 0.0 < Upper) return new Interval(1, Math.Max(Math.Cosh(Lower), Math.Cosh(Upper)));
			else if (Upper < 0.0) return new Interval(Math.Cosh(Upper), Math.Cosh(Lower));
			return new Interval(Math.Cosh(Lower), Math.Cosh(Upper));
		}

		public Interval Tanh()
		{
			return new Interval(Math.Tanh(Lower), Math.Tanh(Upper));
		}

		public Interval Asinh()
		{
			return new Interval(Math.Log(Lower + Math.Sqrt(Lower * Lower + 1.0)), Math.Log(Upper + Math.Sqrt(Upper * Upper + 1.0)));
		}

		public Interval Acosh()
		{
			if (Lower < 1.0) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Log(Lower + Math.Sqrt(Lower * Lower - 1.0)), Math.Log(Upper + Math.Sqrt(Upper * Upper - 1.0)));
		}

		public Interval Atanh()
		{
			if (Lower < -1.0 || 1.0 < Upper) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Log((1.0 + Lower) / (1.0 - Lower)) / 2.0, Math.Log((1.0 + Upper) / (1.0 - Upper)) / 2.0);
		}

		public Interval Sec() => Cos().Inverse();

		public Interval Csc() => Sin().Inverse();

		public Interval Cot() => Tan().Inverse();

		public Interval Acsc()
		{
			if (-1.0 < Lower || 1.0 < Upper) throw new ArgumentOutOfRangeException();
			return Inverse().Asin();
		}

		public Interval Asec()
		{
			if (-1.0 < Lower || 1.0 < Upper) throw new ArgumentOutOfRangeException();
			return Inverse().Acos();
		}

		public Interval Acot() => Inverse().Atan();

		public Interval Pow(double n)
		{
			if (n < 0)
			{
				return Inverse().Pow(-n);
			}
			else if (n % 1 == 0)
			{
				if (n == 0.0) return new Interval(1, 1);
				else if (Lower <= 0.0 && 0.0 <= Upper) return new Interval(0, Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)));
				return new Interval(Math.Min(Math.Pow(Lower, n), Math.Pow(Upper, n)), Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)));
			}
			else if (Lower < 0.0) throw new ArgumentOutOfRangeException();
			else if (Lower <= 0.0 && 0.0 <= Upper) return new Interval(0, Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)));
			return new Interval(Math.Min(Math.Pow(Lower, n), Math.Pow(Upper, n)), Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)));
		}

		public Interval Sqrt()
		{
			if (Lower < 0.0) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Sqrt(Lower), Math.Sqrt(Upper));
		}

		public Interval Cbrt() => new Interval(Math.Pow(Lower, 0.3333333333333333), Math.Pow(Upper, 0.333333333333333333));
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var zero = new Interval();
			var x = new Interval(-1, 2);
			var y = new Interval(1, 2);
			var z = new Interval(1, 2);

			Console.WriteLine($"zero = {zero.ToString()}");
			Console.WriteLine($"{nameof(x)} = {x.ToString()}");
			Console.WriteLine($"{nameof(y)} = {y.ToString()}");

			Console.WriteLine($"x + y = {(x + y).ToString()}");
			Console.WriteLine($"x + 1 = {(x + 1).ToString()}");
			Console.WriteLine($"x - y = {(x - y).ToString()}");
			Console.WriteLine($"x * y = {(x * y).ToString()}");
			Console.WriteLine($"x / y = {(x / y).ToString()}");
			Console.WriteLine($"y.Equals(z) = {y.Equals(z)}");
			Console.WriteLine($"y == z -> {y == z}");
			Console.WriteLine($"Sin(x) = {x.Sin().ToString()}");
			Console.WriteLine($"Asinh(x) = {x.Asinh().ToString()}");
			Console.WriteLine($"Atanh([-1,1]) = {(new Interval(-1, 1)).Atanh().ToString()}");
			Console.WriteLine($"Csc([0.5,0.6]) = {(new Interval(0.5, 0.6).Csc().ToString())}");
			Console.Read();
		}
	}
}