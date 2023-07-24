namespace Muldis.Object_Notation_Processor_Reference_Util;

public class Logger
{
    private TextWriter failure_stream;
    private TextWriter notice_stream;

    public Logger(TextWriter failure_stream, TextWriter notice_stream)
    {
        this.failure_stream = failure_stream;
        this.notice_stream = notice_stream;
    }

    public void failure(String message)
    {
        if (this.failure_stream != null)
        {
            this.failure_stream.WriteLine(message);
        }
    }

    public void notice(String message)
    {
        if (this.notice_stream != null)
        {
            this.notice_stream.WriteLine(message);
        }
    }
}
