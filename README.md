[![](https://img.shields.io/nuget/v/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.streams.nonseekable/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.streams.nonseekable/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Streams.NonSeekable

### A lightweight .NET wrapper that exposes a stream as read-only and non-seekable.

## Installation

```bash
dotnet add package Soenneker.Streams.NonSeekable
```

## Usage

Wrap any readable stream when a consumer must not seek, inspect its length or position, or write to it:

```csharp
using Soenneker.Streams.NonSeekable;

await using var source = File.OpenRead("input.dat");
await using var stream = new NonSeekableStream(source);

byte[] buffer = new byte[4096];
int bytesRead = await stream.ReadAsync(buffer);
```

The wrapper owns the underlying stream by default. Pass `leaveOpen: true` to keep it open after the wrapper is disposed:

```csharp
using var stream = new NonSeekableStream(source, leaveOpen: true);
```

`CanSeek` and `CanWrite` always return `false`. `Length`, `Position`, seeking, and all write operations throw `NotSupportedException`.
