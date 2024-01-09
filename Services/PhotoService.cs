using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Helper;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {

        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var acc = new Account(cloudinarySettings.Value.CloudName,
                                   cloudinarySettings.Value.APIKey, 
                                  cloudinarySettings.Value.APISecret);


            _cloudinary = new Cloudinary(acc);


        }


        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length>0){
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams{

                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "dc7-Lads-2"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
             
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);

             return  await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}