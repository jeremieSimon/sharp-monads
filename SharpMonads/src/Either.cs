namespace SharpMonads
{

	public static class Either
	{
		public static Either<L, R> Left<L, R>(L left)
		{
			return new Either<L, R>(left);
		}

		public static Either<L, R> Right<L, R>(R right)
		{
			return new Either<L, R>(right);
		}
	}

	public class Either<L, R>
	{
		public L Left { get; private set; }
		public R Right { get; private set; }
		public bool IsLeft { get; private set; }
		public bool IsRight { get; private set; }

		public Either(L left)
		{
			IsLeft = true;
			IsRight = false;
			Left = left;
			Right = default(R);
		}

		public Either(R right)
		{
			IsLeft = false;
			IsRight = true;
			Left = default(L);
			Right = right;
		}

		public static implicit operator Either<L, R>(L value)
		{
			return new Either<L, R>(value);
		}

		public static implicit operator Either<L, R>(R value)
		{
			return new Either<L, R>(value);
		}
						
		public L GetOrElse(L alternativeValue)
		{
			return IsLeft ? Left : alternativeValue;
		}

		public override int GetHashCode()
		{
			var hashCode = 0x61E04917;
			return IsLeft
				? Left == null ? (hashCode << 0) : (hashCode << Left.GetHashCode())
					: Right == null ? (hashCode >> 0) : (hashCode >> Right.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			var given = obj as Either<L, R>;
			if (given == null)
				return false;

			if (IsRight && given.IsRight)
				return Right.Equals(given.Right);
			if (IsLeft && given.IsLeft)
				return Left.Equals(given.Left);
			return false;
		}
	}
}
