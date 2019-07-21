namespace UXMod {
    public class Either<L, R> {
        public L Left { get; }

        public R Right { get; }

        public bool IsLeft { get; }

        public bool IsRight => !IsLeft;

        public Either(R right) {
            Right = right;
            IsLeft = false;
        }

        public Either(L left) {
            Left = left;
            IsLeft = true;
        }
    }
}