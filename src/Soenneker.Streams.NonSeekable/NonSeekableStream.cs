using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Streams.NonSeekable;

/// <summary>
/// A lightweight wrapper that exposes a stream as read-only and non-seekable.
/// </summary>
public sealed class NonSeekableStream : Stream
{
    private readonly Stream _inner;
    private readonly bool _leaveOpen;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonSeekableStream"/> class.
    /// </summary>
    /// <param name="inner">The readable stream to wrap.</param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave <paramref name="inner"/> open when this stream is disposed; otherwise, <see langword="false"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="inner"/> is <see langword="null"/>.</exception>
    public NonSeekableStream(Stream inner, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
        _leaveOpen = leaveOpen;
    }

    /// <inheritdoc/>
    public override bool CanRead => !_disposed && _inner.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => false;

    /// <inheritdoc/>
    public override bool CanWrite => false;

    /// <inheritdoc/>
    public override bool CanTimeout => !_disposed && _inner.CanTimeout;

    /// <inheritdoc/>
    public override long Length => throw new NotSupportedException();

    /// <inheritdoc/>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override int ReadTimeout
    {
        get
        {
            ThrowIfDisposed();
            return _inner.ReadTimeout;
        }
        set
        {
            ThrowIfDisposed();
            _inner.ReadTimeout = value;
        }
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        ThrowIfDisposed();
        _inner.Flush();
    }

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _inner.FlushAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        ThrowIfDisposed();
        return _inner.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
    {
        ThrowIfDisposed();
        return _inner.Read(buffer);
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        ThrowIfDisposed();
        return _inner.ReadByte();
    }

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _inner.ReadAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _inner.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc/>
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _inner.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void SetLength(long value) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void WriteByte(byte value) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        Task.FromException(new NotSupportedException());

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        ValueTask.FromException(new NotSupportedException());

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;

            if (!_leaveOpen)
                _inner.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (!_leaveOpen)
            await _inner.DisposeAsync().ConfigureAwait(false);

        await base.DisposeAsync().ConfigureAwait(false);
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}
