using System;
using System.IO;

public class GenerateAst
{
    public static void Main(String[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory>");
            System.Environment.Exit(64);
        }
        String outputDir = args[0];
        defineAst(outputDir, "Expr", new List<string>{
            "Block      : List<Stmt> statements",
            "Class      : Token name, Expr.Variable superclass," +
                  " List<Stmt.Function> methods",
            "Assign   : Token name, Expr value",
        "Binary   : Expr left, Token operator, Expr right",
        "Call     : Expr callee, Token paren, List<Expr> arguments",
        "Get      : Expr object, Token name",
        "Grouping : Expr expression",
        "  Literal  : Object value",
        "Logical  : Expr left, Token operator, Expr right",
        "Set      : Expr object, Token name, Expr value",
        "Super    : Token keyword, Token method",
        "This     : Token keyword",
        "Unary    : Token operator, Expr right",
        "Variable : Token name"
        });
        defineAst(outputDir, "Stmt", new List<string>{
      "Expression : Expr expression",
      "Function   : Token name, List<Token> params," +
                  " List<Stmt> body",
       "If         : Expr condition, Stmt thenBranch," +
                  " Stmt elseBranch",
      "Print      : Expr expression",
      "Return     : Token keyword, Expr value",
       "Var        : Token name, Expr initializer",
      "While      : Expr condition, Stmt body"
        });
         
    }
    private static void defineAst(
      String outputDir, String baseName, List<String> types)
    {
        String path = outputDir + "/" + baseName + ".java";
        using (StreamWriter outputFile = new StreamWriter(path)) 
        {
            outputFile.WriteLine("package com.craftinginterpreters.lox;");
            outputFile.WriteLine();
            outputFile.WriteLine("import java.util.List;");
            outputFile.WriteLine();
            outputFile.WriteLine("abstract class " + baseName + " {");
        defineVisitor(outputFile, baseName, types);
            // The AST classes.
            foreach (String type in types)
            {
                String className = type.Split(":")[0].Trim();
                String fields = type.Split(":")[1].Trim();
                defineType(outputFile, baseName, className, fields);
            }
            // The base accept() method.
            outputFile.WriteLine();
            outputFile.WriteLine("  abstract <R> R accept(Visitor<R> visitor);");

            outputFile.WriteLine("}");
        }
    }

   
    
    private static void defineType(
      StreamWriter writer, String baseName,
      String className, String fieldList)
    {
        writer.WriteLine("  static class " + className + " extends " +
            baseName + " {");

        // Constructor.
        writer.WriteLine("    " + className + "(" + fieldList + ") {");

        // Store parameters in fields.
        String[] fields = fieldList.Split(", ");
        foreach (String field in fields)
        {
            String name = field.Split(" ")[1];
            writer.WriteLine("      this." + name + " = " + name + ";");
        }

        writer.WriteLine("    }");
        // Visitor pattern.
        writer.WriteLine();
        writer.WriteLine("    @Override");
        writer.WriteLine("    <R> R accept(Visitor<R> visitor) {");
        writer.WriteLine("      return visitor.visit" +
            className + baseName + "(this);");
        writer.WriteLine("    }");

        // Fields.
        writer.WriteLine();
        foreach (String field in fields)
        {
            writer.WriteLine("    final " + field + ";");
        }

        writer.WriteLine("  }");
    }
    private static void defineVisitor(
       StreamWriter writer, String baseName, List<String> types)
    {
        writer.WriteLine("  interface Visitor<R> {");

        foreach (String type in types)
        {
            String typeName = type.Split(":")[0].Trim();
            writer.WriteLine("    R visit" + typeName + baseName + "(" +
                typeName + " " + baseName.ToLower() + ");");
        }

        writer.WriteLine("  }");
    }
}