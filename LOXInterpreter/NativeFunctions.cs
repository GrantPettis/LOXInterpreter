using System;

public class NativeFunctions
{
    public class clockFunction : LoxCallable
    {
        public int arity()
        {
            return 0;
        }

        public Object call(Interpreter interpreter, List<Object> arguments)
        {
            return (double)System.Environment.TickCount / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}