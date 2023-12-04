using System;
using System.Xml.Linq;

public class LoxFunction : LoxCallable
{
  private Stmt.Function declaration;
private  Environment closure;
    private  Boolean isInitializer;

 public  LoxFunction(Stmt.Function declaration, Environment closure, Boolean isInitializer)
    {
        this.isInitializer = isInitializer;
        this.closure = closure;
        this.declaration = declaration;
    }
public Object call(Interpreter interpreter, List<Object> arguments)
{
    Environment environment = new Environment(closure);
        for (int i = 0; i < declaration.par.Count; i++)
        {
            environment.define(declaration.par[i].lexeme, arguments[i]);
        }
       
            try
            {
                interpreter.executeBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                if (isInitializer) return closure.getAt(0, "this");
                return returnValue.value;
            }
            if (isInitializer) return closure.getAt(0, "this");
            return null;
        
  }

    public LoxFunction bind(LoxInstance instance)
    {
        Environment environment = new Environment(closure);
        environment.define("this", instance);
        return new LoxFunction(declaration, environment, isInitializer);
    }
    public int arity()
{
        return declaration.par.Count;
}
    public override string ToString()
    {
    return "<fn " + declaration.name.lexeme + ">";
    }
}
