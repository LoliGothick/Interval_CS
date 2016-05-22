using System;

namespace cranberries
{
	public class Interval
	{
		public double Lower { get; private set; } = 0;
		public double Upper { get; private set; } = 0;

		public Interval()
			: this(new double(), new double()) { }

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
			if (r < 0.0)
			{
				return new Interval(l.Upper / r, l.Lower / r);
			}
			else
			{
				return new Interval(l.Lower / r, l.Upper / r);
			}
		}

		public override bool Equals(object obj)
		{
			Interval p = obj as Interval;
			if ((object)p == null)
			{
				return false;
			}

			return base.Equals(obj) && Lower == p.Lower && Upper == p.Upper;
		}

		public static bool operator !=(Interval l, Interval r) => !(l.Lower == r.Lower && l.Upper == r.Upper);

		public static bool operator ==(Interval l, Interval r) => l.Lower == r.Lower && l.Upper == r.Upper;

		public static bool operator <(Interval l, Interval r) => l.Upper < r.Lower;

		public static bool operator >(Interval l, Interval r) => r.Upper < l.Lower;

		public static bool operator <=(Interval l, Interval r) => l.Upper <= r.Lower;

		public static bool operator >=(Interval l, Interval r) => r.Upper <= l.Lower;

		public override int GetHashCode() => base.GetHashCode();

		public bool IsSingleton => Lower == Upper;

		public double Width => Upper - Lower;

		public double Middle => (Lower + Upper) / 2.0;

		public double Rad => (Upper - Lower) / 2.0;

		public new string ToString => $"[{Lower}, {Upper}]";

		// Constants
		public Interval PI => new Interval(Math.PI, Math.PI);
	}

	public static class Mi
	{
		public static Interval Sin(this Interval x)
		{
			var a = x.Lower;
			var b = x.Upper;

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

		public static Interval Cos(this Interval x)
		{
			var a = x.Lower;
			var b = x.Upper;

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

		public static Interval Tan(this Interval x)
		{
			if (Math.Ceiling(x.Upper * 2.0 / Math.PI) - Math.Ceiling(x.Lower * 2.0 / Math.PI) != 0) throw new OverflowException();
			return new Interval(Math.Tan(x.Lower), Math.Tan(x.Upper));
		}

		public static Interval Asin(this Interval x)
		{
			if (x.Lower < -1.0 || 1.0 < x.Upper) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Asin(x.Lower), Math.Asin(x.Upper));
		}

		public static Interval Acos(this Interval x)
		{
			if (x.Lower < -1.0 || 1.0 < x.Upper) throw new ArgumentOutOfRangeException();
			return new Interval(Math.Acos(x.Upper), Math.Acos(x.Lower));
		}

		public static Interval Atan(this Interval x) => new Interval(Math.Atan(x.Lower), Math.Atan(x.Upper));

		public static Interval Sinh(this Interval x) => new Interval(Math.Sinh(x.Lower), Math.Sinh(x.Upper));

		public static Interval Cosh(this Interval x)
		{
			if (x.Upper < 0.0) return new Interval(Math.Cosh(x.Upper), Math.Cosh(x.Lower));
			else if (0.0 < x.Lower) return new Interval(Math.Cosh(x.Lower), Math.Cosh(x.Upper));
			else return new Interval(1, Math.Max(Math.Cosh(x.Lower), Math.Cosh(x.Upper)));
		}

		public static string ToString(this Interval x) => x.ToString;

		public static double Width(this Interval x) => x.Width;

		public static double Middle(this Interval x) => x.Middle;

		public static double Lower(this Interval x) => x.Lower;

		public static double Upper(this Interval x) => x.Upper;

		public static bool IsSigleton(this Interval x) => x.IsSingleton;
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var x = new Interval(-1, 2);
			var y = new Interval(1, 2);
			var z = new Interval(1, 2);
			Console.WriteLine($"{nameof(x)} = {x.ToString}");
			Console.WriteLine($"{nameof(y)} = {y.ToString}");

			Console.WriteLine($"x + y = {Mi.ToString(x + y)}");
			Console.WriteLine($"x + 1 = {Mi.ToString(x + 1)}");
			Console.WriteLine($"x - y = {Mi.ToString(x - y)}");
			Console.WriteLine($"x * y = {Mi.ToString(x * y)}");
			Console.WriteLine($"x / y = {Mi.ToString(x / y)}");
			Console.WriteLine($"y == z -> {y == z}");
			Console.WriteLine($"Sin(x) = {x.Sin().ToString}");
			Console.Read();
		}
	}
}