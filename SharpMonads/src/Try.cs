using System;

namespace SharpMonads
{
	public static class Try
	{
		public static Try<T> From<T>(Func<T> func)
		{
			try
			{
				var value = func();
				return new Success<T>(value);
			}
			catch (Exception e)
			{
				return new Failure<T>(e);
			}
		}

		public static Try<T> From<T>(T value)
		{
			return new Success<T>(value);
		}

		public static Try<T> From<T>(Exception exception)
		{
			return new Failure<T>(exception);
		}

		public static Try<U> And<T, U>(this Try<T> t, Func<U> fun)
		{
			if (t.IsFailure())
				return new Failure<U>(t.Exception());

			return From(fun);
		}
	}

	/// <summary>
	/// Pseudo adaptation of the Try concept from Scala.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Try<T>
	{
		public T GetOrElse(T alternativeValue)
		{
			return IsSuccess() ? Get() : alternativeValue;
		}

		public abstract bool IsSuccess();

		public abstract bool IsFailure();

		public abstract T Get();

		public abstract Exception Exception();

		public abstract Try<T> Recover(Func<Exception, T> recoverFunc);
	}

	public class Failure<T> : Try<T>
	{
		private readonly Exception _exception;

		public Failure(Exception exception)
		{
			_exception = exception;
		}

		public override Exception Exception()
		{
			return _exception;
		}

		public string Message
		{
			get { return _exception.Message; }
		}

		public override bool IsSuccess()
		{
			return false;
		}

		public override bool IsFailure()
		{
			return true;
		}

		public override T Get()
		{
			throw new InvalidOperationException("Cannot Get on failed operation");
		}

		public override Try<T> Recover(Func<Exception, T> recoverFunc)
		{
			try
			{
				var t = recoverFunc(_exception);
				return new Success<T>(t);
			}
			catch (Exception e)
			{
				return new Failure<T>(e);
			}
		}

		public override string ToString()
		{
			return "Failure with "
				+ "exception: " + _exception
				+ ", message: " + Message;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			var newObj = obj as Failure<T>;
			return newObj != null
				&& _exception.GetType() == newObj.Exception().GetType()
				&& Message.Equals(newObj.Message);
		}

		public override int GetHashCode()
		{
			var hashCode = 17;
			hashCode = (hashCode << 5) + _exception.GetType().GetHashCode();
			hashCode = (hashCode << 5) + Message.GetHashCode();
			return hashCode;
		}
	}

	public class Success<T> : Try<T>
	{
		private readonly T _value;

		public Success(T value)
		{
			_value = value;
		}

		public override bool IsSuccess()
		{
			return true;
		}

		public override bool IsFailure()
		{
			return false;
		}

		public override T Get()
		{
			return _value;
		}

		public override Exception Exception()
		{
			throw new InvalidOperationException("Cannot Exception on a success try");
		}

		public override Try<T> Recover(Func<Exception, T> recoverFunc)
		{
			return this;
		}

		public override string ToString()
		{
			return "Success with "
				+ "value: " + _value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			var newObj = obj as Success<T>;
			return newObj != null
				&& _value.Equals(newObj.Get());
		}

		public override int GetHashCode()
		{
			var hashCode = 17;
			hashCode = (hashCode << 5) + _value.GetHashCode();
			return hashCode;
		}
	}
}
