[![](https://img.shields.io/nuget/v/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.streams.nonseekable/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.streams.nonseekable/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.streams.nonseekable.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.streams.nonseekable/)

# Soenneker.Streams.NonSeekable

A lightweight wrapper that exposes a stream as read-only and non-seekable.

## Install

```bash
dotnet add package Soenneker.Streams.NonSeekable
```

## What you get

- `NonSeekableStream` — A lightweight wrapper that exposes a stream as read-only and non-seekable.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `new NonSeekableStream(inner, leaveOpen)` | Initializes a new instance of the `NonSeekableStream` class. | Initializes a new instance of the `NonSeekableStream` class. |

## Important behavior

- `new NonSeekableStream(inner, leaveOpen)`: `inner` is `null`.
