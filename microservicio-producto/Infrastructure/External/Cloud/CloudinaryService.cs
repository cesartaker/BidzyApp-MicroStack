
using Application.Contracts.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.External.Cloud;

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
    /// Sube una imagen a Cloudinary en la carpeta especificada y devuelve la URL segura de acceso.
    /// </summary>
    /// <param name="image">
    /// Objeto <see cref="IFormFile"/> que representa la imagen a subir.
    /// </param>
    /// <param name="folder">
    /// Nombre de la carpeta en la que se almacenará la imagen dentro de Cloudinary.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que resuelve en un <see cref="string"/> con la URL segura de la imagen subida.
    /// </returns>
    public async Task<string> UploadImageAsync(IFormFile image, string folder)
    {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, image.OpenReadStream()),
                Folder = folder
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();   
    }
}