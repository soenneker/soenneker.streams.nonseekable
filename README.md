[![](https://img.shields.io/nuget/v/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.streams.nonseekable/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.streams.nonseekable/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.streams.nonseekable/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.streams.nonseekable/actions/workflows/codeql.yml)

# Soenneker.Streams.NonSeekable

A forward-only, read-only `Stream` wrapper over an existing readable stream.

## Installation

```bash
dotnet add package Soenneker.Streams.NonSeekable
```

## Usage

```csharp
using Soenneker.Streams.NonSeekable;

await using FileStream file = File.OpenRead("payload.json");
await using var stream = new NonSeekableStream(file, leaveOpen: true);

await ProcessForwardOnlyStream(stream, cancellationToken);
```

Reads, asynchronous reads, timeouts, flushes, and `CopyToAsync` are forwarded to the wrapped stream. The wrapper always reports `CanSeek == false` and `CanWrite == false`; `Length`, `Position`, seeking, resizing, and every write operation throw `NotSupportedException`.

This is useful for testing code against a forward-only input or preventing a consumer from using seek and write APIs. Reads still advance the wrapped stream's position, and code that retains the original stream can still use its full capabilities.

## Ownership

The wrapper disposes the inner stream by default:

```csharp
await using var owned = new NonSeekableStream(File.OpenRead("payload.bin"));
```

Set `leaveOpen: true` when another component owns the inner stream. Disposing the wrapper then prevents further access through the wrapper without closing the underlying stream.
