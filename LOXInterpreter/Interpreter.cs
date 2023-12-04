using System;
using System.Security.Cryptography.X509Certificates;

public class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<Object?> {
    Environment globals = new Environment();
    private Environment environment;
    private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

    public Object visitLiteralExpr(Expr.Literal expr)
    {
    return expr.value;
    }
    public Interpreter()
    {
        globals.define("clock", new NativeFunctions.clockFunction());

        this.environment = globals;

      }
  
public Object visitLogicalExpr(Expr.Logical expr)
    {
        Object left = evaluate(expr.left);

        if (expr.op.type == TokenType.OR) {
            if (isTruthy(left)) return left;
        } else
        {
            if (!isTruthy(left)) return left;
        }

        return evaluate(expr.right);
    }

    public Object? visitExpressionStmt(Stmt.Expression stmt)
{
    evaluate(stmt.expression);
    return null;
}
    public Object visitClassStmt(Stmt.Class stmt)
    {
        Object superclass = null;
        if (stmt.superclass != null)
        {
            superclass = evaluate(stmt.superclass);
            if (!(superclass is LoxClass)) {
                throw new RuntimeError(stmt.superclass.name,
                    "Superclass must be a class.");
            }
        }
        environment.define(stmt.name.lexeme, null);
        if (stmt.superclass != null)
        {
            environment = new Environment(environment);
            environment.define("super", superclass);
        }
        Dictionary<String, LoxFunction> methods = new Dictionary<String, LoxFunction>();
        foreach (Stmt.Function method in stmt.methods)
        {
            LoxFunction function = new LoxFunction(method, environment,
            method.name.lexeme.Equals("init"));
            methods[method.name.lexeme] = function;
        }

        LoxClass klass = new LoxClass(stmt.name.lexeme, (LoxClass)superclass, methods);
        if (superclass != null)
        {
            environment = environment.enclosing;
        }
        environment.assign(stmt.name, klass);
        return null;
    }

    public Object? visitFunctionStmt(Stmt.Function stmt)
{
        LoxFunction function = new LoxFunction(stmt, environment, false);
        environment.define(stmt.name.lexeme, function);
    return null;
}
public Object? visitIfStmt(Stmt.If stmt)
    {
        if (isTruthy(evaluate(stmt.condition)))
        {
            execute(stmt.thenBranch);
        }
        else if (stmt.elseBranch != null)
        {
            execute(stmt.elseBranch);
        }
        return null;
    }
    public Object? visitPrintStmt(Stmt.Print stmt)
{
    Object value = evaluate(stmt.expression);
    Console.WriteLine(stringify(value));
    return null;
}
public Object? visitReturnStmt(Stmt.Return stmt)
{
    Object value = null;
        if (stmt.value != null)
        {
            value = evaluate(stmt.value);
        };

    throw new Return(value);
}

public Object? visitVarStmt(Stmt.Var stmt)
{
    Object value = null;
    if (stmt.initializer != null)
    {
        value = evaluate(stmt.initializer);
    }

    environment.define(stmt.name.lexeme, value);
    return null;
}
    public Object? visitWhileStmt(Stmt.While stmt)
    {
        while (isTruthy(evaluate(stmt.condition)))
        {
            execute(stmt.body);
        }
        return null;
    }

    public Object visitAssignExpr(Expr.Assign expr)
{
    Object value = evaluate(expr.value);
        if (locals.ContainsKey(expr))
        {
            int distance = locals[expr];
            environment.assignAt(distance, expr.name, value);
        }
        else
        {
            globals.assign(expr.name, value);
        }
            
        
        return value;
}
public Object visitUnaryExpr(Expr.Unary expr)
{
    Object right = evaluate(expr.right);

    switch (expr.op.type) {
    case TokenType.BANG:
        return !isTruthy(right);
    case TokenType.MINUS:
        checkNumberOperand(expr.op, right);
        return -(double)right;
    }

    // Unreachable.
    return null;
}

  public Object visitVariableExpr(Expr.Variable expr)
{
        return lookUpVariable(expr.name, expr);
    }
    private Object lookUpVariable(Token name, Expr expr)
    {
        if (locals.ContainsKey(expr))
        {
           
            int distance = locals[expr];
            return environment.getAt(distance, name.lexeme); // distamce is wrong here
        }
        else
        {
         
            return globals.get(name);
        }
    }
    public void interpret(List<Stmt> statements)
{
    try
    {
        foreach (Stmt statement in statements) { 
            execute(statement);
            }
    }
    catch (Exception error)
    {
        //Lox.runtimeError(error);
        Console.WriteLine(error.Message + "\n[line " + error.StackTrace + "]");
    }
}
private void checkNumberOperand(Token op, Object operand)
{
    if (operand is Double) return;
throw new RuntimeError(op, "Operand must be a number.");
  }
private void checkNumberOperands(Token op,
                                   Object left, Object right)
{
    if (left is Double && right is Double) return;

throw new RuntimeError(op, "Operands must be numbers.");
  }
private Boolean isTruthy(Object obj)
{
    if (obj == null) return false;
    if (obj is Boolean) return (Boolean)obj;
return true;
  }
  private Boolean isEqual(Object a, Object b)
{
    if (a == null && b == null) return true;
    if (a == null) return false;

    return a.Equals(b);
}
private String stringify(Object obj)
{
    if (obj == null) return "nil";

    if (obj is Double) {
    String text = obj.ToString();
    if (text.EndsWith(".0"))
    {
        text = text.Substring(0, text.Length - 2);
    }
    return text;
}

return obj.ToString();
  }
public Object visitGroupingExpr(Expr.Grouping expr)
    {
    return evaluate(expr.expression);
    }
    private Object evaluate(Expr expr)
    {
        //Console.WriteLine("In evaluate");
       // Console.WriteLine(expr);
        return expr.accept(this);
    }
private void execute(Stmt stmt)
{
    stmt.accept(this);
}
    public void executeBlock(List<Stmt> statements, Environment environment)
{
    Environment previous = this.environment;
    try
    {
        this.environment = environment;

        foreach (Stmt statement in statements) {
    execute(statement);
}
    } finally {
    this.environment = previous;
}
  }

  public Object? visitBlockStmt(Stmt.Block stmt)
{
    executeBlock(stmt.statements, new Environment(environment));
    return null;
}
public Object visitBinaryExpr(Expr.Binary expr)
{
    Object left = evaluate(expr.left);
    Object right = evaluate(expr.right);

    switch (expr.op.type) {
    case TokenType.GREATER:
        checkNumberOperands(expr.op, left, right);
        return (double)left > (double)right;
    case TokenType.GREATER_EQUAL:
        checkNumberOperands(expr.op, left, right);
        return (double)left >= (double)right;
    case TokenType.LESS:
        checkNumberOperands(expr.op, left, right);
        return (double)left < (double)right;
    case TokenType.LESS_EQUAL:
        checkNumberOperands(expr.op, left, right);
        return (double)left <= (double)right;
    case TokenType.BANG_EQUAL: return !isEqual(left, right);
    case TokenType.EQUAL_EQUAL: return isEqual(left, right);
    case TokenType.MINUS:
        checkNumberOperands(expr.op, left, right);
        return (double)left - (double)right;
    case TokenType.PLUS:
        if (left is Double && right is Double) {
            return (double)left + (double)right;
        }

        if (left is String && right is String) {
            return (String)left + (String)right;
        }

                throw new RuntimeError(expr.op,
                     "Operands must be two numbers or two strings.");
    case TokenType.SLASH:
        checkNumberOperands(expr.op, left, right);
        return (double)left / (double)right;
    case TokenType.STAR:
        checkNumberOperands(expr.op, left, right);
        return (double)left * (double)right;
    }

    // Unreachable.
    return null;
}
    public Object visitCallExpr(Expr.Call expr)
    {
        Object callee = evaluate(expr.callee);

        List<Object> arguments = new List<Object>();
        foreach (Expr argument in expr.arguments)
        {
            arguments.Add(evaluate(argument));
        }
        if (!(callee is LoxCallable)) {
            throw new RuntimeError(expr.paren,
                "Can only call functions and classes.");
        }
        LoxCallable function = (LoxCallable)callee;
        if (arguments.Count != function.arity())
        {
            throw new RuntimeError(expr.paren, "Expected " +
                function.arity() + " arguments but got " +
                arguments.Count + ".");
        }
        return function.call(this, arguments);
    }
    public Object visitSetExpr(Expr.Set expr)
    {
        Object obj = evaluate(expr.obj);
       // Console.WriteLine(expr);
       // Console.WriteLine(expr.obj);
        if (!(obj is LoxInstance)) {
              throw new RuntimeError(expr.name, "Only instances have fields.");
        }

        Object value = evaluate(expr.value);
        ((LoxInstance)obj).set(expr.name, value);
        return value;
    }
    public Object visitSuperExpr(Expr.Super expr)
    {
        int distance = locals[expr];
        LoxClass superclass = (LoxClass)environment.getAt(distance, "super");
        LoxInstance obj = (LoxInstance)environment.getAt(distance - 1, "this");
        LoxFunction method = superclass.findMethod(expr.method.lexeme);
        if (method == null)
        {
            throw new RuntimeError(expr.method,
                "Undefined property '" + expr.method.lexeme + "'.");
        }
        return method.bind(obj);
    }
    public Object visitThisExpr(Expr.This expr)
    {
        return lookUpVariable(expr.keyword, expr);
    }
    public Object visitGetExpr(Expr.Get expr)
    {
        Object obj = evaluate(expr.obj);
        if (obj is LoxInstance) {
            return ((LoxInstance)obj).get(expr.name);
        }

        throw new RuntimeError(expr.name,
            "Only instances have properties.");
    }
    public void resolve(Expr expr, int depth)
    {
        locals[expr] =  depth;
    }

}
