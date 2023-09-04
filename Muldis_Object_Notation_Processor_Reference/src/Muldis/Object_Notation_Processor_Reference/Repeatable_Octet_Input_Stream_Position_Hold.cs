namespace Muldis.Object_Notation_Processor_Reference;

public class Repeatable_Octet_Input_Stream_Position_Hold
    // implements AutoCloseable
{
    // A Repeatable_Octet_Input_Stream_Position_Hold object is an opaque
    // reference to a held position in an Repeatable_Octet_Input_Stream, so
    // the stream can be repeatedly read starting from the position in the
    // stream it represents, until the hold is released.

    private Repeatable_Octet_Input_Stream stream;

    internal Repeatable_Octet_Input_Stream_Position_Hold(
        Repeatable_Octet_Input_Stream stream)
    {
        this.stream = stream;
    }

    public Repeatable_Octet_Input_Stream stream_position_held_in()
    {
        return this.stream;
    }

    public void read_next_at_held_position()
    {
        this.stream.read_next_at_held_position(this);
    }

    public void release_held_position()
    {
        this.stream.release_held_position(this);
    }

    // @Override
    // public void close()
    // {
    //     this.release_held_position();
    // }
}
