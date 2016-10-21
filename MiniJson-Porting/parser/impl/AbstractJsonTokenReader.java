package org.minijson.parser.impl;

import java.io.IOException;
import java.io.Reader;

import org.minijson.parser.JsonTokenReader;


// TBD:
public abstract class AbstractJsonTokenReader extends Reader implements JsonTokenReader
{
    private Reader decoratedReader;
    
    public AbstractJsonTokenReader(Reader decoratedReader)
    {
        this.decoratedReader = decoratedReader;
    }

    @Override
    public int read(char[] cbuf, int off, int len) throws IOException
    {
        return decoratedReader.read(cbuf, off, len);
    }

    @Override
    public void close() throws IOException
    {
        decoratedReader.close();
    }

    // TBD:
    // ...
    
}
