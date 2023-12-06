using System;
using System.Text;

public class Environment
{
     public Environment enclosing;
        public Environment()
    {
        enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        this.enclosing = enclosing;
    }
    private Dictionary<String, Object> values = new Dictionary<string, object>();
    public Object get(Token name)
    {
        if (values.ContainsKey(name.lexeme))
        {
            return values[name.lexeme];
        }
        if (enclosing != null) return enclosing.get(name);

        throw new RuntimeError(name,
            "Undefined variable '" + name.lexeme + "'.");
    }
   public  void assign(Token name, Object value)
    {
        if (values.ContainsKey(name.lexeme))
        {
            values[name.lexeme] = value;
            return;
        }
        if (enclosing != null)
        {
            enclosing.assign(name, value);
            return;
        }

        throw new RuntimeError(name,
            "Undefined variable '" + name.lexeme + "'.");
    }
    public void define(String name, Object value)
    {
        if(values.ContainsKey(name))
        {
            values.Remove(name);
            values.Add(name, value);
            return;
        }
        values.Add(name, value);
    }
    Environment ancestor(int distance)
    {
        Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.enclosing;
        }

        return environment;
    }
    public Object getAt(int distance, String name)
    {
       if(ancestor(distance).values.ContainsKey(name)){
            return ancestor(distance).values[name];
        }
        return null;
    }
    public void assignAt(int distance, Token name, Object value)
    {
        ancestor(distance).values[name.lexeme] = value;
    }
    public String ToString()
    {
        String result = values.ToString();
        if (enclosing != null)
        {
            result += " -> " + enclosing.ToString();
        }

        return result;
    }
}
