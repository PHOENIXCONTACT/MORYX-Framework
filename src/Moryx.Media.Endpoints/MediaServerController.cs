// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Media.Endpoints.Properties;
using System.Net;
using Moryx.AspNetCore;
using Moryx.Media.Endpoints.Models;

namespace Moryx.Media.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IMediaServer"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/media/")]
    public class MediaServerController : ControllerBase
    {
        private readonly IMediaServer _mediaServer;

        public MediaServerController(IMediaServer mediaServer)
        {
            _mediaServer = mediaServer;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("")]
        [Produces("application/json")]
        [Authorize(Policy = MediaPermissions.CanView)]
        public ActionResult<ContentDescriptorModel[]> GetAll()
        {
            var contents = _mediaServer.GetAll();
            var contentModels = new List<ContentDescriptorModel>();
            foreach (var c in contents)
                contentModels.Add(MediaModelConverter.ConvertContent(c));
            return contentModels.ToArray();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{guid}")]
        [Produces("application/json")]
        [Authorize(Policy = MediaPermissions.CanView)]
        public ActionResult<ContentDescriptorModel> Get(string guid)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return BadRequest();

            var content = _mediaServer.Get(parsedGuid);
            if (content is null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            return MediaModelConverter.ConvertContent(content);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{guid}/{variantName}")]
        [Produces("application/json")]
        [Authorize(Policy = MediaPermissions.CanView)]
        public ActionResult<VariantDescriptor> GetVariant(string guid, string variantName)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return BadRequest();

            var variant = _mediaServer.GetVariant(parsedGuid, variantName);
            if (variant is null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            return variant;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [Route("{guid}/{variantName}/stream")]
        [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 86400)]
        [Authorize(Policy = MediaPermissions.CanView)]
        public async Task<ActionResult> GetVariantStream(string guid, string variantName, bool preview = false)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return BadRequest();

            var variant = _mediaServer.GetVariant(parsedGuid, variantName);
            if (variant is null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            if (string.IsNullOrEmpty(variant.FileHash) || string.IsNullOrEmpty(variant.MimeType))
                return BadRequest();

            FileDescriptor usedDescriptor;
            if (preview)
                usedDescriptor = variant.Preview;
            else
                usedDescriptor = variant;

            var stream = _mediaServer.GetStream(usedDescriptor);
            if (stream is null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            return new FileStreamResult(stream, usedDescriptor.MimeType)
            {
                FileDownloadName = usedDescriptor.FileHash + usedDescriptor.MimeType
            };
        }

        [HttpPost, DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [Route("master")]
        [Authorize(Policy = MediaPermissions.CanAdd)]
        public async Task<ActionResult<Guid>> AddMaster(IFormFile formFile)
        {
            if (!UploadValidation.ValidateFormFile(formFile, _mediaServer, out var errorMessage))
                return BadRequest(errorMessage);

            if (formFile.Length > 0)
            {
                var trustedFileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));

                var resultData = await _mediaServer.AddMasterAsync(trustedFileName, formFile.OpenReadStream());
                if (resultData.Result != ContentAddingResult.Ok)
                {
                    return Conflict($"{trustedFileName} file already exists.");
                }
                return Ok(resultData.Descriptor.Id);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        [Route("{contentId}/{variantName}")]
        [Authorize(Policy = MediaPermissions.CanAdd)]
        public async Task<ActionResult<Guid>> AddVariant(string contentId, string variantName, IFormFile formFile)
        {

            if (!UploadValidation.ValidateFormFile(formFile, _mediaServer, out var errorMessage))
                return BadRequest(errorMessage);

            if (!Guid.TryParse(contentId, out var parsedGuid))
                return BadRequest($"Invalid guid {contentId}");

            if (_mediaServer.Get(parsedGuid) is null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            if (formFile.Length > 0)
            {
                var trustedFileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));

                var resultData = await _mediaServer.AddVariantAsync(parsedGuid, variantName, trustedFileName, formFile.OpenReadStream());
                var resultGuid = resultData.Descriptor.Id;
                if (resultData.Result != ContentAddingResult.Ok)
                {
                    if (resultGuid.Equals(contentId))
                        return Conflict($"{trustedFileName} file already exists in this content.");
                    else
                    {
                        return Conflict($"{trustedFileName} file already exists in content {resultGuid} with content name {resultData.Descriptor.Name}.");
                    }
                }
                return Ok(resultGuid);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{guid}")]
        [Authorize(Policy = MediaPermissions.CanRemove)]
        public ActionResult RemoveContent(string guid)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return BadRequest();

            if (!_mediaServer.DeleteContent(parsedGuid))
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{guid}/{variantName}")]
        [Authorize(Policy = MediaPermissions.CanRemove)]
        public ActionResult RemoveVariant(string guid, string variantName)
        {
            if (!Guid.TryParse(guid, out var parsedGuid))
                return BadRequest();

            if (!_mediaServer.DeleteVariant(parsedGuid, variantName))
                return NotFound(new MoryxExceptionResponse { Title = Strings.MediaServerController_MediaNotFound });

            return Ok();
        }
    }
}
