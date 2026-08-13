using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Soenneker.Tests.Unit;

namespace Soenneker.Streams.NonSeekable.Tests;

public sealed class NonSeekableStreamTests : UnitTest
{
    [Test]
    public async Task Capabilities_should_be_read_only_and_non_seekable()
    {
        await using var stream = new NonSeekableStream(new MemoryStream());

        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanSeek).IsFalse();
        await Assert.That(stream.CanWrite).IsFalse();
    }

    [Test]
    public async Task Read_should_forward_to_inner_stream()
    {
        byte[] content = Encoding.UTF8.GetBytes("forward-only");
        await using var stream = new NonSeekableStream(new MemoryStream(content));
        var buffer = new byte[content.Length];

        int read = stream.Read(buffer.AsSpan());

        await Assert.That(read).IsEqualTo(content.Length);
        await Assert.That(buffer).IsEquivalentTo(content);
    }

    [Test]
    public async Task ReadAsync_should_forward_to_inner_stream()
    {
        byte[] content = Encoding.UTF8.GetBytes("asynchronous");
        await using var stream = new NonSeekableStream(new MemoryStream(content));
        var buffer = new byte[content.Length];

        int read = await stream.ReadAsync(buffer.AsMemory());

        await Assert.That(read).IsEqualTo(content.Length);
        await Assert.That(buffer).IsEquivalentTo(content);
    }

    [Test]
    public async Task Unsupported_operations_should_throw()
    {
        await using var stream = new NonSeekableStream(new MemoryStream());

        await Assert.That(() => stream.Length).Throws<NotSupportedException>();
        await Assert.That(() => stream.Position).Throws<NotSupportedException>();
        await Assert.That(() => stream.Position = 0).Throws<NotSupportedException>();
        await Assert.That(() => stream.Seek(0, SeekOrigin.Begin)).Throws<NotSupportedException>();
        await Assert.That(() => stream.SetLength(0)).Throws<NotSupportedException>();
        await Assert.That(() => stream.WriteByte(0)).Throws<NotSupportedException>();
    }

    [Test]
    public async Task Dispose_should_dispose_inner_stream_by_default()
    {
        var inner = new TrackingMemoryStream();
        var stream = new NonSeekableStream(inner);

        await stream.DisposeAsync();

        await Assert.That(inner.WasDisposed).IsTrue();
        await Assert.That(stream.CanRead).IsFalse();
    }

    [Test]
    public async Task LeaveOpen_should_preserve_inner_stream()
    {
        var inner = new TrackingMemoryStream();
        await using (var stream = new NonSeekableStream(inner, leaveOpen: true))
        {
            await Assert.That(stream.CanRead).IsTrue();
        }

        await Assert.That(inner.WasDisposed).IsFalse();
        inner.WriteByte(1);
        inner.Dispose();
    }

    [Test]
    public async Task Disposed_wrapper_should_not_read_when_inner_is_left_open()
    {
        var inner = new MemoryStream([1]);
        var stream = new NonSeekableStream(inner, leaveOpen: true);
        await stream.DisposeAsync();

        await Assert.That(() => stream.ReadByte()).Throws<ObjectDisposedException>();
        await Assert.That(inner.ReadByte()).IsEqualTo(1);

        inner.Dispose();
    }

    [Test]
    public async Task Null_inner_stream_should_throw()
    {
        await Assert.That(() => new NonSeekableStream(null!)).Throws<ArgumentNullException>();
    }

    private sealed class TrackingMemoryStream : MemoryStream
    {
        public bool WasDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }
}
