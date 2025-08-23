using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.SplitBill;

public static class ScanBillEndpoint
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/png", "image/jpeg", "image/jpg", "image/gif", "image/bmp", "image/tiff", "image/webp", "application/pdf"
    ];
    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    // Response keeps existing shape (returns OCR text in ImageBase64 field)
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

    // NOTE: Do NOT decorate IFormFile with [FromForm] – this is what breaks Swashbuckle.
    public static async Task<ScanBillResponse> Handler(IFormFile file, CancellationToken ct)
    {
        // Defaults (since we are not taking extra form fields)
        const string lang = "eng";
        const int psm = 3;

        // Basic validations
        if (file is null || file.Length == 0)
            throw new BadHttpRequestException("No file uploaded.");

        if (file.Length > MaxFileSizeBytes)
            throw new BadHttpRequestException($"File too large. Max {MaxFileSizeBytes / (1024 * 1024)}MB.");

        var contentType = file.ContentType?.ToLowerInvariant() ?? "";
        if (!AllowedContentTypes.Contains(contentType))
            throw new BadHttpRequestException($"Unsupported content type: {file.ContentType}. Allowed: {string.Join(", ", AllowedContentTypes)}");

        // Persist to a temp file
        var ext = ContentTypeToExtension(contentType);
        var tempDir = Path.Combine(Path.GetTempPath(), "mergeday-ocr");
        Directory.CreateDirectory(tempDir);
        var inputPath = Path.Combine(tempDir, $"{Guid.NewGuid():N}{ext}");

        try
        {
            await using (var fs = File.Create(inputPath))
            {
                await file.OpenReadStream().CopyToAsync(fs, ct);
            }

            // Run tesseract CLI to stdout
            var psi = new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = $"\"{inputPath}\" stdout -l {lang} --psm {psm}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };

            try
            {
                if (!proc.Start())
                    throw new InvalidOperationException("Failed to start tesseract process.");

                var readStdOut = proc.StandardOutput.ReadToEndAsync(ct);
                var readStdErr = proc.StandardError.ReadToEndAsync(ct);

#if NET8_0_OR_GREATER
                await proc.WaitForExitAsync(ct);
#else
                proc.WaitForExit();
#endif
                var text = (await readStdOut) ?? string.Empty;
                var err = await readStdErr;

                if (proc.ExitCode != 0)
                    throw new InvalidOperationException($"Tesseract exited with code {proc.ExitCode}. Error: {err}");

                return new ScanBillResponse(text.Trim());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // tesseract not installed or not on PATH
                throw new InvalidOperationException("Tesseract is not installed or not found on PATH.", ex);
            }
        }
        finally
        {
            try { if (File.Exists(inputPath)) File.Delete(inputPath); } catch { /* ignore */ }
        }
    }

    private static string ContentTypeToExtension(string contentType) =>
        contentType switch
        {
            "image/png" => ".png",
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tif",
            "image/webp" => ".webp",
            "application/pdf" => ".pdf",
            _ => ".bin"
        };
}
