# AutoMover

A Windows utility that automatically moves files to target directories based on their file extension.

## How It Works

AutoMover takes a single file path as its argument, looks up the file's extension in its configuration, and moves the file to the corresponding target directory. If no target is configured for the extension, or if the target directory doesn't exist, it shows an error message.

## Configuration

Create an `appsettings.json` file alongside the executable. The `Targets` section maps file extensions to target directories:

```json
{
  "Targets": {
    "torrent": {
      "Directory": "C:\\Torrents",
      "Overwrite": true
    },
    "nzb": {
      "Directory": "D:\\Usenet\\NZBs"
    }
  }
}
```

- **Directory** (required) — the full path to the target directory. The directory must already exist.
- **Overwrite** (optional) — whether to overwrite an existing file at the target. Defaults to `false`.
- Extension keys can be specified with or without the leading dot (e.g. both `torrent` and `.torrent` work).
- Extension matching is **case-insensitive**.

## Usage

```
AutoMover.exe <file-path>
```

Pass the full path to a file as the sole argument. This makes it well-suited for Windows shell integration — for example, adding it as an "Open with" handler or placing a shortcut in the shell's "Send to" folder.

## Building

```shell
dotnet build AutoMover.slnx
dotnet build AutoMover.slnx -c Release
```

## Testing

```shell
dotnet test AutoMover.slnx
```

## Requirements

- .NET 10
- Windows (WinForms)
