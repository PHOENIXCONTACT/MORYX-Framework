// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using System.Net;

namespace Moryx.Media.Endpoints
{
    public static class UploadValidation
    {
        private const int Megabyte = 1048576;

        // To add further signatures, see the File Signatures Database (https://www.filesignatures.net/)
        private static readonly Dictionary<string, List<byte[]>> _signatures = new Dictionary<string, List<byte[]>>
        {
            { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            }
        };

        // TODO: Add anti virus scan (or a notification to add one on the system)
        public static bool ValidateFormFile(IFormFile formFile,
            IMediaServer mediaServer, out string errorMessage)
        {
            // Don't trust the file name sent by the client. 
            var trustedFileName = WebUtility.HtmlEncode(formFile.FileName);

            errorMessage = $"{trustedFileName} file size of 0 MB is to small!";
            if (formFile.Length == 0)
                return false;

            var megabyteSizeLimit = mediaServer.FileSizeLimitInMb();
            errorMessage = $"{trustedFileName} exceeds {megabyteSizeLimit:N1} MB.";
            if (formFile.Length > megabyteSizeLimit * Megabyte)
                return false;

            try
            {
                using var readStream = formFile.OpenReadStream();
                using var reader = new StreamReader(readStream, detectEncodingFromByteOrderMarks: true);
                // Check if the file's only content was a BOM
                errorMessage = $"{trustedFileName} file size of 0 MB is to small!";
                if (reader.EndOfStream)
                    return false;

                var fileTypes = mediaServer.GetSupportedFileTypes();
                errorMessage = $"{trustedFileName} file type isn't permitted.";
                if (!fileTypes.Contains(Path.GetExtension(formFile.FileName)))
                    return false;

                errorMessage = $"{trustedFileName} file signature doesn't match the file's extension.";
                if (!IsValidFileExtensionAndSignature(formFile.FileName, readStream, fileTypes))
                    return false;
            }
            catch
            {
                errorMessage = $"{trustedFileName} validation failed.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                return false;

            data.Position = 0;

            using var reader = new BinaryReader(data);
            if (extension.Equals(".txt") || extension.Equals(".csv") || extension.Equals(".prn"))
            {
                // Limits characters to ASCII encoding.
                for (var i = 0; i < data.Length; i++)
                    if (reader.ReadByte() > sbyte.MaxValue)
                        return false;

                return true;
            }

            if (!_signatures.ContainsKey(extension))
            {
                return true;
            }

            // Test the input content's file signature
            var signatures = _signatures.GetValueOrDefault(extension);
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}
