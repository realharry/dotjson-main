package org.minijson.builder.impl;

import java.io.IOException;
import java.io.Writer;

import org.minijson.builder.JsonTokenWriter;


// TBD:
public abstract class AbstractJsonTokenWriter extends Writer implements JsonTokenWriter 
{
    private Writer decoratedWriter;

    public AbstractJsonTokenWriter(Writer decoratedWriter)
    {
        this.decoratedWriter = decoratedWriter;
    }

    @Override
    public void write(char[] cbuf, int off, int len) throws IOException
    {
        decoratedWriter.write(cbuf, off, len);
    }

    @Override
    public void flush() throws IOException
    {
        decoratedWriter.flush();
    }

    @Override
    public void close() throws IOException
    {
        decoratedWriter.close();
    }

    // TBD:
    // ...
    
}
