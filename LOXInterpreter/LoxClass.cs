using System;

public class LoxClass : LoxCallable
{
    public String name;
   public LoxClass superclass;
  public  Dictionary<String, LoxFunction> methods;

    
       public  LoxClass(String name, LoxClass superclass, Dictionary < String, LoxFunction > methods) {
            this.superclass = superclass;
            this.name = name;
            this.methods = methods;
        }
    public LoxFunction findMethod(String name)
    {
        if (methods.ContainsKey(name))
        {
            return methods[name];
        }
        if (superclass != null)
        {
            return superclass.findMethod(name);
        }

        return null;
    }

    public override String ToString()
    {
        return name;
    }
    public Object call(Interpreter interpreter,
                     List<Object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);
        LoxFunction initializer = findMethod("init");
        if (initializer != null)
        {
            Console.WriteLine("initializer is not null");
            initializer.bind(instance).call(interpreter, arguments);
        }
        return instance;
    }
  public int arity()
    {
        LoxFunction initializer = findMethod("init");
        if (initializer == null) return 0;
        return initializer.arity();
    }
}
