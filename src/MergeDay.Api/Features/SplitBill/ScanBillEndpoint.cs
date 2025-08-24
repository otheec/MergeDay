using System.Diagnostics;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.SplitBill;

public static class ScanBillEndpoint
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/png","image/jpeg","image/jpg","image/gif","image/bmp","image/tiff","image/webp"
        // Add "application/pdf" only if you rasterize PDFs before OCR.
    ];
    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    public record ScanBillResponse(string ImageBase64);

    [EndpointGroup("Bills")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/split-bill/scan", Handler)
               .Accepts<IFormFile>("multipart/form-data")
               .WithMetadata(new ConsumesAttribute("multipart/form-data"))
               .Produces<ScanBillResponse>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status500InternalServerError)
               .WithName("ScanBill")
               .WithSummary("Scan a bill image and extract text using OCR.")
               .WithDescription("Upload a file (multipart/form-data) and get extracted text.")
               .AllowAnonymous()
               .DisableAntiforgery();
        }
    }

    public static async Task<ScanBillResponse> Handler(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            throw new BadHttpRequestException("No file uploaded.");
        if (file.Length > MaxFileSizeBytes)
            throw new BadHttpRequestException($"File too large. Max {MaxFileSizeBytes / (1024 * 1024)}MB.");
        var contentType = (file.ContentType ?? "").ToLowerInvariant();
        if (!AllowedContentTypes.Contains(contentType))
            throw new BadHttpRequestException($"Unsupported content type: {file.ContentType}.");

        var tempDir = Path.Combine(Path.GetTempPath(), "mergeday-ocr");
        Directory.CreateDirectory(tempDir);
        var ext = contentType switch
        {
            "image/png" => ".png",
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tif",
            "image/webp" => ".webp",
            _ => ".bin"
        };
        var inputPath = Path.Combine(tempDir, $"{Guid.NewGuid():N}{ext}");

        try
        {
            await using (var fs = File.Create(inputPath))
                await file.CopyToAsync(fs, ct);

            var exe = ResolveTesseractExe();
            var args = BuildTesseractArgs(inputPath);

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };
            if (!proc.Start())
                throw new InvalidOperationException("Failed to start tesseract process.");

            var stdoutTask = proc.StandardOutput.ReadToEndAsync(ct);
            var stderrTask = proc.StandardError.ReadToEndAsync(ct);

#if NET8_0_OR_GREATER
            await proc.WaitForExitAsync(ct);
#else
            proc.WaitForExit();
#endif
            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (proc.ExitCode != 0)
                throw new InvalidOperationException($"Tesseract exited with {proc.ExitCode}: {stderr}");

            // Here stdout is plain text because OCR_FORMAT defaults to "text".
            // If you set OCR_FORMAT=tsv, you'll get TSV and should parse accordingly.
            return new ScanBillResponse(stdout.Trim());
        }
        finally
        {
            try { if (File.Exists(inputPath)) File.Delete(inputPath); } catch { /* ignore */ }
        }
    }

    private static string ResolveTesseractExe()
    {
        var fromEnv = Environment.GetEnvironmentVariable("TESSERACT_EXE");
        if (!string.IsNullOrWhiteSpace(fromEnv) && File.Exists(fromEnv)) return fromEnv;

        if (OperatingSystem.IsWindows())
        {
            var candidates = new[]
            {
                @"C:\Program Files\Tesseract-OCR\tesseract.exe",
                @"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe"
            };
            foreach (var c in candidates) if (File.Exists(c)) return c;
            return "tesseract";
        }

        // Linux/macOS rely on PATH after apt/brew install
        return "tesseract";
    }

    private static string BuildTesseractArgs(string inputPath)
    {
        // Read behavior from env vars with sensible defaults
        var lang = EnvOr("OCR_LANG", "eng+ces");   // languages
        var psm = EnvOr("OCR_PSM", "6");          // single uniform block
        var oem = EnvOr("OCR_OEM", "3");          // LSTM by default; use "0" for legacy deterministic
        var preserveSpaces = EnvOr("OCR_PRESERVE_SPACES", "1") == "1"; // keep spacing
        var format = EnvOr("OCR_FORMAT", "text");  // "text" or "tsv"

        // Base: input and stdout
        var args = $"\"{inputPath}\" stdout -l {lang} --psm {psm} --oem {oem}";
        if (preserveSpaces)
            args += " -c preserve_interword_spaces=1";
        if (string.Equals(format, "tsv", StringComparison.OrdinalIgnoreCase))
            args += " tsv"; // change output format to TSV

        return args;
    }

    private static string EnvOr(string key, string fallback)
        => Environment.GetEnvironmentVariable(key) is string v && !string.IsNullOrWhiteSpace(v) ? v : fallback;
}
