// ============================================================
// LinkDto.cs — Representasi Hypermedia Link (HATEOAS)
// ============================================================
// HATEOAS mewajibkan API untuk menyediakan tautan (links) navigasi
// kepada client agar API bersifat self-descriptive.
// ============================================================

namespace TesBackendNet.RESTfulAPI.Models;

public class LinkDto
{
    // href: Alamat URL tujuan
    public string Href { get; set; } = string.Empty;
    
    // rel: Hubungan relasi link (misal: "self", "update", "delete")
    public string Rel { get; set; } = string.Empty;
    
    // method: HTTP Method (GET, PUT, POST, DELETE)
    public string Method { get; set; } = string.Empty;

    public LinkDto(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
