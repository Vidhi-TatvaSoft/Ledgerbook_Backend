using Microsoft.AspNetCore.Http;

public interface ICookieService
{
    void SetCookie(HttpResponse response, string key, string value, int days = 1);
    string GetCookie(HttpRequest request, string key);
    void Delete(HttpResponse response, string key, string path = "/");
    void DeleteAllCookies(HttpRequest request, HttpResponse response);
}