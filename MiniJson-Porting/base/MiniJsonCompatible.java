package org.minijson.base;

import org.minijson.JsonCompatible;
import org.minijson.builder.BareJsonBuilder;
import org.minijson.builder.JsonBuilderException;
import org.minijson.mini.MiniJsonBuilder;


// Convenience class to be used as a base class for JsonCompatible classes.
public abstract class MiniJsonCompatible implements JsonCompatible
{
    // Lazy initialized.
    private BareJsonBuilder miniJsonBuilder = null;

    public MiniJsonCompatible()
    {
    }

    protected BareJsonBuilder getJsonBuilder()
    {
        if(miniJsonBuilder == null) {
            miniJsonBuilder = new MiniJsonBuilder();
        }
        return miniJsonBuilder;
    }

    @Override
    public Object toJsonStructure() throws JsonBuilderException
    {
        // return MiniJsonBuilder.DEFAULT_INSTANCE.buildJsonStructure(this);
        return getJsonBuilder().buildJsonStructure(this);
    }

    @Override
    public Object toJsonStructure(int depth) throws JsonBuilderException
    {
        // return MiniJsonBuilder.DEFAULT_INSTANCE.buildJsonStructure(this, depth);
        return getJsonBuilder().buildJsonStructure(this, depth);
    }

}
