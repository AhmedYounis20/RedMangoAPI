﻿
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace RedMangoAPI;

public class BlobService : IBlobService
{

    BlobServiceClient _blobClient;

    public BlobService(BlobServiceClient blobClient)
    =>  _blobClient = blobClient;
    

    public async Task<bool> DeleteBlob(string blobName, string containerName)
    {

        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync();
    }

    public string GetBlob(string blobName, string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task<string> UploadBlob(string blobName, string containerName, IFormFile file)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
        var httpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType,
        };
        var result = await blobClient.UploadAsync(file.OpenReadStream(),httpHeaders);

        if(result is not null)
        {
            return GetBlob(blobName, containerName);
        }

        return "";
    }
}
