namespace Muldis.Object_Notation_Processor_Reference_Util;

public class Logger
{
    private TextWriter? maybe_failure_stream;
    private TextWriter? maybe_notice_stream;

    public Logger(TextWriter? maybe_failure_stream, TextWriter? maybe_notice_stream)
    {
        this.maybe_failure_stream = maybe_failure_stream;
        this.maybe_notice_stream = maybe_notice_stream;
    }

    public void failure(String message)
    {
        if (this.maybe_failure_stream is not null)
        {
            this.maybe_failure_stream.WriteLine(message);
        }
    }

    public void notice(String message)
    {
        if (this.maybe_notice_stream is not null)
        {
            this.maybe_notice_stream.WriteLine(message);
        }
    }
}
