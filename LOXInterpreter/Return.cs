using System;

class Return : Exception
{
    public Object value;

    public Return(Object value) {
        this.value = value;
    }
}
