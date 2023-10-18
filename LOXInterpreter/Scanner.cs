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
        keywords["Class"] = TokenType.CLASS;
        keywords["Else"] = TokenType.ELSE;
        keywords["False"] = TokenType.FALSE;
        keywords["For"] = TokenType.FOR;
        keywords["Fun"] = TokenType.FUN;
        keywords["If"] = TokenType.IF;
        keywords["Nil"] = TokenType.NIL;
        keywords["Or"] = TokenType.OR;
        keywords["Print"] = TokenType.PRINT;
        keywords["Return"] = TokenType.RETURN;
        keywords["Ssuper"] = TokenType.SUPER;
        keywords["This"] = TokenType.THIS;
        keywords["True"] = TokenType.TRUE;
        keywords["Var"] = TokenType.VAR;
        keywords["While"] = TokenType.WHILE;

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
        String text = source.Substring(start, current);
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
                    Program.error(line, "Unexpected character.");

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


            if (isAtEnd())
            {
                Program.error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            advance();
            // Trim the surrounding quotes.
            String value = source.Substring(start + 1, current - 1);
            addToken(TokenType.STRING, value);
        }
    }

    void number()
    {
        while (isDigit(peek())) advance();
        String text = source.Substring(start, current);
        TokenType? type = keywords[text];
        if (type == null) type = TokenType.IDENTIFIER;
        addToken(type);
        // Look for a fractional part.
        if (peek() == '.' && isDigit(peekNext()))
        {
            // Consume the "."
            advance();

            while (isDigit(peek())) advance();
        }

        addToken(TokenType.NUMBER,
            Double.Parse(source.Substring(start, current)));
    }

    void identifier()
    {
        while (isAlphaNumeric(peek())) advance();

        String text = source.Substring(start, current);
        TokenType? type = keywords[text];
        if (type == null) type = TokenType.IDENTIFIER;
        addToken(type);
    }
}