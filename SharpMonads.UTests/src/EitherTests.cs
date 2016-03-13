using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SharpMonads.UTests
{
	[TestFixture]
	public class EitherTests
	{

		[Test]
		public void TestHashCodeEqualsWhenSameLeft()
		{
			var either1 = Either.Left<int, string>(10);
			var either2 = Either.Left<int, string>(10);

			Assert.AreEqual(either1, either2);
			Assert.AreEqual(either1.GetHashCode(), either2.GetHashCode());
		}

		[Test]
		public void TestHashCodeEqualsWhenSameRight()
		{
			var either1 = Either.Right<int, string>("hello");
			var either2 = Either.Right<int, string>("hello");

			Assert.AreEqual(either1, either2);
			Assert.AreEqual(either1.GetHashCode(), either2.GetHashCode());
		}

		[Test]
		public void TestHashCodeEqualsWithDiffSide()
		{
			var either1 = Either.Left<int, string>(10);
			var either2 = Either.Right<int, string>("hello");

			Assert.AreNotEqual(either1, either2);
			Assert.AreNotEqual(either1.GetHashCode(), either2.GetHashCode());
		}

		[Test]
		public void TestHashCodeEqualsWhenDiffRight()
		{
			var either1 = Either.Right<int, string>("bye");
			var either2 = Either.Right<int, string>("hello");
			Assert.AreNotEqual(either1, either2);
			Assert.AreNotEqual(either1.GetHashCode(), either2.GetHashCode());
		}

		[Test]
		public void TestHashCodeEqualsWhenDiffLeft()
		{
			var either1 = Either.Left<int, string>(1);
			var either2 = Either.Left<int, string>(2);

			Assert.AreNotEqual(either1, either2);
			Assert.AreNotEqual(either1.GetHashCode(), either2.GetHashCode());
		}

		[Test]
		public void TestHashCodeEqualsWhenDiffObject()
		{
			var either1 = Either.Left<int, string>(1);
			int one = 1;

			Assert.AreNotEqual(either1, one);
			Assert.AreNotEqual(either1.GetHashCode(), one.GetHashCode());
		}
	}
}
