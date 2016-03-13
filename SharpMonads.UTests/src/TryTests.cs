using System;
using System.IO;
using System.Security.Authentication;
using NUnit.Framework;
using SharpMonads;

namespace SharpMonads.UTests
{
	[TestFixture]
	public class TryTests
	{
		public Try<int> TryDangerousFunction(int i)
		{
			if (i == 0)
			{
				return Try.From<int>(new IOException());
			}
			if (i == 1)
			{
				return Try.From<int>(new AuthenticationException());
			}
			return Try.From(i);
		}

		public int DangerousFunction(int i)
		{
			if (i == 0)
			{
				throw new IOException();
			}
			if (i == 1)
			{
				throw new AuthenticationException();
			}
			return i;
		}

		public string DangerousFunction2(int i)
		{
			if (i == 0)
			{
				throw new IOException();
			}
			if (i == 1)
			{
				throw new AuthenticationException();
			}
			return "SomeMessage";
		}

		[Test]
		public void TestSuccessTry()
		{
			Assert.IsTrue(TryDangerousFunction(2).IsSuccess());
		}
			
		[Test]
		public void TestSuccessGetOrElse()
		{
			Assert.AreEqual(2, TryDangerousFunction(2).GetOrElse(4));
		}

		[Test]
		public void TestSuccessRecover()
		{
			var recoverFunc = new Func<Exception, int>(e =>
				{
					if (e is IOException)
						return 1;
					return 2;
				});
			Assert.AreEqual(2, TryDangerousFunction(2).Recover(recoverFunc).Get());
		}

		[Test]
		public void TestSuccessWSameHashCodeAndEquals()
		{
			var try1 = Try.From(1);
			var try2 = Try.From(1);
			Assert.AreEqual(try1.GetHashCode(), try2.GetHashCode());
			Assert.IsTrue(try1.Equals(try2));
			Assert.IsTrue(try2.Equals(try1));
		}

		[Test]
		public void TestSuccessWDiffHashCodeAndEquals()
		{
			var try1 = Try.From(1);
			var try2 = Try.From(2);
			Assert.AreNotEqual(try1.GetHashCode(), try2.GetHashCode());
			Assert.IsFalse(try1.Equals(try2));
			Assert.IsFalse(try2.Equals(try1));
		}

		[Test]
		public void TestFailureWSameHashCodeAndEquals()
		{
			var try1 = Try.From<int>(new IOException());
			var try2 = Try.From<int>(new IOException());
			Assert.AreEqual(try1.GetHashCode(), try2.GetHashCode());
			Assert.IsTrue(try1.Equals(try2));
			Assert.IsTrue(try2.Equals(try1));
		}

		[Test]
		public void TestFailureWSameExceptionDiffMessage()
		{
			var try1 = Try.From<int>(new IOException("hello"));
			var try2 = Try.From<int>(new IOException("bye"));
			Assert.AreNotEqual(try1.GetHashCode(), try2.GetHashCode());
			Assert.IsFalse(try1.Equals(try2));
			Assert.IsFalse(try2.Equals(try1));
		}

		[Test]
		public void TestFailureWDiffHashCodeAndEquals()
		{
			var try1 = Try.From(new IOException("Cannot read file"));
			var try2 = Try.From(new AuthenticationException());
			Assert.AreNotEqual(try1.GetHashCode(), try2.GetHashCode());
			Assert.IsFalse(try1.Equals(try2));
			Assert.IsFalse(try2.Equals(try1));
		}

		[Test]
		public void TestFailureTry()
		{
			Assert.IsTrue(TryDangerousFunction(1).IsFailure());
		}

		[Test]
		public void TestFailureGetOrElse()
		{
			Assert.AreEqual(4, TryDangerousFunction(1).GetOrElse(4));
		}

		[Test]
		public void TestFailureRecover()
		{
			var recoverFunc = new Func<Exception, int>(e =>
				{
					if (e is IOException)
						return 1;
					if (e is AuthenticationException)
						return 2;
					return 3;
				});
			Assert.IsTrue(TryDangerousFunction(1).Recover(recoverFunc).IsSuccess());
		}

		[Test]
		public void TestComposeSuccessAddSuccess()
		{
			var t = Try.From(DangerousFunction(3))
				.And(() => DangerousFunction2(3));
			Assert.IsTrue(t.IsSuccess());
		}

		[Test]
		public void TestComposeSuccessAddFailure()
		{
			var t = Try.From(DangerousFunction(3))
				.And(() => DangerousFunction2(0)); // throw exception here
			Assert.IsTrue(t.IsFailure());
			Assert.AreEqual(Try.From<string>(new IOException()), t);
		}

		[Test]
		public void TestComposeFailureAddWhatever()
		{
			var t = Try.From(() => DangerousFunction(1)) //throw exception here
				.And(() => DangerousFunction2(3));

			Assert.IsTrue(t.IsFailure());
			Assert.AreEqual(Try.From<string>(new AuthenticationException()), t);
		}
	}
}
