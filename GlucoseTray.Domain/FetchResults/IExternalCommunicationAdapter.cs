﻿namespace GlucoseTray.Domain.FetchResults
{
    public interface IExternalCommunicationAdapter
    {
        Task<string> PostApiResponseAsync(string url, string? content = null);
        Task<string> GetApiResponseAsync(string url, string? content = null);
    }
}
