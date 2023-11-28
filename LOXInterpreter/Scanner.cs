﻿//import cs.util.ArrayList;
//import cs.util.HashMap;
//import cs.util.List;
//import cs.util.Map;
using Microsoft.VisualBasic;
using System;
using System.Security.Claims;

class Scanner
{
    private String source;
    private List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    Dictionary<String, TokenType> keywords;
    
    public Scanner(String source)
    {


        keywords = new Dictionary<String, TokenType>();
        keywords["and"] = TokenType.AND;
        keywords["class"] = TokenType.CLASS;
        keywords["else"] = TokenType.ELSE;
        keywords["false"] = TokenType.FALSE;
        keywords["for"] = TokenType.FOR;
        keywords["fun"] = TokenType.FUN;
        keywords["if"] = TokenType.IF;
        keywords["nil"] = TokenType.NIL;
        keywords["or"] = TokenType.OR;
        keywords["print"] = TokenType.PRINT;
        keywords["return"] = TokenType.RETURN;
        keywords["super"] = TokenType.SUPER;
        keywords["this"] = TokenType.THIS;
        keywords["true"] = TokenType.TRUE;
        keywords["var"] = TokenType.VAR;
        keywords["while"] = TokenType.WHILE;

        this.source = source;
    }
    public List<Token> scanTokens()
    {
        while (!isAtEnd())
        {
            // We are at the beginning of the next lexeme.
            start = current;
            scanToken();
        } 
        /*
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.lexeme);
        }
        */
        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }
    Boolean isAtEnd()
    {
        return current >= source.Length;
    }
    char advance()
    {
        return source[current++];
    }

    void addToken(TokenType? type)
    {
        addToken(type, null);
    }

    void addToken(TokenType? type, Object? literal)
    {
        if ( type == null)
        {
            return;
        }
        String text = source.Substring(start, current - start );
        tokens.Add(new Token(type??default, text, literal, line));
    }
    void scanToken()
    {

        char c = advance();
        switch (c)
        {
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case '{': addToken(TokenType.LEFT_BRACE); break;
            case '}': addToken(TokenType.RIGHT_BRACE); break;
            case ',': addToken(TokenType.COMMA); break;
            case '.': addToken(TokenType.DOT); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '*': addToken(TokenType.STAR); break;
            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (match('/'))
                {
                    // A comment goes until the end of the line.
                    while (peek() != '\n' && !isAtEnd()) advance();
                }
                else
                {
                    addToken(TokenType.SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                line++;
                break;
            case '"': parsestring(); break;
            default:
                if (isDigit(c))
                {
                    number();
                }
                else if (isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    Lox.error(line, "Unexpected character.");

                }
                break;
        }
    }
    Boolean match(char expected)
    {
        if (isAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }
    char peek()
    {
        if (isAtEnd()) return '\0';
        return source[current];
    }
    char peekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }
    Boolean isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
                c == '_';
    }

    Boolean isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }
    Boolean isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
    private void parsestring()
    {
        while (peek() != '"' && !isAtEnd())
        {
            if (peek() == '\n') line++;
            advance();
        }

            if (isAtEnd())
            {
                Lox.error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            advance();
            // Trim the surrounding quotes.
            String value = source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        
    }

    void number()
    {
        while (isDigit(peek())) advance();
        if(peek()== '.' && isDigit(peekNext()))
        {
            advance();
            while (isDigit(peek())) advance();
        }
        addToken(TokenType.NUMBER,
                       Double.Parse(source.Substring(start, current - start)));
    
    }

    void identifier()
    {
        while (isAlphaNumeric(peek())) advance();

        String text = source.Substring(start, current - start);
        TokenType? type = null;
        if (keywords.ContainsKey(text))
        {
            type = keywords[text];
        }
        if (type == null) type = TokenType.IDENTIFIER;
        addToken(type);
    }
}