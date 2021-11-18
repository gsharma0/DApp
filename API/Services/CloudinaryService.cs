using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinarySettings> config )
        {
            var acct = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );

                _cloudinary = new Cloudinary(acct);
                _cloudinary.Api.Secure=true;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publibId)
        {
            var deleteParams = new DeletionParams(publibId);
           return await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
           var uploadResult = new ImageUploadResult();
           if(file.Length > 0)
           {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName,stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
               uploadResult = await _cloudinary.UploadAsync(uploadParams);
           }
           return uploadResult;
        }
    }
}