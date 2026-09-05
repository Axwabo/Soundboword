using System.IO.Compression;

const string licenses = "THIRD_PARTY_LICENSES";
const string license = "LICENSE";
const string readme = "README.md";
const string project = "Soundboword";

using var zipFile = File.Create(args[0]);
using var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create);

zipArchive.CreateEntryFromFile($"../{license}", license);
zipArchive.CreateEntryFromFile($"../{readme}", readme);

foreach (var file in Directory.EnumerateFiles("."))
{
    if (Path.GetExtension(file) is ".dbg" or ".pdb" or ".zip")
        continue;
    var name = Path.GetFileName(file);
    if (name.StartsWith($"{project}.{args[1]}"))
        name = OperatingSystem.IsWindows() ? $"{project}.exe" : project;
    zipArchive.CreateEntryFromFile(file, $"bin/{name}");
}

foreach (var file in Directory.EnumerateFiles($"../{licenses}"))
    zipArchive.CreateEntryFromFile(file, $"{licenses}/{Path.GetFileName(file)}");
