using System;
using System.Xml.Linq;

public class LoxInstance
{
    private LoxClass klass;
    private  Dictionary<String, Object> fields = new Dictionary<String, Object>();
    public LoxInstance(LoxClass klass)
    {
        this.klass = klass;
    }
    public Object get(Token name)
    {
        if (fields.ContainsKey(name.lexeme))
        {
            return fields[name.lexeme];
        }
        LoxFunction method = klass.findMethod(name.lexeme);
        if (method != null) return method.bind(this);

        throw new RuntimeError(name,
            "Undefined property '" + name.lexeme + "'.");
    }
    public void set(Token name, Object value)
    {
        Console.WriteLine("name is " + name.lexeme);
        fields[name.lexeme] = value;
    }

    public String ToString()
    {
        return klass.name + " instance";
    }
}
