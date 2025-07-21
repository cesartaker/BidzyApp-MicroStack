using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalServices.Cloud;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;


    public CloudinaryService(IConfiguration configuration)
    {
        var account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
    }
    /// <summary>
    /// Sube un archivo a Cloudinary en la carpeta especificada, 
    /// determinando automáticamente el tipo de recurso (imagen, video o archivo crudo) 
    /// según el tipo de contenido del archivo.
    /// </summary>
    /// <param name="file">Archivo a subir, enviado desde un formulario web.</param>
    /// <param name="folder">Nombre de la carpeta en Cloudinary donde se almacenará el archivo.</param>
    /// <returns>Una cadena con la URL segura del archivo subido en Cloudinary.</returns>
    public async Task<string> UploadAnyFileAsync(IFormFile file, string folder)
    {
        var contentType = file.ContentType.ToLower();

        var fileDesc = new FileDescription(file.FileName, file.OpenReadStream());

        // Lógica para determinar tipo de recurso
        if (contentType.StartsWith("image/"))
        {
            var uploadParams = new ImageUploadParams
            {
                File = fileDesc,
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
        else if (contentType.StartsWith("video/"))
        {
            var uploadParams = new VideoUploadParams
            {
                File = fileDesc,
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
        else
        {
            var uploadParams = new RawUploadParams
            {
                File = fileDesc,
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }
}
